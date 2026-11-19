using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using FleetManagementAPI;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
    WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
};
var builder = WebApplication.CreateBuilder(options);

// KESİN PORT KİLİDİ (5055 portunu tüm ağ arayüzlerinde zorlar)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5055);
});

builder.Services.AddControllers();

var dbPath = Path.Combine(AppContext.BaseDirectory, "garaj_verileri.db");
builder.Services.AddDbContext<UygulamaDbContext>(opt =>
    opt.UseSqlite($"Data Source={dbPath}"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UygulamaDbContext>();
    db.Database.EnsureCreated();
}

app.UseDefaultFiles();
app.UseStaticFiles();

// 1. ADMIN VE ŞOFÖR AYRIŞTIRILMIŞ GİRİŞ KONTROLÜ (Oturum Çerezi - Tarayıcı kapanınca sıfırlanır)
app.MapPost("/api/login", async (HttpContext ctx) =>
{
    var data = await ctx.Request.ReadFromJsonAsync<Dictionary<string, string>>();
    if (data != null && data.TryGetValue("user", out var u) && data.TryGetValue("pass", out var p))
    {
        string? rol = null;
        if (u == "admin" && p == "1234") rol = "admin";
        else if (u == "sofor" && p == "5678") rol = "sofor";

        if (rol != null)
        {
            ctx.Response.Cookies.Append("FiloAuth", $"GARAJ_{rol.ToUpper()}_2026", new CookieOptions
            {
                HttpOnly = true
            });
            return Results.Ok(new { basarili = true, rol });
        }
    }
    return Results.Unauthorized();
});

app.MapPost("/api/logout", (HttpContext ctx) =>
{
    ctx.Response.Cookies.Delete("FiloAuth");
    return Results.Ok();
});

app.MapGet("/api/auth-check", (HttpContext ctx) =>
{
    var cookie = ctx.Request.Cookies["FiloAuth"];
    if (cookie == "GARAJ_ADMIN_2026") return Results.Ok(new { rol = "admin" });
    if (cookie == "GARAJ_SOFOR_2026") return Results.Ok(new { rol = "sofor" });
    return Results.Unauthorized();
});

// YETKİ VE GÜVENLİK DUVARI (Şoförlerin DELETE ve yönetim işlemlerini engeller)
app.Use(async (ctx, next) =>
{
    var path = ctx.Request.Path.Value ?? "";
    if (path.StartsWith("/api/") && path != "/api/login" && path != "/api/auth-check" && path != "/api/logout")
    {
        var cookie = ctx.Request.Cookies["FiloAuth"];
        bool isAdmin = cookie == "GARAJ_ADMIN_2026";
        bool isSofor = cookie == "GARAJ_SOFOR_2026";

        if (!isAdmin && !isSofor)
        {
            ctx.Response.StatusCode = 401;
            return;
        }

        if (isSofor)
        {
            if (ctx.Request.Method == "DELETE" ||
                (path.StartsWith("/api/Araclar", StringComparison.OrdinalIgnoreCase) && ctx.Request.Method != "GET") ||
                (path.StartsWith("/api/Suruculer", StringComparison.OrdinalIgnoreCase) && ctx.Request.Method != "GET") ||
                path.Contains("/excel", StringComparison.OrdinalIgnoreCase))
            {
                ctx.Response.StatusCode = 403;
                return;
            }
        }
    }
    await next();
});

app.MapControllers();

// 2. WI-FI MOBİL ERİŞİM İÇİN YEREL IP TESPİTİ
string localIp = "127.0.0.1";
try
{
    using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
    socket.Connect("8.8.8.8", 65530);
    localIp = (socket.LocalEndPoint as IPEndPoint)?.Address.ToString() ?? "127.0.0.1";
}
catch { }

var localUrl = "http://127.0.0.1:5055";

app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine($"[PC Erisim]: {localUrl}");
    Console.WriteLine($"[Mobil Wi-Fi Erisim]: http://{localIp}:5055");
    Task.Run(async () =>
    {
        await Task.Delay(1000);
        try
        {
            Process.Start(new ProcessStartInfo { FileName = "chrome.exe", Arguments = localUrl, UseShellExecute = true });
        }
        catch
        {
            Process.Start(new ProcessStartInfo { FileName = localUrl, UseShellExecute = true });
        }
    });
});

app.Run();
