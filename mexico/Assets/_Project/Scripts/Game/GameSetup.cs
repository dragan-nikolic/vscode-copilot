using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CardGame.Game
{
    /// <summary>
    /// Handles the initial game setup including card distribution.
    /// Game Rules:
    /// - 32 cards total in the deck
    /// - 3 players, each receives 10 cards
    /// - 2 cards remain face down (talon/stock)
    /// </summary>
    public class GameSetup : MonoBehaviour
    {
        [Header("Game Configuration")]
        [SerializeField] private int _totalCards = 32;
        [SerializeField] private int _cardsPerPlayer = 10;
        [SerializeField] private int _remainingCards = 2;
        [SerializeField] private int _playerCount = 3;

        [Header("Card Prefab")]
        [SerializeField] private GameObject _cardPrefab;

        [Header("Spawn Positions")]
        [SerializeField] private Transform _player1HandPosition;
        [SerializeField] private Transform _player2HandPosition;
        [SerializeField] private Transform _player3HandPosition;
        [SerializeField] private Transform _talonPosition;

        [Header("Deck")]
        [SerializeField] private Cards.CardDatabase _cardDatabase;

        private List<GameObject> _allCards = new List<GameObject>();
        private Dictionary<int, List<GameObject>> _playerHands = new Dictionary<int, List<GameObject>>();
        private List<GameObject> _talonCards = new List<GameObject>();

        private void Start()
        {
            ValidateSetup();
        }

        /// <summary>
        /// Validates that the game setup configuration is correct.
        /// </summary>
        private void ValidateSetup()
        {
            int distributedCards = (_cardsPerPlayer * _playerCount) + _remainingCards;
            
            if (distributedCards != _totalCards)
            {
                Debug.LogError($"[GameSetup] Card count mismatch! " +
                    $"Expected {_totalCards} cards, but distribution requires {distributedCards} cards. " +
                    $"({_cardsPerPlayer} per player × {_playerCount} players + {_remainingCards} remaining)");
            }
            else
            {
                Debug.Log($"[GameSetup] Configuration valid: {_totalCards} total cards, " +
                    $"{_cardsPerPlayer} per player, {_remainingCards} in talon");
            }
        }

        /// <summary>
        /// Initializes and distributes all cards for the game.
        /// </summary>
        public void SetupGame()
        {
            Debug.Log("[GameSetup] Setting up game...");

            // Create deck
            List<Cards.CardData> deck = CreateDeck();

            // Shuffle deck
            ShuffleDeck(deck);

            // Distribute cards
            DistributeCards(deck);

            Debug.Log($"[GameSetup] Game setup complete. " +
                $"Players: {_playerCount}, Cards per player: {_cardsPerPlayer}, Talon: {_remainingCards}");
        }

        /// <summary>
        /// Creates a deck of 32 cards.
        /// </summary>
        private List<Cards.CardData> CreateDeck()
        {
            List<Cards.CardData> deck = new List<Cards.CardData>();

            if (_cardDatabase != null && _cardDatabase.Cards != null)
            {
                // Use cards from database (ensure there are at least 32 unique cards)
                deck.AddRange(_cardDatabase.Cards.Take(_totalCards));
                
                if (deck.Count < _totalCards)
                {
                    Debug.LogWarning($"[GameSetup] Card database has only {deck.Count} cards, " +
                        $"but {_totalCards} are required!");
                }
            }
            else
            {
                Debug.LogWarning("[GameSetup] No card database assigned! Creating placeholder cards.");
                
                // Create placeholder card data if no database is assigned
                for (int i = 0; i < _totalCards; i++)
                {
                    // TODO: Create actual card data from database
                    // For now, this is a placeholder
                }
            }

            Debug.Log($"[GameSetup] Created deck with {deck.Count} cards");
            return deck;
        }

        /// <summary>
        /// Shuffles the deck using Fisher-Yates algorithm.
        /// </summary>
        private void ShuffleDeck(List<Cards.CardData> deck)
        {
            System.Random rng = new System.Random();
            int n = deck.Count;
            
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Cards.CardData temp = deck[k];
                deck[k] = deck[n];
                deck[n] = temp;
            }

            Debug.Log("[GameSetup] Deck shuffled");
        }

        /// <summary>
        /// Distributes cards to players and talon.
        /// </summary>
        private void DistributeCards(List<Cards.CardData> deck)
        {
            int cardIndex = 0;

            // Initialize player hands
            for (int playerIndex = 0; playerIndex < _playerCount; playerIndex++)
            {
                _playerHands[playerIndex] = new List<GameObject>();
            }

            // Deal cards to players (round-robin)
            for (int cardNum = 0; cardNum < _cardsPerPlayer; cardNum++)
            {
                for (int playerIndex = 0; playerIndex < _playerCount; playerIndex++)
                {
                    if (cardIndex < deck.Count)
                    {
                        GameObject card = CreateCard(deck[cardIndex], playerIndex);
                        _playerHands[playerIndex].Add(card);
                        cardIndex++;
                    }
                }
            }

            // Place remaining cards in talon (face down)
            for (int i = 0; i < _remainingCards && cardIndex < deck.Count; i++)
            {
                GameObject talonCard = CreateCard(deck[cardIndex], -1, true);
                _talonCards.Add(talonCard);
                cardIndex++;
            }

            Debug.Log($"[GameSetup] Distributed {cardIndex} cards: " +
                $"{_cardsPerPlayer} to each of {_playerCount} players, {_remainingCards} to talon");
        }

        /// <summary>
        /// Creates a card game object.
        /// </summary>
        private GameObject CreateCard(Cards.CardData cardData, int playerIndex, bool faceDown = false)
        {
            if (_cardPrefab == null)
            {
                Debug.LogError("[GameSetup] Card prefab not assigned!");
                return null;
            }

            // Determine spawn position based on player index
            Transform spawnPosition = GetPlayerHandPosition(playerIndex);
            
            GameObject cardObject = Instantiate(_cardPrefab, spawnPosition.position, spawnPosition.rotation);
            
            // Set up card component
            Cards.Card cardComponent = cardObject.GetComponent<Cards.Card>();
            if (cardComponent != null && cardData != null)
            {
                cardComponent.SetCardData(cardData);
                cardComponent.SetFaceDown(faceDown);
            }

            _allCards.Add(cardObject);
            return cardObject;
        }

        /// <summary>
        /// Gets the hand position for a specific player.
        /// </summary>
        private Transform GetPlayerHandPosition(int playerIndex)
        {
            switch (playerIndex)
            {
                case 0: return _player1HandPosition ?? transform;
                case 1: return _player2HandPosition ?? transform;
                case 2: return _player3HandPosition ?? transform;
                case -1: return _talonPosition ?? transform; // Talon cards
                default: return transform;
            }
        }

        /// <summary>
        /// Gets a player's hand.
        /// </summary>
        public List<GameObject> GetPlayerHand(int playerIndex)
        {
            if (_playerHands.ContainsKey(playerIndex))
            {
                return new List<GameObject>(_playerHands[playerIndex]);
            }
            return new List<GameObject>();
        }

        /// <summary>
        /// Gets the talon cards.
        /// </summary>
        public List<GameObject> GetTalonCards()
        {
            return new List<GameObject>(_talonCards);
        }

        /// <summary>
        /// Clears the current game setup.
        /// </summary>
        public void ClearGame()
        {
            foreach (GameObject card in _allCards)
            {
                if (card != null)
                {
                    Destroy(card);
                }
            }

            _allCards.Clear();
            _playerHands.Clear();
            _talonCards.Clear();

            Debug.Log("[GameSetup] Game cleared");
        }

        private void OnValidate()
        {
            ValidateSetup();
        }
    }
}
