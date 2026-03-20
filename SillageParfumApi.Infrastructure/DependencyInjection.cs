using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SillageParfumApi.Domain.Interfaces;
using SillageParfumApi.Infrastructure.Data;
using SillageParfumApi.Infrastructure.Repositories;

namespace SillageParfumApi.Infrastructure
{
    public static class DependencyInjection
    {
        // Este es el método mágico que extenderá a IServiceCollection
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Configuración de Base de Datos
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // 2. Configuración de Identity atado a Entity Framework
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // 3. Inyección de tus Repositorios
            services.AddScoped<IPerfumeRepository, PerfumeRepository>();

            return services;
        }
    }
}