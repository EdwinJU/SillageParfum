using Microsoft.AspNetCore.Mvc;
using SillageParfumApi.Interfaces;
using SillageParfumApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace SillageParfumApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PerfumesController : ControllerBase
    {
        private readonly IPerfumeRepository _repository;

        public PerfumesController(IPerfumeRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Perfumes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Perfume>>> GetPerfumes()
        {
            try
            {
                var perfumes = await _repository.GetAllAsync();
                return Ok(perfumes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los perfumes: {ex.Message}");
            }
        }

        // GET: api/Perfumes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Perfume>> GetPerfume(int id)
        {
            try
            {
                var perfume = await _repository.GetByIdAsync(id);
                if (perfume == null) return NotFound();
                return perfume;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el perfume: {ex.Message}");
            }
        }

        // POST: api/Perfumes
        [HttpPost]
        public async Task<ActionResult<Perfume>> PostPerfume(Perfume perfume)
        {
            try
            {
                var created = await _repository.CreateAsync(perfume);
                return CreatedAtAction(nameof(GetPerfume), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el perfume: {ex.Message}");
            }
        }

        // PUT: api/Perfumes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerfume(int id, Perfume perfume)
        {
            try
            {
                if (id != perfume.Id)
                    return BadRequest("El ID de la URL no coincide con el ID del modelo.");

                var updated = await _repository.UpdateAsync(perfume);
                if (!updated)
                {
                    if (!_repository.Exists(id)) return NotFound();
                    return StatusCode(500, "Error de concurrencia al actualizar.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el perfume: {ex.Message}");
            }
        }

        // DELETE: api/Perfumes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerfume(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el perfume: {ex.Message}");
            }
        }
    }
}
