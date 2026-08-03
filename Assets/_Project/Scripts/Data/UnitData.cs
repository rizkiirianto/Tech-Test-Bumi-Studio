using UnityEngine;

namespace TechTest.Data
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "TechTest/Unit Data")]
    public class UnitData : ScriptableObject
    {
        [Header("Unit Information")]
        public string unitName;
        public Sprite unitSprite;
        public bool isPlayer;

        [Header("Base Stats")]
        public int maxHP;
        
        [Header("Player Specific Stats")]
        public int maxEnergy = 3;
        public int maxFatigue = 100;
        public int drawPerTurn = 5;
    }
}
