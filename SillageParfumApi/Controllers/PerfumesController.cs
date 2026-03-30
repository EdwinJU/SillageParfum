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
        // El Cajero ahora se comunica con el Gerente, no con la Bodega
        private readonly IPerfumeService _perfumeService;
        // Inyectamos el servicio externo para enriquecer los datos de los perfumes
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
            try
            {
                // Usamos el servicio y devolvemos la lista de DTOs
                var perfumes = await _perfumeService.ObtenerTodosLosPerfumesAsync();
                return Ok(perfumes);
            }
            catch (Exception ex)
            {
                // Conservamos tu manejo de errores intacto
                return StatusCode(500, $"Error al obtener los perfumes: {ex.Message}");
            }
        }
        // GET: api/Perfumes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PerfumeResponseDto>> GetPerfume(int id)
        {
            try
            {
                // El Gerente hace el trabajo de buscar y mapear a DTO
                var perfume = await _perfumeService.ObtenerPerfumePorIdAsync(id);

                // Si no lo encuentra, devolvemos un bonito 404
                if (perfume == null) return NotFound($"No se encontró el perfume con el ID {id}");

                return Ok(perfume);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el perfume: {ex.Message}");
            }
        }
        // POST: api/Perfumes
        [HttpPost]
        public async Task<ActionResult> PostPerfume(CrearPerfumeDto perfumeDto)
        {
            try
            {
                // El servicio intenta crear el perfume
                var createdId = await _perfumeService.CrearPerfumeAsync(perfumeDto);

                // 2. Devolvemos un 201 genérico con el ID nuevo
                //return StatusCode(201, new { mensaje = "Perfume creado exitosamente", id = createdId });

                // Ahora sí usamos CreatedAtAction porque el método GetPerfume ya existe de nuevo.
                // Esto generará un código 201 y un encabezado 'Location' en la respuesta HTTP.
                return CreatedAtAction(nameof(GetPerfume), new { id = createdId }, perfumeDto);
            }
            catch (ArgumentException argEx)
            {
                // Si la Entidad Rica rechaza los datos (ej. precio negativo), 
                // le devolvemos un 400 Bad Request bonito a React.
                return BadRequest($"Datos inválidos: {argEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el perfume: {ex.Message}");
            }
        }

        // PUT: api/Perfumes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerfume(int id, ActualizarPerfumeDto perfumeDto)
        {
            try
            {
                if (id != perfumeDto.Id)
                    return BadRequest("El ID de la URL no coincide con el ID del modelo.");

                var updated = await _perfumeService.ActualizarPerfumeAsync(perfumeDto);
                if (!updated)
                {
                    return NotFound($"No se encontró el perfume con ID {id} para actualizar.");
                }
                
                return NoContent(); // 204 No Content (Éxito, pero no devuelve nada)
            }
            catch (ArgumentException argEx)
            {
                // Si intentan poner un precio negativo, la Entidad lo rechaza aquí
                return BadRequest($"Datos inválidos: {argEx.Message}");
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
                var deleted = await _perfumeService.EliminarPerfumeAsync(id);
                if (!deleted) return NotFound($"No se encontró el perfume con ID {id} para eliminar.");
                
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el perfume: {ex.Message}");
            }
        }
        // NUEVO ENDPOINT: Buscar en internet (RapidAPI)
        // =======================================================
        [HttpGet("buscar-externo/{nombre}")]
        public async Task<ActionResult<PerfumeExternoDto>> BuscarPerfumeEnInternet(string nombre)
        {
            try
            {
                // El Cajero le pide al nuevo obrero que busque en internet
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


    