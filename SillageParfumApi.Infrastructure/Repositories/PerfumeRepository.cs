using Microsoft.EntityFrameworkCore;
using SillageParfumApi.Domain.Entities;
using SillageParfumApi.Domain.Interfaces;
using SillageParfumApi.Infrastructure.Data; // Aquí está tu ApplicationDbContext ahora

namespace SillageParfumApi.Infrastructure.Repositories
{
    public class PerfumeRepository : IPerfumeRepository
    {
        private readonly ApplicationDbContext _context;

        public PerfumeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Perfume>> GetAllAsync()
        {
            return await _context.Perfumes.ToListAsync();
        }

        public async Task<Perfume?> GetByIdAsync(int id)
        {
            return await _context.Perfumes.FindAsync(id);
        }

        public async Task<Perfume> CreateAsync(Perfume perfume)
        {
            _context.Perfumes.Add(perfume);
            await _context.SaveChangesAsync();
            return perfume;
        }

        public async Task<bool> UpdateAsync(Perfume perfume)
        {
            // Nota: Aquí quitamos el perfume.UpdatedAt = DateTime.UtcNow; 
            // porque esa responsabilidad ahora es de la Entidad Rica (Domain)

            _context.Entry(perfume).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var perfume = await _context.Perfumes.FindAsync(id);
            if (perfume == null) return false;

            _context.Perfumes.Remove(perfume);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Exists(int id)
        {
            return _context.Perfumes.Any(e => e.Id == id);
        }
    }
}