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

        public async Task<VillaNumber> GetAsync(Expression<Func<VillaNumber, bool>> filter = null, bool tracked = true, string? includeProperties = null)
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

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<VillaNumber>> GetAllAsync(string? includeProperties = null)
        {
            IQueryable<VillaNumber> query = _db.VillaNumbers;
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<List<VillaNumber>> GetAllAsync(Expression<Func<VillaNumber, bool>> filter = null, string? includeProperties = null,
             int pageSize = 10, int pageNumber = 1)
        {
            

            IEnumerable<VillaNumber> villaNumberList = await _db.VillaNumbers.Include(u => u.Villa).ToListAsync();

            IQueryable<VillaNumber> query = villaNumberList.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            if (pageSize > 0)
            {
                if (pageSize > 100)
                {
                    pageSize = 100;
                }
                //skip0.take(5)
                //page number- 2     || page size -5
                //skip(5*(1)) take(5)
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
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
