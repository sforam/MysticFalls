using AutoMapper;
using MysticFalls_VillaAPI.Models;
using MysticFalls_VillaAPI.Models.Dto;

namespace MysticFalls_VillaAPI
{
    public class MappingConfig: Profile
    {
        public MappingConfig() 
        {
            CreateMap<Villa,VillaDTO>();
            CreateMap<VillaDTO,Villa>();
            CreateMap<Villa, VillaCreateDTO>().ReverseMap();
            CreateMap<Villa, VillaUpdateDTO>().ReverseMap();


        }
    }
}
