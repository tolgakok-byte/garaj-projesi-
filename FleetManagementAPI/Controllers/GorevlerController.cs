using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FleetManagementAPI.Models;

namespace FleetManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GorevlerController : ControllerBase
    {
        private readonly UygulamaDbContext _context;

        public GorevlerController(UygulamaDbContext context)
        {
            _context = context;
        }

        // GET: api/Gorevler (Plaka ve Tarih Filtreli)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetGorevler(
            [FromQuery] string? plaka,
            [FromQuery] DateTime? baslangic,
            [FromQuery] DateTime? bitis)
        {
            var query = from g in _context.Gorevlendirmeler
                        join a in _context.Araclar on g.AracID equals a.AracID into aracGroup
                        from arac in aracGroup.DefaultIfEmpty()
                        join s in _context.Suruculer on g.SurucuID equals s.SurucuID into surucuGroup
                        from surucu in surucuGroup.DefaultIfEmpty()
                        select new
                        {
                            g.GorevID,
                            g.AracID,
                            Plaka = arac != null ? arac.Plaka : "Bilinmiyor",
                            g.SurucuID,
                            SurucuAd = surucu != null ? (surucu.Ad + " " + surucu.Soyad) : "Bilinmiyor",
                            g.Aciklama,
                            g.BaslangicTarihi,
                            g.BitisTarihi,
                            g.CikisKm,
                            g.DonusKm
                        };

            if (!string.IsNullOrWhiteSpace(plaka))
                query = query.Where(x => x.Plaka.Contains(plaka.ToUpper()));

            if (baslangic.HasValue)
                query = query.Where(x => x.BaslangicTarihi.Date >= baslangic.Value.Date);

            if (bitis.HasValue)
                query = query.Where(x => x.BaslangicTarihi.Date <= bitis.Value.Date);

            return await query.OrderByDescending(g => g.GorevID).ToListAsync();
        }

        // POST: api/Gorevler
        [HttpPost]
        public async Task<ActionResult<GorevKayit>> PostGorev([FromBody] GorevKayit gorevlendirme)
        {
            gorevlendirme.BaslangicTarihi = DateTime.Now;
            var arac = await _context.Araclar.FindAsync(gorevlendirme.AracID);
            if (arac != null)
            {
                gorevlendirme.CikisKm = arac.Kilometre ?? 0;
                arac.Durum = "Görevde";
            }

            _context.Gorevlendirmeler.Add(gorevlendirme);
            await _context.SaveChangesAsync();
            return Ok(gorevlendirme);
        }

        // PUT: api/Gorevler/tamamla/5?donusKm=125400
        [HttpPut("tamamla/{id}")]
        public async Task<IActionResult> GoreviTamamla(int id, [FromQuery] int donusKm)
        {
            var gorev = await _context.Gorevlendirmeler.FindAsync(id);
            if (gorev == null) return NotFound();

            if (gorev.CikisKm.HasValue && donusKm < gorev.CikisKm.Value)
                return BadRequest("Dönüş kilometresi çıkış kilometresinden küçük olamaz.");

            gorev.BitisTarihi = DateTime.Now;
            gorev.DonusKm = donusKm;

            var arac = await _context.Araclar.FindAsync(gorev.AracID);
            if (arac != null)
            {
                arac.Durum = "Aktif";
                arac.Kilometre = donusKm;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Gorevler/excel
        [HttpGet("excel")]
        public async Task<IActionResult> ExportExcel()
        {
            var liste = await (from g in _context.Gorevlendirmeler
                               join a in _context.Araclar on g.AracID equals a.AracID into ag
                               from arac in ag.DefaultIfEmpty()
                               join s in _context.Suruculer on g.SurucuID equals s.SurucuID into sg
                               from surucu in sg.DefaultIfEmpty()
                               orderby g.GorevID descending
                               select new
                               {
                                   g.GorevID,
                                   Plaka = arac != null ? arac.Plaka : "-",
                                   Surucu = surucu != null ? (surucu.Ad + " " + surucu.Soyad) : "-",
                                   g.Aciklama,
                                   g.BaslangicTarihi,
                                   g.BitisTarihi,
                                   g.CikisKm,
                                   g.DonusKm
                               }).ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Gorev ID;Plaka;Surucu;Aciklama;Cikis Tarihi;Donus Tarihi;Cikis KM;Donus KM;Yapilan KM");

            foreach (var item in liste)
            {
                var yapilanKm = (item.DonusKm.HasValue && item.CikisKm.HasValue)
                    ? (item.DonusKm.Value - item.CikisKm.Value).ToString()
                    : "Devam Ediyor";
                var donusTarih = item.BitisTarihi.HasValue ? item.BitisTarihi.Value.ToString("dd.MM.yyyy HH:mm") : "-";
                sb.AppendLine($"{item.GorevID};{item.Plaka};{item.Surucu};{item.Aciklama};{item.BaslangicTarihi:dd.MM.yyyy HH:mm};{donusTarih};{item.CikisKm ?? 0};{item.DonusKm?.ToString() ?? "-"};{yapilanKm}");
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return File(bytes, "text/csv; charset=utf-8", $"Filo_Gorev_Raporu_{DateTime.Now:yyyyMMdd}.csv");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGorev(int id)
        {
            var gorev = await _context.Gorevlendirmeler.FindAsync(id);
            if (gorev == null) return NotFound();

            _context.Gorevlendirmeler.Remove(gorev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
