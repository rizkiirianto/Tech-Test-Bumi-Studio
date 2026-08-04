using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TechTest.Data;

namespace TechTest.Combat
{
    public enum BattleState
    {
        Start,
        PlayerTurn,
        ChoosingTarget,
        EnemyTurn,
        Won,
        Lost
    }

    public class BattleManager : MonoBehaviour
    {
        public BattleState state;

        public Unit playerUnit;
        public List<Unit> activeEnemies = new List<Unit>();

        [Header("Enemy Spawning")]
        public Transform[] enemySpawnPoints;
        public GameObject enemyPrefab;
        public GameObject enemyUIPrefab;
        public Transform[] enemyUIContainers;

        public DeckManager deckManager;

        [Header("Energy System")]
        public int currentEnergy;
        public int maxEnergy;

        // Pending card for targeting
        private CardData pendingCard;

        private void Update()
        {
            // Cancel targeting with right click
            if (state == BattleState.ChoosingTarget && Input.GetMouseButtonDown(1))
            {
                CancelTargeting();
            }
        }

        public void StartBattle(UnitData pData, List<UnitData> eDataList)
        {
            state = BattleState.Start;

            // Initialize Player
            playerUnit.Initialize(pData);

            // Clear old enemies & old UI
            foreach (var e in activeEnemies)
            {
                if (e != null) Destroy(e.gameObject);
            }
            activeEnemies.Clear();

            if (enemyUIContainers != null)
            {
                foreach (var container in enemyUIContainers)
                {
                    if (container != null)
                    {
                        foreach (Transform child in container)
                        {
                            Destroy(child.gameObject);
                        }
                    }
                }
            }

            // Spawn Enemies
            for (int i = 0; i < eDataList.Count; i++)
            {
                if (i >= enemySpawnPoints.Length) break;

                // Spawn Enemy Unit
                GameObject eGo = Instantiate(enemyPrefab, enemySpawnPoints[i]);
                Unit eUnit = eGo.GetComponent<Unit>();
                eUnit.Initialize(eDataList[i]);
                activeEnemies.Add(eUnit);

                // Spawn Enemy UI
                if (enemyUIPrefab != null && enemyUIContainers != null && i < enemyUIContainers.Length)
                {
                    GameObject uiGo = Instantiate(enemyUIPrefab, enemyUIContainers[i]);
                    
                    // Kita biarkan posisinya mengikuti Container (berguna jika kamu memposisikan container manual di Canvas)
                    uiGo.transform.localPosition = Vector3.zero;

                    TechTest.UI.EnemyUI eUI = uiGo.GetComponent<TechTest.UI.EnemyUI>();
                    if (eUI != null) eUI.Setup(eUnit);
                }
            }

            maxEnergy = pData.maxEnergy;

            // Setup Deck
            deckManager.InitializeBattle();

            StartCoroutine(PlayerTurn());
        }

        private IEnumerator PlayerTurn()
        {
            state = BattleState.PlayerTurn;

            playerUnit.ClearBlock();
            currentEnergy = maxEnergy;

            // Determine Enemy Intents (Random for all)
            foreach (var enemy in activeEnemies)
            {
                // We'll store intent in currentBlock temporarily as a hack, or just random it per enemy later. 
                // A better approach is to add an `intentDamage` variable to `Unit.cs`.
                // Let's add it to Unit dynamically or just random on turn.
            }

            deckManager.DrawCards(playerUnit.unitData.drawPerTurn);

            Debug.Log("Player Turn Started. Energy: " + currentEnergy);
            yield return null;
        }

        public void OnEndTurnButton()
        {
            if (state != BattleState.PlayerTurn && state != BattleState.ChoosingTarget) return;

            if (state == BattleState.ChoosingTarget) CancelTargeting();

            deckManager.DiscardHand();
            StartCoroutine(EnemyTurn());
        }

        private IEnumerator EnemyTurn()
        {
            state = BattleState.EnemyTurn;
            Debug.Log("Enemy Turn Started.");

            yield return new WaitForSeconds(0.5f);

            // Enemies attack one by one
            foreach (var enemy in activeEnemies)
            {
                if (enemy.isDead) continue;

                enemy.ClearBlock();

                // Fake intent for now: random attack
                int dmg = Random.Range(4, 9);
                Debug.Log($"{enemy.unitData.unitName} attacks for {dmg} damage!");
                playerUnit.TakeDamage(dmg);

                yield return new WaitForSeconds(0.5f);

                if (playerUnit.isDead)
                {
                    state = BattleState.Lost;
                    Debug.Log("Game Over! Player died.");
                    TechTest.Core.RunManager.Instance.EndBattle(false, 0);
                    yield break;
                }
            }

            StartCoroutine(PlayerTurn());
        }

        public bool CanPlayCard(CardData card)
        {
            return currentEnergy >= card.energyCost;
        }

        public void BeginTargeting(CardData card)
        {
            if (!CanPlayCard(card) || state != BattleState.PlayerTurn)
            {
                Debug.Log("Cannot play card! Not enough energy or wrong phase.");
                return;
            }

            pendingCard = card;
            state = BattleState.ChoosingTarget;
            Debug.Log($"Choosing target for {card.cardName}. Right click to cancel.");
        }

        public void CancelTargeting()
        {
            pendingCard = null;
            state = BattleState.PlayerTurn;
            Debug.Log("Targeting cancelled.");
        }

        public void ExecuteTargetedCard(Unit target)
        {
            if (state != BattleState.ChoosingTarget || pendingCard == null) return;

            // Optional: If card is "Self", force target to be player.
            // If card is "AllEnemies", ignore target and loop activeEnemies.
            // For now, we apply to whatever was clicked.

            currentEnergy -= pendingCard.energyCost;

            foreach (var effect in pendingCard.effects)
            {
                switch (effect.effectType)
                {
                    case EffectType.Damage:
                        target.TakeDamage(effect.value);
                        break;
                    case EffectType.Block:
                        target.AddBlock(effect.value); // Usually cast on player
                        break;
                    case EffectType.Heal:
                        // target.Heal(effect.value);
                        break;
                    case EffectType.DrawCard:
                        deckManager.DrawCards(effect.value);
                        break;
                }
            }

            deckManager.PlayCard(pendingCard);
            pendingCard = null;

            CheckWinCondition();

            if (state != BattleState.Won)
            {
                state = BattleState.PlayerTurn;
            }
        }

        private void CheckWinCondition()
        {
            // Remove dead enemies
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                if (activeEnemies[i].isDead)
                {
                    Destroy(activeEnemies[i].gameObject);
                    activeEnemies.RemoveAt(i);
                }
            }

            if (activeEnemies.Count == 0)
            {
                state = BattleState.Won;
                Debug.Log("Battle Won!");
                TechTest.Core.RunManager.Instance.EndBattle(true, playerUnit.currentHP);
            }
        }
    }
}
