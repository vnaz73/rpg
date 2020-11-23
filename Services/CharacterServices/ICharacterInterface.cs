using rpg.Dtos.Character;
using rpg.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rpg.Services.CharacterServices
{
    public interface ICharacterService
    {
         Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters();
         Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id);
         Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);
         Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter);
         Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacterById(int id);
    }
}