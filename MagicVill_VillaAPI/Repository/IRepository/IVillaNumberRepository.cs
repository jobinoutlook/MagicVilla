using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IVillaNumberRepository
    {
        Task<List<VillaNumber>> GetAllAsync(Expression<Func<VillaNumber, bool>> filter = null);
        Task<VillaNumber> GetAsync(Expression<Func<VillaNumber, bool>> filter = null, bool tracked = true);
        Task CreateAsync(VillaNumber entity);
        Task UpdateAsync(VillaNumber entity);
        Task RemoveAsync(VillaNumber entity);
        Task SaveAsync();
    }
}
