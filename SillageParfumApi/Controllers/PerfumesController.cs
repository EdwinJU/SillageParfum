using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SillageParfumApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace SillageParfumApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PerfumesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PerfumesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Perfumes
        // Leer todos los perfumes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Perfume>>> GetPerfumes()
        {
            try
            {
                return await _context.Perfumes.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los perfumes: {ex.Message}");
            }
        }

        // GET: api/Perfumes/5
        // Leer un perfume específico por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Perfume>> GetPerfume(int id)
        {
            try
            {
                var perfume = await _context.Perfumes.FindAsync(id);

                if (perfume == null)
                {
                    return NotFound();
                }

                return perfume;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el perfume: {ex.Message}");
            }
        }

        // POST: api/Perfumes
        // Crear un nuevo perfume
        [HttpPost]
        public async Task<ActionResult<Perfume>> PostPerfume(Perfume perfume)
        {
            try
            {
                _context.Perfumes.Add(perfume);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPerfume), new { id = perfume.Id }, perfume);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el perfume: {ex.Message}");
            }
        }

        // PUT: api/Perfumes/5
        // Actualizar un perfume existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerfume(int id, Perfume perfume)
        {
            try
            {
                if (id != perfume.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del modelo.");
                }

                perfume.UpdatedAt = DateTime.UtcNow;
                _context.Entry(perfume).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PerfumeExists(id))
                        return NotFound();
                    else
                        throw;
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el perfume: {ex.Message}");
            }
        }

        // DELETE: api/Perfumes/5
        // Eliminar un perfume
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerfume(int id)
        {
            try
            {
                var perfume = await _context.Perfumes.FindAsync(id);
                if (perfume == null)
                {
                    return NotFound();
                }

                _context.Perfumes.Remove(perfume);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el perfume: {ex.Message}");
            }
        }

        // Método auxiliar para verificar si el perfume existe
        private bool PerfumeExists(int id)
        {
            return _context.Perfumes.Any(e => e.Id == id);
        }
    }
}
