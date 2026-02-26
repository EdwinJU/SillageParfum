/*TODO ESTE BLOQUE DE CODIGO SE USABA PARA LA PRIMERA PARTE DE LA CLASE, CUANDO NO HABÍAMOS VISTO AÚN ENTITY FRAMEWORK NI BASES DE DATOS.
 * using Microsoft.AspNetCore.Mvc;
using MiPrimeraApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MiPrimeraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private static List<Tarea> Tareas = new List<Tarea>
        {
            new Tarea { Id = 1, Nombre = "Hacer ejercicio", EstaCompletada = false },
            new Tarea { Id = 2, Nombre = "Estudiar .NET", EstaCompletada = true }
        };

        // GET: api/<TareasController>
        [HttpGet]
        public IEnumerable<Tarea> Get()
        {
            return Tareas;
        }

        // GET: api/tareas/1
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            // Usamos LINQ (Fase 1) para buscar por el ID dentro del objeto
            var tarea = Tareas.FirstOrDefault(t => t.Id == id);

            if (tarea == null)
            {
                return NotFound("Tarea no encontrada");
            }

            return Ok(tarea);
        }

        // POST: api/tareas
        [HttpPost]
        public IActionResult Post([FromBody] Tarea nuevaTarea)
        {
            // Simulamos un autoincrementable de base de datos
            // Buscamos el ID más alto y le sumamos 1
            int nuevoId = Tareas.Count > 0 ? Tareas.Max(t => t.Id) + 1 : 1;

            nuevaTarea.Id = nuevoId;
            Tareas.Add(nuevaTarea);

            // Devolvemos el objeto creado con su nuevo ID
            return CreatedAtAction(nameof(Get), new { id = nuevaTarea.Id }, nuevaTarea);
        }

        // PUT api/<TareasController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Tarea tareaActualizada)
        {
            // 1. Validacion de seguridad: 
            // Aseguramos que el ID de la URL coincida con el ID del cuerpo (si se envió).
            if (id != tareaActualizada.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID de la tarea.");
            }
            // 2. Buscar la tarea existente
            var tarea = Tareas.FirstOrDefault(t => t.Id == id);
            if (tarea == null)
            {
                return NotFound("Tarea no encontrada");
            }
            // 3. Actualizar los datos
            // Como estamos en memoria, asignamos los valores manualmente.
            tarea.Nombre = tareaActualizada.Nombre;
            tarea.EstaCompletada = tareaActualizada.EstaCompletada;
            // 4. Retornar NoContent (204) es estándar para updates exitosos
            return NoContent();
        }

        // DELETE api/<TareasController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var tarea = Tareas.FirstOrDefault(t => t.Id == id);
            if (tarea == null)
            {
                return NotFound("Tarea no encontrada");
            }
            Tareas.Remove(tarea);
            return NoContent();

        }
    }
}
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiPrimeraApi.Models;

namespace MiPrimeraApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        // Variable para la base de datos
        private readonly ApplicationDbContext _context;

        // Constructor con Inyección de Dependencias
        public TareasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/tareas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarea>>> Get()
        {
            return await _context.Tareas.ToListAsync();
        }

        // GET: api/tareas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound("Tarea no encontrada.");
            }

            return Ok(tarea);
        }

        // POST: api/tareas
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Tarea nuevaTarea)
        {
            _context.Tareas.Add(nuevaTarea);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = nuevaTarea.Id }, nuevaTarea);
        }

        // PUT: api/tareas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Tarea tareaActualizada)
        {
            if (id != tareaActualizada.Id)
            {
                return BadRequest("El ID no coincide.");
            }

            _context.Entry(tareaActualizada).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Tareas.AnyAsync(e => e.Id == id))
                {
                    return NotFound("La tarea no existe.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/tareas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound("Tarea no encontrada.");
            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

