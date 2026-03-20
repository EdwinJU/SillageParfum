using SillageParfumApi.Application.DTOs;

namespace SillageParfumApi.Application.Interfaces
{
    public interface IPerfumeService
    {
        // Fíjate que devolvemos el DTO, no la Entidad de la base de datos
        Task<IEnumerable<PerfumeResponseDto>> ObtenerTodosLosPerfumesAsync();

        // NUEVO: Recibe la caja de entrada y devuelve el ID del perfume creado
        Task<int> CrearPerfumeAsync(CrearPerfumeDto perfumeDto);

        // NUEVO: Método para buscar por ID. Nota el "?" porque podría no existir y devolver null
        Task<PerfumeResponseDto?> ObtenerPerfumePorIdAsync(int id);

        Task<bool> ActualizarPerfumeAsync(ActualizarPerfumeDto dto);
        Task<bool> EliminarPerfumeAsync(int id);
    }
}