using System.Threading.Tasks;
using rpg.Dtos.Character;
using rpg.Dtos.CharacterSkill;
using rpg.Models;

namespace rpg.Services.CharacterSkillService
{
    public interface ICharacterSkillService
    {
         Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto addCharacterSkillDto);
    }
}