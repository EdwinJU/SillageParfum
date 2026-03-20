using SillageParfumApi.Domain.Entities;

namespace SillageParfumApi.Domain.Interfaces
{
    public interface IPerfumeRepository
    {
        Task<IEnumerable<Perfume>> GetAllAsync();
        Task<Perfume?> GetByIdAsync(int id);
        Task<Perfume> CreateAsync(Perfume perfume);
        Task<bool> UpdateAsync(Perfume perfume);
        Task<bool> DeleteAsync(int id);
        bool Exists(int id);
    }
}