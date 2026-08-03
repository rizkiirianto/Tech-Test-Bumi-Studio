using System.Collections.Generic;
using UnityEngine;
using TechTest.Data;
using System.Linq;

namespace TechTest.Combat
{
    public class DeckManager : MonoBehaviour
    {
        [Header("Deck States")]
        public List<CardData> currentRunDeck = new List<CardData>();
        
        [Header("Battle States")]
        public List<CardData> drawPile = new List<CardData>();
        public List<CardData> hand = new List<CardData>();
        public List<CardData> discardPile = new List<CardData>();

        // Event for UI to update when hand changes
        public delegate void OnHandChanged();
        public event OnHandChanged onHandChanged;

        /// <summary>
        /// Called at the start of a combat encounter.
        /// </summary>
        public void InitializeBattle()
        {
            drawPile.Clear();
            hand.Clear();
            discardPile.Clear();

            // Clone the run deck into the draw pile
            drawPile.AddRange(currentRunDeck);
            ShuffleList(drawPile);
        }

        public void DrawCards(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (drawPile.Count == 0)
                {
                    if (discardPile.Count > 0)
                    {
                        ShuffleDiscardIntoDrawPile();
                    }
                    else
                    {
                        Debug.Log("No cards left to draw!");
                        break; // No cards in draw and discard piles
                    }
                }

                CardData cardToDraw = drawPile[0];
                drawPile.RemoveAt(0);
                hand.Add(cardToDraw);
            }
            
            onHandChanged?.Invoke();
        }

        public void PlayCard(CardData card)
        {
            if (hand.Contains(card))
            {
                hand.Remove(card);
                discardPile.Add(card);
                onHandChanged?.Invoke();
            }
        }

        public void DiscardHand()
        {
            discardPile.AddRange(hand);
            hand.Clear();
            onHandChanged?.Invoke();
        }

        private void ShuffleDiscardIntoDrawPile()
        {
            drawPile.AddRange(discardPile);
            discardPile.Clear();
            ShuffleList(drawPile);
            Debug.Log("Reshuffled discard pile into draw pile.");
        }

        private void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }
}
