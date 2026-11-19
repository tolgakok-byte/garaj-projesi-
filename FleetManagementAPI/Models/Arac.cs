namespace FleetManagementAPI.Models
{
    public class Arac
    {
        public int AracID { get; set; }
        public string Plaka { get; set; } = string.Empty;
        public string Marka { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? Yil { get; set; }
        public int? Kilometre { get; set; }
        public string? Durum { get; set; } // Aktif, Bakımda vb.
        public DateTime? KayitTarihi { get; set; } = DateTime.Now;

        // Yeni Ticari Alanlar
        public DateTime? MuayeneTarihi { get; set; }
        public DateTime? SigortaTarihi { get; set; }
    }
}
