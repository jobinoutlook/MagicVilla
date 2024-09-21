using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class VillaNumberRepository : IVillaNumberRepository
    {
        ApplicationDbContext _db;
        public VillaNumberRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task CreateAsync(VillaNumber entity)
        {
            await _db.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<VillaNumber> GetAsync(Expression<Func<VillaNumber, bool>> filter = null, bool tracked = true)
        {
            IQueryable<VillaNumber> query = _db.VillaNumbers;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<VillaNumber>> GetAllAsync(Expression<Func<VillaNumber, bool>> filter = null)
        {
            

            IEnumerable<VillaNumber> villaNumberList = await _db.VillaNumbers.Include(u => u.Villa).ToListAsync();

            IQueryable<VillaNumber> query = villaNumberList.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return  query.ToList();
        }

        
        public async Task RemoveAsync(VillaNumber entity)
        {
            _db.Remove(entity);
            await SaveAsync();
        }


        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(VillaNumber entity)
        {
            _db.Update(entity);
            await SaveAsync();
        }
    }
}
