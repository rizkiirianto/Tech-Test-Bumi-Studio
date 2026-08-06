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
        public int maxRooms = 7;

        [Header("Persistent Resources")]
        public int currentRunHP;
        public int currentFatigue;

        [Header("Data References")]
        public UnitData heroData;
        private UnitData originalHeroData; // Untuk menyimpan referensi file aslinya

        public CardData fatigueDebuffCard; // The card added to deck when too fatigued

        [Header("System References")]
        public DeckManager deckManager;
        public BattleManager battleManager;

        [Header("Enemy Encounters")]
        public List<UnitData> normalEnemies;
        public UnitData bossEnemy;
        public UnitData tutorialEnemy; // Musuh khusus untuk Room 1

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            // Simpan referensi file aslinya di memori saat game pertama kali nyala
            if (heroData != null && originalHeroData == null)
            {
                originalHeroData = heroData;
            }
        }

        public void StartNewRun()
        {
            // Buat CLONE (salinan) dari data asli agar file .asset tidak kotor
            if (originalHeroData != null)
            {
                heroData = Instantiate(originalHeroData);
            }

            currentRoomIndex = 0;
            currentRunHP = heroData.maxHP;
            currentFatigue = 0;

            if (deckManager != null)
            {
                deckManager.ResetDeck();
            }

            Debug.Log("New Run Started!");
            LoadMapNode();
        }

        public void LoadMapNode()
        {
            if (currentRoomIndex >= maxRooms)
            {
                Debug.Log("YOU WIN! You have completed the run.");
                TechTest.UI.RunUIManager.Instance.ShowGameOver(true);
                return;
            }

            currentRoomIndex++;
            Debug.Log($"Entering Room {currentRoomIndex}");

            // Room 3 and 6 are Campfire. Room 7 is Boss.
            if (currentRoomIndex == 3 || currentRoomIndex == 6)
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
            TechTest.UI.RunUIManager.Instance.ShowBattle();

            AddFatigue(10);

            List<UnitData> enemiesToFight = new List<UnitData>();

            if (currentRoomIndex == maxRooms)
            {
                enemiesToFight.Add(bossEnemy);
                Debug.Log("BOSS ENCOUNTER!");
            }
            else if (currentRoomIndex == 1)
            {
                // Room 1 selalu 1 musuh tutorial (atau musuh pertama di list normal jika kosong)
                UnitData firstEnemy = tutorialEnemy != null ? tutorialEnemy : normalEnemies[0];
                enemiesToFight.Add(firstEnemy);
                Debug.Log("Tutorial Encounter! 1 enemy approaches.");
            }
            else if (currentRoomIndex == 2)
            {
                // Spawn 1 to 3 regular enemies untuk Room lain
                int enemyCount = Random.Range(1, 2);
                for (int i = 0; i < enemyCount; i++)
                {
                    int randomIndex = Random.Range(0, normalEnemies.Count);
                    enemiesToFight.Add(normalEnemies[randomIndex]);
                }
                Debug.Log($"Normal Encounter! {enemyCount} enemies approach.");
            }
            else if (currentRoomIndex == 3)
            {
                // Spawn 1 to 3 regular enemies untuk Room lain
                int enemyCount = Random.Range(1, 3);
                for (int i = 0; i < enemyCount; i++)
                {
                    int randomIndex = Random.Range(0, normalEnemies.Count);
                    enemiesToFight.Add(normalEnemies[randomIndex]);
                }
                Debug.Log($"Normal Encounter! {enemyCount} enemies approach.");
            }
            else
            {
                // Spawn 1 to 3 regular enemies untuk Room lain
                int enemyCount = Random.Range(1, 4);
                for (int i = 0; i < enemyCount; i++)
                {
                    int randomIndex = Random.Range(0, normalEnemies.Count);
                    enemiesToFight.Add(normalEnemies[randomIndex]);
                }
                Debug.Log($"Normal Encounter! {enemyCount} enemies approach.");
            }

            battleManager.StartBattle(heroData, enemiesToFight);
            battleManager.playerUnit.SetHP(currentRunHP);
        }

        private void EnterCampfireNode()
        {
            Debug.Log("Campfire Node Started. Choose: Rest or Upgrade?");
            TechTest.UI.RunUIManager.Instance.ShowCampfire();
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

        public void Train()
        {
            Debug.Log("You trained at the campfire. You feel stronger, but tired.");
            // Train increases max HP for this run, but doesn't heal or reset fatigue
            currentRunHP += 10;
            heroData.maxHP += 10;

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
                if (currentRoomIndex < maxRooms)
                {
                    TechTest.UI.RunUIManager.Instance.ShowReward();
                }
                else
                {
                    TechTest.UI.RunUIManager.Instance.ShowGameOver(true);
                }
            }
            else
            {
                Debug.Log("GAME OVER! Your hero has fallen.");
                TechTest.UI.RunUIManager.Instance.ShowGameOver(false);
            }
        }

        public void AddFatigue(int amount)
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
