using System.Threading.Tasks;
using rpg.Dtos.Fight;
using rpg.Models;

namespace rpg.Services.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttackDto);
        Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto weaponAttackDto);
        Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequestDto);

    }
}