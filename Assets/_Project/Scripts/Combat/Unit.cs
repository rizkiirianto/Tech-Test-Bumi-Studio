using UnityEngine;
using TechTest.Data;

namespace TechTest.Combat
{
    public class Unit : MonoBehaviour
    {
        public UnitData unitData;

        public int currentHP;
        public int currentBlock;
        public int nextTurnIntentDamage; // Nilai serangan untuk turn ini
        public bool isDead => currentHP <= 0;

        [Header("Visual Components (Optional)")]
        public SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Coroutine flashCoroutine;

        public void Initialize(UnitData data)
        {
            unitData = data;
            // currentHP is set by RunManager if it's the player, or maxHP for enemies
            if (!unitData.isPlayer)
            {
                currentHP = unitData.maxHP;
            }
            currentBlock = 0;

            if (spriteRenderer != null && unitData != null)
            {
                if (unitData.unitSprite != null) spriteRenderer.sprite = unitData.unitSprite;
                spriteRenderer.color = unitData.unitColor;
                transform.localScale = unitData.unitScale;
                originalColor = unitData.unitColor;
            }
        }

        public void Flash(bool isFlashing)
        {
            if (spriteRenderer == null) return;

            if (flashCoroutine != null) StopCoroutine(flashCoroutine);

            if (isFlashing)
            {
                flashCoroutine = StartCoroutine(FlashRoutine());
            }
            else
            {
                spriteRenderer.color = originalColor;
            }
        }

        private System.Collections.IEnumerator FlashRoutine()
        {
            while (true)
            {
                spriteRenderer.color = Color.Lerp(originalColor, Color.yellow, Mathf.PingPong(Time.time * 5f, 1f));
                yield return null;
            }
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

        public void Heal(int amount)
        {
            if (amount <= 0) return;
            currentHP += amount;
            if (currentHP > unitData.maxHP) currentHP = unitData.maxHP;
            Debug.Log($"{unitData.unitName} heals for {amount}. HP: {currentHP}");
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
