using UnityEngine;
using TechTest.Combat;
using TechTest.Core;

namespace TechTest.Data
{
    [CreateAssetMenu(fileName = "NightmareAura", menuName = "TechTest/Passives/Nightmare Aura")]
    public class NightmareAura : UnitPassive
    {
        public int fatigueAmount = 10;

        public override void OnAttackLanded(Unit caster, Unit target, int damage)
        {
            if (target.unitData.isPlayer)
            {
                Debug.Log($"NIGHTMARE AURA! Boss inflicts {fatigueAmount} Fatigue to the player!");
                if (RunManager.Instance != null)
                {
                    RunManager.Instance.AddFatigue(fatigueAmount);
                }
            }
        }
    }
}
