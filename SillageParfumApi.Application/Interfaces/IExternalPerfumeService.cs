using SillageParfumApi.Application.DTOs;

namespace SillageParfumApi.Application.Interfaces
{
    public interface IExternalPerfumeService
    {
        // Le pasamos el nombre del perfume y nos devuelve los datos enriquecidos
        Task<PerfumeExternoDto?> BuscarEnInternetAsync(string perfumeName);
    }
}