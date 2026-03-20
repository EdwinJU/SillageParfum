using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SillageParfumApi.Domain.Entities;

namespace SillageParfumApi.Infrastructure.Data;

// Heredamos de DbContext (la clase base de EF Core)
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    // El constructor recibe las opciones (como la cadena de conexión)
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Aquí le decimos que cree una tabla llamada "Perfumes" basada en la clase "Perfume"
    public DbSet<Perfume> Perfumes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Perfume>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Perfume>()
            .Property(p => p.DiscountPrice)
            .HasPrecision(18, 2);
    }
}