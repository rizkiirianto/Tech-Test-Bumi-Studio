using UnityEngine;
using TechTest.Data;
using TechTest.Combat;
using System.Collections.Generic;

namespace TechTest.Core
{
    public class RunManager : MonoBehaviour
    {
        public static RunManager Instance;

        [Header("Run State")]
        public int currentRoomIndex = 0;
        public int maxRooms = 4; // Requirement: at least 4 stages

        [Header("Persistent Resources")]
        public int currentRunHP;
        public int currentFatigue;
        
        [Header("Data References")]
        public UnitData heroData;
        public CardData fatigueDebuffCard; // The card added to deck when too fatigued
        
        [Header("System References")]
        public DeckManager deckManager;
        public BattleManager battleManager;
        
        [Header("Enemy Encounters")]
        public List<UnitData> normalEnemies; 
        public UnitData bossEnemy;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void StartNewRun()
        {
            currentRoomIndex = 0;
            currentRunHP = heroData.maxHP;
            currentFatigue = 0;
            
            Debug.Log("New Run Started!");
            LoadMapNode();
        }

        public void LoadMapNode()
        {
            if (currentRoomIndex >= maxRooms)
            {
                Debug.Log("YOU WIN! You have completed the run.");
                return;
            }

            currentRoomIndex++;
            Debug.Log($"Entering Room {currentRoomIndex}");
            
            // For prototype: Room 1, 2 are normal combat. Room 3 is Campfire. Room 4 is Boss.
            if (currentRoomIndex == 3)
            {
                EnterCampfireNode();
            }
            else
            {
                EnterCombatNode();
            }
        }

        private void EnterCombatNode()
        {
            Debug.Log("Combat Node Started.");
            
            // Add fatigue for traveling/fighting
            AddFatigue(20);

            // Determine which enemy to fight
            UnitData enemyToFight;
            if (currentRoomIndex == maxRooms)
            {
                enemyToFight = bossEnemy;
                Debug.Log("BOSS ENCOUNTER!");
            }
            else
            {
                int randomIndex = Random.Range(0, normalEnemies.Count);
                enemyToFight = normalEnemies[randomIndex];
            }
            
            battleManager.StartBattle(heroData, enemyToFight);
            battleManager.playerUnit.SetHP(currentRunHP);
        }

        private void EnterCampfireNode()
        {
            Debug.Log("Campfire Node Started. Choose: Rest or Upgrade?");
            // We can show a UI with 2 buttons here.
            // For now, we'll simulate clicking Rest:
            Rest();
        }

        public void Rest()
        {
            Debug.Log("You rested at the campfire.");
            currentFatigue = 0; // Reset fatigue
            
            // Heal a bit
            currentRunHP += (int)(heroData.maxHP * 0.3f); 
            if (currentRunHP > heroData.maxHP) currentRunHP = heroData.maxHP;
            
            Debug.Log($"Fatigue reset. HP is now {currentRunHP}");
            
            CompleteNode();
        }

        public void CompleteNode()
        {
            Debug.Log("Node Completed. Proceeding...");
            LoadMapNode();
        }

        public void EndBattle(bool won, int remainingHP)
        {
            if (won)
            {
                currentRunHP = remainingHP;
                CompleteNode();
            }
            else
            {
                Debug.Log("GAME OVER! Your hero has fallen.");
            }
        }

        private void AddFatigue(int amount)
        {
            currentFatigue += amount;
            Debug.Log($"Fatigue increased by {amount}. Current: {currentFatigue}");

            if (currentFatigue >= 100)
            {
                Debug.Log("Fatigue limit reached! Adding an Exhaustion card to your deck.");
                deckManager.currentRunDeck.Add(fatigueDebuffCard);
                // Reduce fatigue so it can happen again, or cap it.
                currentFatigue = 50; 
            }
        }
    }
}
