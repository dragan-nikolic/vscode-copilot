using UnityEngine;
using System.Collections.Generic;

namespace CardGame.Cards
{
    /// <summary>
    /// ScriptableObject that manages the collection of all cards.
    /// Used to store and access card data for the game.
    /// </summary>
    [CreateAssetMenu(fileName = "CardDatabase", menuName = "Card Game/Card Database")]
    public class CardDatabase : ScriptableObject
    {
        [Header("Card Collection")]
        [SerializeField] private List<CardData> _cards = new List<CardData>();

        /// <summary>
        /// Gets all cards in the database.
        /// </summary>
        public List<CardData> Cards => _cards;

        /// <summary>
        /// Gets a card by its unique ID.
        /// </summary>
        public CardData GetCardById(string cardId)
        {
            return _cards.Find(card => card.CardId == cardId);
        }

        /// <summary>
        /// Gets all cards of a specific suit.
        /// </summary>
        public List<CardData> GetCardsBySuit(CardSuit suit)
        {
            return _cards.FindAll(card => card.Suit == suit);
        }

        /// <summary>
        /// Gets all cards of a specific rank.
        /// </summary>
        public List<CardData> GetCardsByRank(CardRank rank)
        {
            return _cards.FindAll(card => card.Rank == rank);
        }

        /// <summary>
        /// Adds a card to the database (editor only).
        /// </summary>
        public void AddCard(CardData card)
        {
            if (!_cards.Contains(card))
            {
                _cards.Add(card);
            }
        }

        /// <summary>
        /// Removes a card from the database (editor only).
        /// </summary>
        public void RemoveCard(CardData card)
        {
            _cards.Remove(card);
        }
    }
}
