using Microsoft.EntityFrameworkCore;
using MysticFalls_VillaAPI.Data;
using MysticFalls_VillaAPI.Models;
using MysticFalls_VillaAPI.Repository.IRepository;
using System.Linq;
using System.Linq.Expressions;

namespace MysticFalls_VillaAPI.Repository
{
    public class VillaRepository :Repository<Villa>, IVillaRepository
    {


        private readonly ApplicationDbContext _db;
        public VillaRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }

        public async Task<Villa> UpdateAsync(Villa entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Villas.Update(entity);
            await _db.SaveChangesAsync();
            return entity;

        }
    }
}
