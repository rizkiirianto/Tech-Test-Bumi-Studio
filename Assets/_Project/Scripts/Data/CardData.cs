using UnityEngine;

namespace TechTest.Data
{
    public enum CardType
    {
        Attack,
        Skill,
        Power
    }

    public enum TargetType
    {
        SingleEnemy,
        AllEnemies,
        Self
    }

    [CreateAssetMenu(fileName = "New Card", menuName = "TechTest/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Card Information")]
        public string cardName;
        [TextArea] public string description;
        public Sprite cardIcon;
        public int energyCost;

        [Header("Card Mechanics")]
        public CardType cardType;
        public TargetType targetType;

        [Header("Effects")]
        public System.Collections.Generic.List<CardEffectDefinition> effects = new System.Collections.Generic.List<CardEffectDefinition>();
        
        public int fatigueCost; // Some cards might cost fatigue to play
    }
}
