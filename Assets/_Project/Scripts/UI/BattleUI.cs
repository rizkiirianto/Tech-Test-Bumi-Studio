using UnityEngine;
using UnityEngine.UI;
using TechTest.Combat;
using TMPro; // Assuming using TextMeshPro

namespace TechTest.UI
{
    public class BattleUI : MonoBehaviour
    {
        public BattleManager battleManager;
        public DeckManager deckManager;

        [Header("Player UI")]
        public TextMeshProUGUI playerHPText;
        public TextMeshProUGUI playerBlockText;
        public TextMeshProUGUI energyText;

        [Header("Enemy UI")]
        public TextMeshProUGUI enemyHPText;
        public TextMeshProUGUI enemyBlockText;
        public TextMeshProUGUI enemyIntentText; // Shows what the enemy will do

        [Header("Deck UI")]
        public TextMeshProUGUI drawPileCountText;
        public TextMeshProUGUI discardPileCountText;
        public Transform handContainer; // Parent for card UI prefabs
        public GameObject cardUIPrefab; // We will create this prefab later in Unity

        private void OnEnable()
        {
            deckManager.onHandChanged += UpdateDeckUI;
        }

        private void OnDisable()
        {
            deckManager.onHandChanged -= UpdateDeckUI;
        }

        private void Update()
        {
            // Simple polling for UI updates (In a real game, use events for better performance)
            if (battleManager.state == BattleState.PlayerTurn || battleManager.state == BattleState.EnemyTurn)
            {
                UpdateUnitUI();
                if (energyText != null) energyText.text = $"Energy: {battleManager.currentEnergy} / {battleManager.maxEnergy}";
            }
        }

        private void UpdateUnitUI()
        {
            if (battleManager.playerUnit != null && battleManager.playerUnit.unitData != null)
            {
                if (playerHPText != null) playerHPText.text = $"HP: {battleManager.playerUnit.currentHP} / {battleManager.playerUnit.unitData.maxHP}";
                if (playerBlockText != null) playerBlockText.text = $"Block: {battleManager.playerUnit.currentBlock}";
            }

            if (battleManager.enemyUnit != null && battleManager.enemyUnit.unitData != null)
            {
                if (enemyHPText != null) enemyHPText.text = $"HP: {battleManager.enemyUnit.currentHP} / {battleManager.enemyUnit.unitData.maxHP}";
                if (enemyBlockText != null) enemyBlockText.text = $"Block: {battleManager.enemyUnit.currentBlock}";
                if (enemyIntentText != null) enemyIntentText.text = $"Intent: Attack {battleManager.upcomingEnemyDamage}";
            }
        }

        private void UpdateDeckUI()
        {
            if (drawPileCountText != null) drawPileCountText.text = $"Draw : {deckManager.drawPile.Count}";
            if (discardPileCountText != null) discardPileCountText.text = $"Discard : {deckManager.discardPile.Count}";

            if (handContainer == null) return;

            // Clear current hand UI
            foreach (Transform child in handContainer)
            {
                Destroy(child.gameObject);
            }

            // Spawn new hand UI
            for (int i = 0; i < deckManager.hand.Count; i++)
            {
                if (cardUIPrefab != null && handContainer != null)
                {
                    GameObject cardGo = Instantiate(cardUIPrefab, handContainer);
                    cardGo.name = "Card_" + deckManager.hand[i].cardName;
                    
                    CardUI cardUI = cardGo.GetComponent<CardUI>();
                    if (cardUI != null)
                    {
                        cardUI.Setup(deckManager.hand[i]);
                    }
                }
            }
        }

        public void OnEndTurnClicked()
        {
            battleManager.OnEndTurnButton();
        }
    }
}
