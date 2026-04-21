using Microsoft.AspNetCore.Mvc;
using SillageParfumApi.Application.DTOs;
using SillageParfumApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SillageParfumApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PerfumesController : ControllerBase
    {
        private readonly IPerfumeService _perfumeService;
        private readonly IExternalPerfumeService _externalPerfumeService;

        public PerfumesController(IPerfumeService perfumeService, IExternalPerfumeService externalPerfumeService)
        {
            _perfumeService = perfumeService;
            _externalPerfumeService = externalPerfumeService;
        }

        // GET: api/Perfumes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerfumeResponseDto>>> GetPerfumes()
        {
            var perfumes = await _perfumeService.ObtenerTodosLosPerfumesAsync();
            return Ok(perfumes);
        }

        // GET: api/Perfumes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PerfumeResponseDto>> GetPerfume(int id)
        {
            var perfume = await _perfumeService.ObtenerPerfumePorIdAsync(id);
            if (perfume == null) return NotFound($"No se encontró el perfume con el ID {id}");
            return Ok(perfume);
        }

        // POST: api/Perfumes
        [HttpPost]
        public async Task<ActionResult> PostPerfume(CrearPerfumeDto perfumeDto)
        {
            var createdId = await _perfumeService.CrearPerfumeAsync(perfumeDto);
            return CreatedAtAction(nameof(GetPerfume), new { id = createdId }, perfumeDto);
        }

        // PUT: api/Perfumes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerfume(int id, ActualizarPerfumeDto perfumeDto)
        {
            if (id != perfumeDto.Id)
                return BadRequest("El ID de la URL no coincide con el ID del modelo.");

            var updated = await _perfumeService.ActualizarPerfumeAsync(perfumeDto);
            if (!updated) return NotFound($"No se encontró el perfume con ID {id} para actualizar.");

            return NoContent();
        }

        // DELETE: api/Perfumes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerfume(int id)
        {
            var deleted = await _perfumeService.EliminarPerfumeAsync(id);
            if (!deleted) return NotFound($"No se encontró el perfume con ID {id} para eliminar.");
            return NoContent();
        }

        // GET: api/Perfumes/buscar-externo/{nombre}
        [HttpGet("buscar-externo/{nombre}")]
        public async Task<ActionResult<PerfumeExternoDto>> BuscarPerfumeEnInternet(string nombre)
        {
            try
            {
                var resultado = await _externalPerfumeService.BuscarEnInternetAsync(nombre);
                if (resultado == null)
                    return NotFound($"No se encontró información en internet para '{nombre}'");
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al consultar la API externa: {ex.Message}");
            }
        }
    }
}
