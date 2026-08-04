using UnityEngine;
using TMPro;
using TechTest.Core;
using TechTest.Data;
using System.Collections.Generic;

namespace TechTest.UI
{
    public class RunUIManager : MonoBehaviour
    {
        public static RunUIManager Instance;

        [Header("Global Run Info")]
        public TextMeshProUGUI globalHPText;
        public TextMeshProUGUI globalFatigueText;
        public TextMeshProUGUI roomText;

        [Header("Panels")]
        public GameObject battleUIPanel;
        public GameObject rewardUIPanel;
        public GameObject campfireUIPanel;
        public GameObject gameOverUIPanel;

        [Header("Reward UI Elements")]
        public List<CardData> possibleCardRewards; // List of all droppable cards
        public Transform rewardCardContainer;
        public GameObject rewardCardPrefab;

        [Header("Game Over UI Elements")]
        public TextMeshProUGUI gameOverTitleText;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (RunManager.Instance != null)
            {
                globalHPText.text = $"Run HP: {RunManager.Instance.currentRunHP}";
                globalFatigueText.text = $"Fatigue: {RunManager.Instance.currentFatigue}";
                roomText.text = $"Room: {RunManager.Instance.currentRoomIndex}";

                if (RunManager.Instance.currentFatigue >= 80)
                    globalFatigueText.color = Color.red;
                else
                    globalFatigueText.color = Color.black;
            }
        }

        public void ShowBattle()
        {
            HideAll();
            battleUIPanel.SetActive(true);
        }

        public void ShowReward()
        {
            HideAll();
            rewardUIPanel.SetActive(true);
            GenerateRewards();
        }

        public void ShowCampfire()
        {
            HideAll();
            campfireUIPanel.SetActive(true);
        }

        public void ShowGameOver(bool isWin)
        {
            HideAll();
            gameOverUIPanel.SetActive(true);
            gameOverTitleText.text = isWin ? "VICTORY!" : "YOU DIED";
        }

        private void HideAll()
        {
            battleUIPanel.SetActive(false);
            rewardUIPanel.SetActive(false);
            campfireUIPanel.SetActive(false);
            gameOverUIPanel.SetActive(false);
        }

        // --- Reward Logic ---
        private void GenerateRewards()
        {
            // Clear old rewards
            foreach (Transform child in rewardCardContainer)
            {
                Destroy(child.gameObject);
            }

            // Pick 3 random cards
            for (int i = 0; i < 3; i++)
            {
                CardData randomCard = possibleCardRewards[Random.Range(0, possibleCardRewards.Count)];
                GameObject cardGo = Instantiate(rewardCardPrefab, rewardCardContainer);

                CardUI cardUI = cardGo.GetComponent<CardUI>();
                if (cardUI != null)
                {
                    cardUI.Setup(randomCard);
                    // Override the onClick to add to deck instead of playing it
                    UnityEngine.UI.Button btn = cardGo.GetComponent<UnityEngine.UI.Button>();
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => OnRewardSelected(randomCard));
                }
            }
        }

        private void OnRewardSelected(CardData selectedCard)
        {
            Debug.Log($"Added {selectedCard.cardName} to Deck!");
            RunManager.Instance.deckManager.currentRunDeck.Add(selectedCard);
            RunManager.Instance.CompleteNode();
        }

        // --- Campfire Logic ---
        public void OnClickRest()
        {
            RunManager.Instance.Rest();
        }

        public void OnClickTrain()
        {
            RunManager.Instance.Train();
        }

        // --- Game Over Logic ---
        public void OnClickRestart()
        {
            RunManager.Instance.StartNewRun();
        }
    }
}
