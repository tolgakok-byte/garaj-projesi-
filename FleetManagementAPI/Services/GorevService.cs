using Microsoft.EntityFrameworkCore;
using FleetManagementAPI.Models;

namespace FleetManagementAPI.Services
{
    public class GorevService
    {
        private readonly UygulamaDbContext _context;

        public GorevService(UygulamaDbContext context)
        {
            _context = context;
        }

        // Tüm görevlendirmeleri getir
        public async Task<IEnumerable<GorevKayit>> GetAllAsync()
        {
            return await _context.Gorevlendirmeler.ToListAsync();
        }

        // ID'ye göre görevlendirilmeyi getir
        public async Task<GorevKayit?> GetByIdAsync(int id)
        {
            return await _context.Gorevlendirmeler.FindAsync(id);
        }

        // Yeni görevlendirme ekle
        public async Task AddAsync(GorevKayit gorev)
        {
            _context.Gorevlendirmeler.Add(gorev);
            await _context.SaveChangesAsync();
        }

        // Görevlendirmeyi güncelle
        public async Task UpdateAsync(GorevKayit gorev)
        {
            _context.Entry(gorev).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // Görevlendirmeyi sil
        public async Task DeleteAsync(int id)
        {
            var gorev = await _context.Gorevlendirmeler.FindAsync(id);
            if (gorev != null)
            {
                _context.Gorevlendirmeler.Remove(gorev);
                await _context.SaveChangesAsync();
            }
        }
    }
}