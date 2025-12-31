using UnityEngine;

namespace CardGame.Cards
{
    /// <summary>
    /// Represents a card instance in the game.
    /// </summary>
    public class Card : MonoBehaviour
    {
        [Header("Card Data")]
        [SerializeField] private CardData _data;

        [Header("Runtime State")]
        [SerializeField] private int _currentAttack;
        [SerializeField] private int _currentHealth;
        [SerializeField] private bool _canAttack = false;
        [SerializeField] private bool _isFaceDown = false;

        // Properties
        public CardData Data => _data;
        public int CurrentAttack => _currentAttack;
        public int CurrentHealth => _currentHealth;
        public bool CanAttack => _canAttack;
        public bool IsFaceDown => _isFaceDown;

        // Events
        public event System.Action<Card> OnCardPlayed;
        public event System.Action<Card> OnCardDestroyed;

        private void Awake()
        {
            InitializeCard();
        }

        /// <summary>
        /// Initializes the card with data from CardData.
        /// </summary>
        public void InitializeCard()
        {
            if (_data == null)
            {
                Debug.LogError("[Card] CardData is null!");
                return;
            }

            _currentAttack = _data.Attack;
            _currentHealth = _data.Health;
        }

        /// <summary>
        /// Sets the card data for this card instance.
        /// </summary>
        public void SetCardData(CardData data)
        {
            _data = data;
            InitializeCard();
        }

        /// <summary>
        /// Sets whether the card is face down (hidden).
        /// </summary>
        public void SetFaceDown(bool faceDown)
        {
            _isFaceDown = faceDown;
            // TODO: Update card visual to show/hide card face
            Debug.Log($"[Card] {(_data != null ? _data.CardName : "Card")} face down: {faceDown}");
        }

        /// <summary>
        /// Plays the card (called when dragged to the board).
        /// </summary>
        public void PlayCard()
        {
            Debug.Log($"[Card] Playing card: {_data.CardName}");
            OnCardPlayed?.Invoke(this);
            
            // TODO: Implement card effects based on type
            switch (_data.Type)
            {
                case CardType.Creature:
                    SummonCreature();
                    break;
                case CardType.Spell:
                    CastSpell();
                    break;
                case CardType.Enchantment:
                    ApplyEnchantment();
                    break;
            }
        }

        private void SummonCreature()
        {
            Debug.Log($"[Card] Summoning creature: {_data.CardName}");
            // Creatures can't attack on the turn they're summoned (summoning sickness)
            _canAttack = false;
        }

        private void CastSpell()
        {
            Debug.Log($"[Card] Casting spell: {_data.CardName}");
            // Spells have immediate effects and go to the discard pile
        }

        private void ApplyEnchantment()
        {
            Debug.Log($"[Card] Applying enchantment: {_data.CardName}");
            // Enchantments remain on the board with ongoing effects
        }

        /// <summary>
        /// Called at the start of each turn for this card.
        /// </summary>
        public void OnTurnStart()
        {
            if (_data.Type == CardType.Creature)
            {
                _canAttack = true; // Remove summoning sickness
            }
        }

        /// <summary>
        /// Deals damage to this card (for creatures).
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (_data.Type != CardType.Creature) return;

            _currentHealth -= damage;
            Debug.Log($"[Card] {_data.CardName} took {damage} damage. Health: {_currentHealth}");

            if (_currentHealth <= 0)
            {
                DestroyCard();
            }
        }

        /// <summary>
        /// Destroys this card.
        /// </summary>
        public void DestroyCard()
        {
            Debug.Log($"[Card] {_data.CardName} destroyed.");
            OnCardDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
