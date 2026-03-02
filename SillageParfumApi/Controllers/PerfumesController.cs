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
            return await _context.Perfumes.ToListAsync();
        }

        // GET: api/Perfumes/5
        // Leer un perfume específico por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Perfume>> GetPerfume(int id)
        {
            var perfume = await _context.Perfumes.FindAsync(id);

            if (perfume == null)
            {
                return NotFound(); // Devuelve 404 si no existe
            }

            return perfume;
        }

        // POST: api/Perfumes
        // Crear un nuevo perfume
        [HttpPost]
        public async Task<ActionResult<Perfume>> PostPerfume(Perfume perfume)
        {
            _context.Perfumes.Add(perfume);
            await _context.SaveChangesAsync();

            // Devuelve un código 201 (Created) y la ruta para consultar el nuevo recurso
            return CreatedAtAction(nameof(GetPerfume), new { id = perfume.Id }, perfume);
        }

        // PUT: api/Perfumes/5
        // Actualizar un perfume existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerfume(int id, Perfume perfume)
        {
            if (id != perfume.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del modelo.");
            }

            perfume.UpdatedAt = DateTime.UtcNow; // Actualizamos la fecha de modificación
            _context.Entry(perfume).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerfumeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Devuelve 204 (Éxito, sin contenido extra que devolver)
        }

        // DELETE: api/Perfumes/5
        // Eliminar un perfume
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerfume(int id)
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

        // Método auxiliar para verificar si el perfume existe
        private bool PerfumeExists(int id)
        {
            return _context.Perfumes.Any(e => e.Id == id);
        }
    }
}
