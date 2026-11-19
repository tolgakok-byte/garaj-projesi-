namespace FleetManagementAPI.Models
{
    public class Surucu
    {
        public int SurucuID { get; set; } // SQL'deki PK ismi ile aynı
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string? Telefon { get; set; }
        public string? EhliyetSinifi { get; set; }
        public DateTime? KayitTarihi { get; set; }
    }
}