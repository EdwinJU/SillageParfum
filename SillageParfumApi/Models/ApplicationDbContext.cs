using Microsoft.EntityFrameworkCore;

namespace SillageParfumApi.Models
{
    // Heredamos de DbContext (la clase base de EF Core)
    public class ApplicationDbContext : DbContext
    {
        // El constructor recibe las opciones (como la cadena de conexión)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Un DbSet representa una Tabla en la base de datos.
        // Aquí le decimos que cree una tabla llamada "Tareas" basada en la clase "Tarea"
        public DbSet<Tarea> Tareas { get; set; }
    }
}