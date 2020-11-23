using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.Dtos.Character;
using rpg.Models;

namespace rpg.Services.CharacterServices
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;

        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            Character character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();

            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            serviceResponse.Data = await _context.Characters.Where(u => u.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            List<Character> dbCharacters = await _context.Characters.Where(c => c.User.Id == GetUserId()).ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList(); ;
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();
            Character dbCharacter = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.CharacterSkills).ThenInclude(sc => sc.Skill)
                .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {

            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _context.Characters
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == updateCharacter.Id);
                if(character.User.Id == GetUserId())
                {    
                    character.Class = updateCharacter.Class;
                    character.Defense = updateCharacter.Defense;
                    character.HitPoints = updateCharacter.HitPoints;
                    character.Intelligence = updateCharacter.Intelligence;
                    character.Name = updateCharacter.Name;
                    character.Strength = updateCharacter.Strength;

                    _context.Characters.Update(character);
                    await _context.SaveChangesAsync();

                    serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacterById(int id)
        {
            ServiceResponse<List<GetCharacterDto>> serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                Character character = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());

                if(character == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found";
                    return serviceResponse;
                }

                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _context.Characters
                .Where(c => c.User.Id == GetUserId())
                .Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}