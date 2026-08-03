using System;
using UnityEngine;

namespace TechTest.Data
{
    public enum EffectType
    {
        Damage,
        Block,
        Heal,
        DrawCard,
        ApplyVulnerable,
        ApplyWeak
    }

    [Serializable]
    public struct CardEffectDefinition
    {
        public EffectType effectType;
        public int value;
        public int secondaryValue; // Useful for duration of buffs/debuffs
    }
}
