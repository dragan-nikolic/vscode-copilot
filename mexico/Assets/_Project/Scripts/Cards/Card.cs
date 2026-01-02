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
        [SerializeField] private bool _isFaceDown = false;

        // Properties
        public CardData Data => _data;
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

            Debug.Log($"[Card] Initialized {_data.CardName}");
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
            
            // TODO: Implement card playing logic for your game
        }

        /// <summary>
        /// Called at the start of each turn for this card.
        /// </summary>
        public void OnTurnStart()
        {
            Debug.Log($"[Card] Turn started for {_data.CardName}");
            // TODO: Implement turn start logic
        }

        /// <summary>
        /// Handles card interactions.
        /// </summary>
        public void TakeDamage(int damage)
        {
            Debug.Log($"[Card] {_data.CardName} interaction");
            // TODO: Implement card interaction logic
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
