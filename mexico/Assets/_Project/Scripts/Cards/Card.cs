using UnityEngine;
using TMPro;

namespace CardGame.Cards
{
    /// <summary>
    /// Represents a card instance in the game.
    /// </summary>
    public class Card : MonoBehaviour
    {
        [Header("Card Data")]
        [SerializeField] private CardData _data;

        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TextMeshPro _cardText;

        [Header("Runtime State")]
        [SerializeField] private bool _isFaceDown = false;

        // Properties
        public CardData Data => _data;
        public bool IsFaceDown => _isFaceDown;

        private void Awake()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            // Create text if it doesn't exist
            if (_cardText == null)
            {
                GameObject textObj = new GameObject("CardText");
                textObj.transform.SetParent(transform);
                textObj.transform.localPosition = Vector3.zero;
                textObj.transform.localScale = Vector3.one * 0.1f;
                
                _cardText = textObj.AddComponent<TextMeshPro>();
                _cardText.alignment = TextAlignmentOptions.Center;
                _cardText.fontSize = 4;
                _cardText.color = Color.black;
            }
        }

        // Events
        public event System.Action<Card> OnCardPlayed;
        public event System.Action<Card> OnCardDestroyed;

        /// <summary>
        /// Initializes the card with data from CardData.
        /// </summary>
        public void InitializeCard()
        {
            if (_data == null)
            {
                // Don't log error here - data might be set later via SetCardData()
                return;
            }

            UpdateVisual();
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
            UpdateVisual();
            Debug.Log($"[Card] {(_data != null ? _data.CardName : "Card")} face down: {faceDown}");
        }

        /// <summary>
        /// Updates the card's visual based on its data and face-down state.
        /// </summary>
        private void UpdateVisual()
        {
            if (_data == null)
                return;

            // Update sprite if available
            if (_spriteRenderer != null)
            {
                if (_isFaceDown)
                {
                    // Show card back if face down
                    if (_data.CardBack != null)
                    {
                        _spriteRenderer.sprite = _data.CardBack;
                    }
                }
                else
                {
                    // Show card face
                    if (_data.CardFace != null)
                    {
                        _spriteRenderer.sprite = _data.CardFace;
                    }
                }
            }
            
            // Update text to show card info
            if (_cardText != null)
            {
                if (_isFaceDown)
                {
                    _cardText.text = "?";
                }
                else
                {
                    // Show rank and suit symbol
                    string suitSymbol = GetSuitSymbol(_data.Suit);
                    _cardText.text = $"{GetRankShort(_data.Rank)}{suitSymbol}";
                }
            }
        }

        private string GetSuitSymbol(CardSuit suit)
        {
            switch (suit)
            {
                case CardSuit.Hearts: return "♥";
                case CardSuit.Diamonds: return "♦";
                case CardSuit.Clubs: return "♣";
                case CardSuit.Spades: return "♠";
                default: return "";
            }
        }

        private string GetRankShort(CardRank rank)
        {
            switch (rank)
            {
                case CardRank.Jack: return "J";
                case CardRank.Queen: return "Q";
                case CardRank.King: return "K";
                case CardRank.Ace: return "A";
                default: return ((int)rank).ToString();
            }
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
