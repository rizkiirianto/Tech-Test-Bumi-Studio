using UnityEngine;
using UnityEngine.UI;
using TechTest.Data;
using TechTest.Combat;
using TMPro;

namespace TechTest.UI
{
    public class CardUI : MonoBehaviour
    {
        public TextMeshProUGUI cardNameText;
        public TextMeshProUGUI cardCostText;
        public TextMeshProUGUI cardDescriptionText;
        // public Image cardIconImage; // Uncomment if you want to use icons

        private CardData currentCardData;
        private BattleManager battleManager;

        public void Setup(CardData data)
        {
            currentCardData = data;
            cardNameText.text = data.cardName;
            cardCostText.text = data.energyCost.ToString();
            cardDescriptionText.text = data.description;

            // Find BattleManager (in a full game, better to pass this reference during instantiation)
            battleManager = FindFirstObjectByType<BattleManager>();
        }

        // Call this from a UI Button component on the Card Prefab
        public void OnClickPlayCard()
        {
            if (battleManager == null || currentCardData == null) return;

            // For now, always target the enemy.
            // If you add targeting later, you'd change this logic.
            battleManager.PlayCard(currentCardData, battleManager.enemyUnit);
        }
    }
}
