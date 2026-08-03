using UnityEngine;
using System.Collections;
using TechTest.Data;

namespace TechTest.Combat
{
    public enum BattleState
    {
        Start,
        PlayerTurn,
        EnemyTurn,
        Won,
        Lost
    }

    public class BattleManager : MonoBehaviour
    {
        public BattleState state;

        public Unit playerUnit;
        public Unit enemyUnit;

        public DeckManager deckManager;

        [Header("Energy System")]
        public int currentEnergy;
        public int maxEnergy;

        // Simple Enemy Intent (for prototype)
        private int upcomingEnemyDamage = 5;

        public void StartBattle(UnitData pData, UnitData eData)
        {
            state = BattleState.Start;
            
            // Initialize Units
            playerUnit.Initialize(pData);
            enemyUnit.Initialize(eData);

            maxEnergy = pData.maxEnergy;

            // Setup Deck
            deckManager.InitializeBattle();

            StartCoroutine(PlayerTurn());
        }

        private IEnumerator PlayerTurn()
        {
            state = BattleState.PlayerTurn;
            
            // Clear block from previous turn
            playerUnit.ClearBlock();
            
            // Reset energy
            currentEnergy = maxEnergy;

            // Determine Enemy Intent
            DetermineEnemyIntent();
            
            // Draw initial cards
            deckManager.DrawCards(playerUnit.unitData.drawPerTurn);

            Debug.Log("Player Turn Started. Energy: " + currentEnergy);
            // Wait for player to play cards or press End Turn
            yield return null; 
        }

        public void OnEndTurnButton()
        {
            if (state != BattleState.PlayerTurn) return;

            deckManager.DiscardHand();
            StartCoroutine(EnemyTurn());
        }

        private IEnumerator EnemyTurn()
        {
            state = BattleState.EnemyTurn;
            Debug.Log("Enemy Turn Started.");
            
            // Clear enemy block from previous turn (if they had any)
            enemyUnit.ClearBlock();

            yield return new WaitForSeconds(1f); // Fake thinking time

            // Enemy executes intent
            Debug.Log($"{enemyUnit.unitData.unitName} attacks for {upcomingEnemyDamage} damage!");
            playerUnit.TakeDamage(upcomingEnemyDamage);

            yield return new WaitForSeconds(1f);

            if (playerUnit.IsDead())
            {
                state = BattleState.Lost;
                Debug.Log("Game Over! Player died.");
                TechTest.Core.RunManager.Instance.EndBattle(false, 0);
            }
            else
            {
                StartCoroutine(PlayerTurn());
            }
        }

        private void DetermineEnemyIntent()
        {
            // Simplest intent for prototype: always attack for a random amount between 4 and 8
            upcomingEnemyDamage = Random.Range(4, 9);
            Debug.Log($"Enemy Intent: Planning to attack for {upcomingEnemyDamage} damage.");
        }

        public bool CanPlayCard(CardData card)
        {
            return currentEnergy >= card.energyCost;
        }

        public void PlayCard(CardData card, Unit target)
        {
            if (!CanPlayCard(card) || state != BattleState.PlayerTurn)
            {
                Debug.Log("Cannot play card!");
                return;
            }

            // Deduct energy
            currentEnergy -= card.energyCost;

            // Apply Effects
            foreach (var effect in card.effects)
            {
                switch (effect.effectType)
                {
                    case EffectType.Damage:
                        target.TakeDamage(effect.value);
                        break;
                    case EffectType.Block:
                        playerUnit.AddBlock(effect.value);
                        break;
                    case EffectType.Heal:
                        // Simple heal logic (not fully implemented in Unit yet)
                        break;
                    case EffectType.DrawCard:
                        deckManager.DrawCards(effect.value);
                        break;
                }
            }

            // Move from hand to discard
            deckManager.PlayCard(card);

            // Check if enemy died
            if (enemyUnit.IsDead())
            {
                state = BattleState.Won;
                Debug.Log("Battle Won!");
                TechTest.Core.RunManager.Instance.EndBattle(true, playerUnit.currentHP);
            }
        }
    }
}
