using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.Dtos.Fight;
using rpg.Models;

namespace rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        public FightService(DataContext context)
        {
            _context = context;

        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequestDto)
        {
           ServiceResponse<FightResultDto> res = new ServiceResponse<FightResultDto>
           {
               Data = new FightResultDto()
           };
           try{
               List<Character> characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .Where(c => fightRequestDto.CharacterId.Contains(c.Id))
                    .ToListAsync();

            
           }
           catch(Exception ex)
           {
               res.Success = false;
               res.Message = ex.Message;
           }
           return res;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto skillAttackDto)
        {
            ServiceResponse<AttackResultDto> res = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = _context.Characters
                                        .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                                        .FirstOrDefault(c => c.Id == skillAttackDto.AttackerId);
                Character opponent = _context.Characters

                                        .FirstOrDefault(c => c.Id == skillAttackDto.OpponentId);

                CharacterSkill characterSkill = attacker.CharacterSkills
                            .FirstOrDefault(cs => cs.SkillId == skillAttackDto.SkillId);

                if (characterSkill == null)
                {
                    res.Success = false;
                    res.Message = $"{attacker.Name} doesn't have skill";
                    return res;
                }
                int damage = DoSkillAttack(attacker, opponent, characterSkill);
                if (opponent.HitPoints <= 0)
                {
                    res.Message = $"{opponent.Name} has been defeated";
                }

                _context.Characters.Update(opponent);
                await _context.SaveChangesAsync();

                AttackResultDto r = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
                res.Data = r;
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
            }
            return res;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, CharacterSkill characterSkill)
        {
            int damage = characterSkill.Skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= (new Random().Next(opponent.Defense));

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttackDto)
        {
            ServiceResponse<AttackResultDto> res = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = _context.Characters
                                        .Include(c => c.Weapon)
                                        .FirstOrDefault(c => c.Id == weaponAttackDto.AttackerId);
                Character opponent = _context.Characters

                                        .FirstOrDefault(c => c.Id == weaponAttackDto.OpponentId);

                int damage = DoWeaponAttack(attacker, opponent);
                if (opponent.HitPoints <= 0)
                {
                    res.Message = $"{opponent.Name} has been defeated";
                }

                _context.Characters.Update(opponent);
                await _context.SaveChangesAsync();

                AttackResultDto r = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
                res.Data = r;
            }
            catch (Exception ex)
            {
                res.Success = false;
                res.Message = ex.Message;
            }
            return res;
        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= (new Random().Next(opponent.Defense));

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }
    }
}