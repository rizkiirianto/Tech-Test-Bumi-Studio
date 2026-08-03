using UnityEngine;
using TechTest.Data;

namespace TechTest.Combat
{
    public class Unit : MonoBehaviour
    {
        public UnitData unitData;

        public int currentHP { get; private set; }
        public int currentBlock { get; private set; }

        public void Initialize(UnitData data)
        {
            unitData = data;
            currentHP = data.maxHP;
            currentBlock = 0;
        }

        public void SetHP(int hp)
        {
            currentHP = hp;
            if (currentHP > unitData.maxHP) currentHP = unitData.maxHP;
        }

        public void TakeDamage(int damage)
        {
            if (currentBlock > 0)
            {
                if (currentBlock >= damage)
                {
                    currentBlock -= damage;
                    damage = 0;
                }
                else
                {
                    damage -= currentBlock;
                    currentBlock = 0;
                }
            }

            if (damage > 0)
            {
                currentHP -= damage;
                if (currentHP < 0) currentHP = 0;
                Debug.Log($"{unitData.unitName} takes {damage} damage! HP remaining: {currentHP}");
            }
            else
            {
                Debug.Log($"{unitData.unitName} blocks the attack!");
            }
        }

        public void AddBlock(int amount)
        {
            currentBlock += amount;
            Debug.Log($"{unitData.unitName} gains {amount} block. Total Block: {currentBlock}");
        }

        public void ClearBlock()
        {
            currentBlock = 0;
        }

        public bool IsDead()
        {
            return currentHP <= 0;
        }
    }
}
