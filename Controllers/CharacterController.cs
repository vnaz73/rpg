using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg.Dtos.Character;
using rpg.Models;
using rpg.Services.CharacterServices;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;

        }
        
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _characterService.GetAllCharacters());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(int id)
        {

            return Ok(await _characterService.GetCharacterById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddCharacter(AddCharacterDto newCharacter)
        {
            
            return Ok(await _characterService.AddCharacter(newCharacter));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            ServiceResponse<GetCharacterDto> res = await _characterService.UpdateCharacter(updateCharacter);
            if(res.Success == false)
            {
                return NotFound(res);
            }
            return Ok(res);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {

            ServiceResponse<List<GetCharacterDto>> res = await _characterService.DeleteCharacterById(id);
            if(res.Success == false)
            {
                return NotFound(res);
            }
            return Ok(res);
        }
    }
}
