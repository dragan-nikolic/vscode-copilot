using UnityEngine;

namespace CardGame.Cards
{
    /// <summary>
    /// Types of cards available in the game.
    /// </summary>
    public enum CardType
    {
        Creature,
        Spell,
        Enchantment
    }

    /// <summary>
    /// Rarity levels for cards.
    /// </summary>
    public enum CardRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    /// <summary>
    /// ScriptableObject that stores card data.
    /// </summary>
    [CreateAssetMenu(fileName = "New Card", menuName = "Card Game/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string _cardName;
        [SerializeField] private string _cardId; // Unique identifier
        [TextArea(3, 6)]
        [SerializeField] private string _description;
        
        [Header("Card Properties")]
        [SerializeField] private CardType _cardType;
        [SerializeField] private CardRarity _rarity;
        [SerializeField] private int _manaCost;
        
        [Header("Stats (for Creatures)")]
        [SerializeField] private int _attack;
        [SerializeField] private int _health;
        
        [Header("Visuals")]
        [SerializeField] private Sprite _cardArtwork;
        [SerializeField] private Sprite _cardFrame;

        // Properties
        public string CardName => _cardName;
        public string CardId => _cardId;
        public string Description => _description;
        public CardType Type => _cardType;
        public CardRarity Rarity => _rarity;
        public int ManaCost => _manaCost;
        public int Attack => _attack;
        public int Health => _health;
        public Sprite CardArtwork => _cardArtwork;
        public Sprite CardFrame => _cardFrame;

        private void OnValidate()
        {
            // Auto-generate card ID if empty
            if (string.IsNullOrEmpty(_cardId))
            {
                _cardId = System.Guid.NewGuid().ToString();
            }
        }
    }
}
