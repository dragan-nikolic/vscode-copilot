using UnityEngine;

namespace CardGame.Cards
{
    /// <summary>
    /// Card suits for traditional playing cards.
    /// </summary>
    public enum CardSuit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    /// <summary>
    /// Card ranks for traditional playing cards.
    /// </summary>
    public enum CardRank
    {
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }

    /// <summary>
    /// ScriptableObject that stores traditional playing card data.
    /// </summary>
    [CreateAssetMenu(fileName = "New Card", menuName = "Card Game/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string _cardId; // Unique identifier
        
        [Header("Card Properties")]
        [SerializeField] private CardSuit _suit;
        [SerializeField] private CardRank _rank;
        [SerializeField] private int _value; // Point value for scoring
        
        [Header("Visuals")]
        [SerializeField] private Sprite _cardFace; // Front of the card
        [SerializeField] private Sprite _cardBack; // Back of the card

        // Properties
        public string CardId => _cardId;
        public CardSuit Suit => _suit;
        public CardRank Rank => _rank;
        public int Value => _value;
        public Sprite CardFace => _cardFace;
        public Sprite CardBack => _cardBack;
        
        /// <summary>
        /// Gets the full card name (e.g., "Ace of Hearts").
        /// </summary>
        public string CardName => $"{_rank} of {_suit}";

        private void OnValidate()
        {
            // Auto-generate card ID if empty
            if (string.IsNullOrEmpty(_cardId))
            {
                _cardId = $"{_suit}_{_rank}";
            }
        }
    }
}
