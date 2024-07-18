using Microsoft.AspNetCore.Mvc;
using MysticFalls_VillaAPI.Data;
using MysticFalls_VillaAPI.Models;
using MysticFalls_VillaAPI.Models.Dto;

namespace MysticFalls_VillaAPI.Controllers
{

   /* [Route("api/[controller]")]*/
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController:ControllerBase
    {
        [HttpGet]

        public IEnumerable<VillaDTO> GetVillas() 
        {
            return VillaStore.villaList;
        }
        [HttpGet("{id:int}")]
        public VillaDTO GetVillas(int id)
        {
            return VillaStore.villaList.FirstOrDefault(u=>u.Id==id);

        }
    }
}
