using AutoMapper;
using MysticFalls.Models.Dto;

namespace MysticFalls
{
    public class MappingConfig: Profile
    {
        public MappingConfig() 
        {
            CreateMap<VillaDTO,VillaCreateDTO>().ReverseMap();

            CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();

            CreateMap<VillaNumberDTO, VillaCreateDTO>().ReverseMap();
            CreateMap<VillaNumberDTO, VillaUpdateDTO>().ReverseMap();



           

        }
    }
}
