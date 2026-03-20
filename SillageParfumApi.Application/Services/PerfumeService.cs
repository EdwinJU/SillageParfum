using SillageParfumApi.Application.DTOs;
using SillageParfumApi.Application.Interfaces;
using SillageParfumApi.Domain.Interfaces; // Para usar IPerfumeRepository

namespace SillageParfumApi.Application.Services
{
    public class PerfumeService : IPerfumeService
    {
        private readonly IPerfumeRepository _perfumeRepository;

        // Inyectamos el Obrero (Repositorio) para que el Gerente (Servicio) lo pueda mandar
        public PerfumeService(IPerfumeRepository perfumeRepository)
        {
            _perfumeRepository = perfumeRepository;
        }

        public async Task<IEnumerable<PerfumeResponseDto>> ObtenerTodosLosPerfumesAsync()
        {
            // 1. Vamos a la bodega a traer las Entidades Ricas
            var perfumesDeBD = await _perfumeRepository.GetAllAsync();

            // 2. Transformamos (Mapeamos) las Entidades a DTOs para enviarlos de forma segura
            var perfumesDto = perfumesDeBD.Select(p => new PerfumeResponseDto
            {
                Id = p.Id,
                SKU = p.SKU,
                Name = p.Name,
                Brand = p.Brand,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                IsAvailable = p.IsAvailable,
                ImageUrl = p.ImageUrl
            }).ToList();

            // 3. Devolvemos la lista limpia
            return perfumesDto;
        }

        public async Task<int> CrearPerfumeAsync(CrearPerfumeDto dto)
        {
            // 1. Instanciamos la Entidad Rica. 
            // ¡Magia de Clean Architecture! Si el precio es 0 o falta el nombre, 
            // la Entidad lanzará un ArgumentException y detendrá todo automáticamente.
            var nuevoPerfume = new Domain.Entities.Perfume(
                dto.SKU,
                dto.Name,
                dto.Brand,
                dto.Price,
                dto.VolumeMl
            );

            // 2. Le pasamos la entidad válida al obrero (Repositorio) para que la guarde
            var perfumeCreado = await _perfumeRepository.CreateAsync(nuevoPerfume);

            // 3. Devolvemos el ID que le asignó SQL Server
            return perfumeCreado.Id;
        }
        public async Task<PerfumeResponseDto?> ObtenerPerfumePorIdAsync(int id)
        {
            // 1. Buscamos la Entidad en la bodega usando tu Repositorio
            var perfumeDeBD = await _perfumeRepository.GetByIdAsync(id);

            // 2. Si no existe, devolvemos nulo para que el controlador maneje el error 404
            if (perfumeDeBD == null) return null;

            // 3. Si existe, lo empaquetamos en su DTO seguro
            return new PerfumeResponseDto
            {
                Id = perfumeDeBD.Id,
                SKU = perfumeDeBD.SKU,
                Name = perfumeDeBD.Name,
                Brand = perfumeDeBD.Brand,
                Price = perfumeDeBD.Price,
                DiscountPrice = perfumeDeBD.DiscountPrice,
                IsAvailable = perfumeDeBD.IsAvailable,
                ImageUrl = perfumeDeBD.ImageUrl
            };
        }
        public async Task<bool> ActualizarPerfumeAsync(ActualizarPerfumeDto dto)
        {
            // 1. Buscamos si el perfume existe en la base de datos
            var perfumeExistente = await _perfumeRepository.GetByIdAsync(dto.Id);
            if (perfumeExistente == null) return false;

            // 2. Usamos el método seguro de tu Entidad Rica para cambiar los datos
            perfumeExistente.ActualizarDatos(dto.SKU, dto.Name, dto.Brand, dto.Price, dto.VolumeMl);

            // 3. Le decimos al obrero (Repositorio) que guarde los cambios
            return await _perfumeRepository.UpdateAsync(perfumeExistente);
        }

        public async Task<bool> EliminarPerfumeAsync(int id)
        {
            // El Repositorio ya tiene la lógica de eliminación lista desde ayer
            return await _perfumeRepository.DeleteAsync(id);
        }
    }
}