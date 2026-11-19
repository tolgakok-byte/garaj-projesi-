using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FleetManagementAPI.Models;

namespace FleetManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AraclarController : ControllerBase
    {
        private readonly UygulamaDbContext _context;

        public AraclarController(UygulamaDbContext context)
        {
            _context = context;
        }

        // GET: api/Araclar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Arac>>> GetAraclar()
        {
            return await _context.Araclar.ToListAsync();
        }

        // GET: api/Araclar/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Arac>> GetArac(int id)
        {
            var arac = await _context.Araclar.FindAsync(id);
            if (arac == null) return NotFound();
            return arac;
        }

        // POST: api/Araclar
        [HttpPost]
        public async Task<ActionResult<Arac>> PostArac(Arac arac)
        {
            arac.KayitTarihi = DateTime.Now;
            _context.Araclar.Add(arac);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArac), new { id = arac.AracID }, arac);
        }

        // PUT: api/Araclar/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArac(int id, Arac arac)
        {
            if (id != arac.AracID) return BadRequest();

            _context.Entry(arac).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Araclar.Any(e => e.AracID == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/Araclar/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArac(int id)
        {
            var arac = await _context.Araclar.FindAsync(id);
            if (arac == null) return NotFound();

            _context.Araclar.Remove(arac);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}