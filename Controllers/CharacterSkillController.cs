using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg.Dtos.CharacterSkill;
using rpg.Services.CharacterSkillService;
//using rpg.Services.CharacterSkillService;

namespace rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CharacterSkillController : ControllerBase
    {
        private readonly ICharacterSkillService _characterSkillService;
        public CharacterSkillController(ICharacterSkillService characterSkillService)
        {
            _characterSkillService = characterSkillService;

        }
        [HttpPost]
        public async Task<IActionResult> AddAcharacterSkill(AddCharacterSkillDto addCharacterSkillDto)
        {
            return Ok(await _characterSkillService.AddCharacterSkill(addCharacterSkillDto));
        }

    }
}