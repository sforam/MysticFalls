using MysticFalls_VillaAPI.Models;
using System.Linq.Expressions;

namespace MysticFalls_VillaAPI.Repository.IRepository
{
    public interface IVillaRepository:IRepository<Villa>
    {
     
        Task <Villa> UpdateAsync(Villa entity);

        
    }
}
