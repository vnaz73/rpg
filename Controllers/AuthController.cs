using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rpg.Data;
using rpg.Dtos;
using rpg.Models;

namespace rpg.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;

        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            ServiceResponse<int> res = await _repo.Register(new Models.User {
                Username = dto.Username}, dto.Password);

            if(res.Success == false)
            {
                return BadRequest(res);
            }    

            return Ok(res);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            ServiceResponse<string> res = await _repo.Login(dto.Username, dto.Password);

            if(res.Success == false)
            {
                return BadRequest(res);
            }    

            return Ok(res);
        }
    }
}