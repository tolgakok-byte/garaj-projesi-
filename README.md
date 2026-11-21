# garaj-projesi-
# 🚗 Garaj ve Filo Yönetim Sistemi (FleetManagementAPI)

.NET 8 ve SQLite ile geliştirilmiş rol bazlı garaj, araç ve sürücü yönetim uygulamasıdır.

---

## ✨ Temel Özellikler
* **Rol Bazlı Yetkilendirme (`FiloAuth`):** Admin ve Şoför rolleri için ayrıştırılmış yetkiler.
* **Araç ve Sürücü Yönetimi:** Filo ve sürücü durumlarının tek panelden takibi.
* **SQLite Altyapısı:** Kurulum gerektirmeyen yerel veritabanı.
* **Web Arayüzü:** `wwwroot/index.html` üzerinden çalışan ön yüz.

---

## 🔐 Test Giriş Bilgileri

| Rol | Kullanıcı Adı | Şifre | Yetki Kapsamı |
| :--- | :--- | :--- | :--- |
| **Admin** | `[admin]` | `[1234]` | Tüm sistem ayarları ve araç/sürücü yönetimi |
| **Şoför** | `[sofor]` | `[5678]` | Sürücü paneli ve atanan görevler |

---

## 🚀 Kurulum ve Çalıştırma

1. **Repoyu klonlayın:** `git clone https://github.com`
2. **Dizinize gidin:** `cd garaj-projesi-/FleetManagementAPI`
3. **Başlatın:** `dotnet run`
4. **Tarayıcıdan açın:** [http://localhost:5055](http://localhost:5055)

---

## 🛠 Kullanılan Teknolojiler
* **Backend:** C#, .NET 8 (ASP.NET Core Web API), Cookie Authentication (`FiloAuth`), SQLite
* **Frontend:** HTML5, CSS3, JavaScript
