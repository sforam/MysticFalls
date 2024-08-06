using MysticFalls_VillaAPI.Models;

namespace MysticFalls_VillaAPI.Repository.IRepository
{
    public interface IVillaNumberRepository:IRepository<VillaNumber>

    {
        Task<VillaNumber> UpdateAsync(VillaNumber entity);
    }
}
