using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IVillaNumberRepository
    {
        Task<List<VillaNumber>> GetAllAsync(string? includeProperties = null);
        Task<List<VillaNumber>> GetAllAsync(Expression<Func<VillaNumber, bool>> filter = null, string? includeProperties = null,
            int pageSize = 10, int pageNumber = 1);
        Task<VillaNumber> GetAsync(Expression<Func<VillaNumber, bool>> filter = null, bool tracked = true, string? includeProperties = null);
        Task CreateAsync(VillaNumber entity);
        Task UpdateAsync(VillaNumber entity);
        Task RemoveAsync(VillaNumber entity);
        Task SaveAsync();
    }
}
