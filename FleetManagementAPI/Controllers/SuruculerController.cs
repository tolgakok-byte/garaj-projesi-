using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FleetManagementAPI.Models;

namespace FleetManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuruculerController : ControllerBase
    {
        private readonly UygulamaDbContext _context;

        public SuruculerController(UygulamaDbContext context)
        {
            _context = context;
        }

        // GET: api/Suruculer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Surucu>>> GetSuruculer()
        {
            return await _context.Suruculer.ToListAsync();
        }

        // GET: api/Suruculer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Surucu>> GetSurucu(int id)
        {
            var surucu = await _context.Suruculer.FindAsync(id);
            if (surucu == null) return NotFound();
            return surucu;
        }

        // POST: api/Suruculer
        [HttpPost]
        public async Task<ActionResult<Surucu>> PostSurucu(Surucu surucu)
        {
            surucu.KayitTarihi = DateTime.Now;
            _context.Suruculer.Add(surucu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSurucu), new { id = surucu.SurucuID }, surucu);
        }

        // PUT: api/Suruculer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSurucu(int id, Surucu surucu)
        {
            if (id != surucu.SurucuID) return BadRequest();

            _context.Entry(surucu).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Suruculer.Any(e => e.SurucuID == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/Suruculer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurucu(int id)
        {
            var surucu = await _context.Suruculer.FindAsync(id);
            if (surucu == null) return NotFound();

            _context.Suruculer.Remove(surucu);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}