using AutoMapper;
using rpg.Models;
using rpg.Dtos.Character;
using rpg.Dtos.Weapon;
using rpg.Dtos.Skill;
using System.Linq;

namespace rpg
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>()
                .ForMember(dto => dto.Skills, c => c.MapFrom(c => c.CharacterSkills.Select(cs => cs.Skill))).ReverseMap();
            CreateMap<Character, AddCharacterDto>().ReverseMap();
            CreateMap<Character, UpdateCharacterDto>().ReverseMap();
            CreateMap<Weapon,GetWeaponDto>().ReverseMap();
            CreateMap<Weapon,AddWeaponDto>().ReverseMap();
            CreateMap<Skill,GetSkillDto>().ReverseMap();

            
        }
    }
}