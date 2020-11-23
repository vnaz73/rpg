using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.Dtos.Character;
using rpg.Dtos.Weapon;
using rpg.Models;

namespace rpg.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        public WeaponService(DataContext context, IHttpContextAccessor httpContext, IMapper mapper)
        {
            _mapper = mapper;
            _httpContext = httpContext;
            _context = context;

        }
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto weaponDto)
        {
            ServiceResponse<GetCharacterDto>  res = new  ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == weaponDto.CharacterId &&
                    c.Id == int.Parse(_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));

                    if(character == null)
                    {
                        res.Success = false;
                        res.Message = "Character not found";
                    }
                
                Weapon weapon = new Weapon
                {
                    Character = character,
                    Damage = weaponDto.Damage,
                    Name = weaponDto.Name
                };

                await _context.Weapons.AddAsync(weapon);
                await _context.SaveChangesAsync();

                res.Data = _mapper.Map<GetCharacterDto>(character);
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