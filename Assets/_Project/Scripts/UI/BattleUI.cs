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
                energyText.text = $"Energy: {battleManager.currentEnergy} / {battleManager.maxEnergy}";
            }
        }

        private void UpdateUnitUI()
        {
            if (battleManager.playerUnit != null)
            {
                playerHPText.text = $"HP: {battleManager.playerUnit.currentHP} / {battleManager.playerUnit.unitData.maxHP}";
                playerBlockText.text = $"Block: {battleManager.playerUnit.currentBlock}";
            }

            if (battleManager.enemyUnit != null)
            {
                enemyHPText.text = $"HP: {battleManager.enemyUnit.currentHP} / {battleManager.enemyUnit.unitData.maxHP}";
                enemyBlockText.text = $"Block: {battleManager.enemyUnit.currentBlock}";
            }
        }

        private void UpdateDeckUI()
        {
            drawPileCountText.text = deckManager.drawPile.Count.ToString();
            discardPileCountText.text = deckManager.discardPile.Count.ToString();

            // Clear current hand UI
            foreach (Transform child in handContainer)
            {
                Destroy(child.gameObject);
            }

            // Spawn new hand UI (placeholder logic)
            // In Unity editor, you'll need to attach a script to the prefab to handle clicking
            for (int i = 0; i < deckManager.hand.Count; i++)
            {
                if (cardUIPrefab != null && handContainer != null)
                {
                    GameObject cardGo = Instantiate(cardUIPrefab, handContainer);
                    cardGo.name = "Card_" + deckManager.hand[i].cardName;
                    
                    // You would usually get a CardUI script here and pass the CardData to it
                    // e.g. cardGo.GetComponent<CardUI>().Setup(deckManager.hand[i]);
                }
            }
        }

        public void OnEndTurnClicked()
        {
            battleManager.OnEndTurnButton();
        }
    }
}
