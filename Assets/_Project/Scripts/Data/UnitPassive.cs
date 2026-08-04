using UnityEngine;
using TechTest.Combat;

namespace TechTest.Data
{
    public abstract class UnitPassive : ScriptableObject
    {
        // Dipanggil ketika unit ini berhasil mendaratkan serangan ke target
        public virtual void OnAttackLanded(Unit caster, Unit target, int damage) { }
        
        // Bisa diekspansi nanti: OnTurnStart, OnTakeDamage, OnDeath, dll.
    }
}
