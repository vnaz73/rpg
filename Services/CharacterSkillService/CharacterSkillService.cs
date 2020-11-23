using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.Dtos.Character;
using rpg.Dtos.CharacterSkill;
using rpg.Models;

namespace rpg.Services.CharacterSkillService
{
    public class CharacterSkillService : ICharacterSkillService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataContext _dataContext;
        public CharacterSkillService(IMapper mapper,
             IHttpContextAccessor httpContextAccessor, 
            DataContext dataContext)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;

        }
        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto addCharacterSkillDto)
        {
            ServiceResponse<GetCharacterDto> res = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _dataContext.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills).ThenInclude(sc => sc.Skill)
                    .FirstOrDefaultAsync( c => c.Id == addCharacterSkillDto.CharacterId &&
                     c.User.Id == int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
                
                if(character == null)
                {
                    res.Success = false;
                    res.Message = "Character not found";
                    return res;
                }

                Skill skill = await _dataContext.Skills
                    .FirstOrDefaultAsync(s => s.Id == addCharacterSkillDto.SkillId);

                  if(skill == null)
                {
                    res.Success = false;
                    res.Message = "Skill not found";
                    return res;
                } 

                CharacterSkill characterSkill = new  CharacterSkill()
                {
                    SkillId = addCharacterSkillDto.SkillId,
                    CharacterId = addCharacterSkillDto.CharacterId
                } ;

                await _dataContext.CharacterSkills.AddAsync(characterSkill);
                await _dataContext.SaveChangesAsync();

                res.Data = _mapper.Map<GetCharacterDto>(character);
                return res;
            }


            catch(Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
            }
            return res;
        }
    }
}