using System.ComponentModel.DataAnnotations;

namespace FleetManagementAPI.Models
{
    public class GorevKayit
    {
        [Key]
        public int GorevID { get; set; }
        public int AracID { get; set; }
        public int SurucuID { get; set; }
        public string Aciklama { get; set; } = string.Empty;
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }

        // Yeni Ticari Alanlar
        public int? CikisKm { get; set; }
        public int? DonusKm { get; set; }
    }
}
