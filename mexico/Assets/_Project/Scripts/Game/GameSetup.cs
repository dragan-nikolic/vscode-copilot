using UnityEngine;
using Unity.Netcode;
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
    public class GameSetup : NetworkBehaviour 
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

        [Header("Card Layout")]
        [SerializeField] private float _cardSpacing = 0.3f; // Space between cards
        [SerializeField] private float _cardOverlap = 0.7f; // How much cards overlap (0-1, higher = more overlap)

        [Header("Testing")]
        [SerializeField] private bool _testModeWithoutNetwork = false;
        [SerializeField] private KeyCode _testSpawnKey = KeyCode.Space;

        private List<GameObject> _allCards = new List<GameObject>();
        private Dictionary<int, List<GameObject>> _playerHands = new Dictionary<int, List<GameObject>>();
        private List<GameObject> _talonCards = new List<GameObject>();

        private void Start()
        {
            ValidateSetup();
        }

        private void Update()
        {
            // Allow testing card spawn with spacebar
            if (_testModeWithoutNetwork && Input.GetKeyDown(_testSpawnKey))
            {
                Debug.Log("[GameSetup] Test mode: Spawning cards...");
                SetupGame();
            }
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
            // Only the Server/Host should execute the setup logic
            if (!IsServer) return;

            Debug.Log("[GameSetup] Server is setting up the game...");

            List<Cards.CardData> deck = CreateDeck();
            ShuffleDeck(deck);
            
            // Logic to distribute card indices to clients via RPCs
            DistributeCardsNetworked(deck);
        }

        private void DistributeCardsNetworked(List<Cards.CardData> deck)
        {
            // TODO: For each player, send them a list of their 10 card IDs
            // For the Talon, keep it on the server and only reveal to the Declarer later
        }

        /// <summary>
        /// Creates a deck of 32 cards.
        /// </summary>
        private List<Cards.CardData> CreateDeck()
        {
            List<Cards.CardData> deck = new List<Cards.CardData>();

            if (_cardDatabase != null && _cardDatabase.Cards != null && _cardDatabase.Cards.Count > 0)
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
                Debug.LogWarning("[GameSetup] No card database assigned or empty! Creating test cards in memory.");
                
                // Create test cards in memory for testing
                deck = CreateTestDeck();
            }

            Debug.Log($"[GameSetup] Created deck with {deck.Count} cards");
            return deck;
        }

        /// <summary>
        /// Creates a test deck of 32 cards in memory for testing purposes.
        /// </summary>
        private List<Cards.CardData> CreateTestDeck()
        {
            List<Cards.CardData> testDeck = new List<Cards.CardData>();
            Cards.CardSuit[] suits = { Cards.CardSuit.Hearts, Cards.CardSuit.Diamonds, 
                                       Cards.CardSuit.Clubs, Cards.CardSuit.Spades };
            Cards.CardRank[] ranks = { Cards.CardRank.Seven, Cards.CardRank.Eight, 
                                       Cards.CardRank.Nine, Cards.CardRank.Ten,
                                       Cards.CardRank.Jack, Cards.CardRank.Queen, 
                                       Cards.CardRank.King, Cards.CardRank.Ace };

            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    Cards.CardData testCard = ScriptableObject.CreateInstance<Cards.CardData>();
                    // Use reflection to set private fields for testing
                    var suitField = typeof(Cards.CardData).GetField("_suit", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var rankField = typeof(Cards.CardData).GetField("_rank", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var valueField = typeof(Cards.CardData).GetField("_value", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var cardFaceField = typeof(Cards.CardData).GetField("_cardFace", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    suitField?.SetValue(testCard, suit);
                    rankField?.SetValue(testCard, rank);
                    valueField?.SetValue(testCard, (int)rank);
                    
                    testDeck.Add(testCard);
                }
            }

            Debug.Log($"[GameSetup] Created {testDeck.Count} test cards");
            return testDeck;
        }

        /// <summary>
        /// Gets the sprite name pattern for a card.
        /// </summary>
        private string GetSpriteNameForCard(Cards.CardSuit suit, Cards.CardRank rank)
        {
            string suitName = suit.ToString();
            string rankName = rank.ToString();
            
            // Common naming patterns in card sprite sheets
            return $"{suitName}_{rankName}";
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
                        int cardIndexInHand = _playerHands[playerIndex].Count;
                        GameObject card = CreateCard(deck[cardIndex], playerIndex, false, cardIndexInHand);
                        _playerHands[playerIndex].Add(card);
                        cardIndex++;
                    }
                }
            }

            // Place remaining cards in talon (face down)
            for (int i = 0; i < _remainingCards && cardIndex < deck.Count; i++)
            {
                GameObject talonCard = CreateCard(deck[cardIndex], -1, true, i);
                _talonCards.Add(talonCard);
                cardIndex++;
            }

            Debug.Log($"[GameSetup] Distributed {cardIndex} cards: " +
                $"{_cardsPerPlayer} to each of {_playerCount} players, {_remainingCards} to talon");
        }

        public void SortPlayerHands()
        {
            List<int> playerIndices = _playerHands.Keys.ToList();

            foreach (int playerIndex in playerIndices)
            {
                List<GameObject> hand = _playerHands[playerIndex];

                var sortedHand = hand
                    .OrderBy(obj => {
                        var data = obj.GetComponent<Cards.Card>().Data;
                        return data.Suit switch {
                            Cards.CardSuit.Hearts => 0,
                            Cards.CardSuit.Clubs => 1,
                            Cards.CardSuit.Diamonds => 2,
                            Cards.CardSuit.Spades => 3,
                            _ => 4
                        };
                    })
                    // Explicitly cast Rank to int to ensure 11 (Jack) comes after 10
                    .ThenBy(obj => (int)obj.GetComponent<Cards.Card>().Data.Rank) 
                    .ToList();

                _playerHands[playerIndex] = sortedHand;

                for (int i = 0; i < sortedHand.Count; i++)
                {
                    Transform spawnPosition = GetPlayerHandPosition(playerIndex);
                    Vector3 offset = CalculateCardOffset(i);
                    sortedHand[i].transform.position = spawnPosition.position + offset;
                    
                    if (sortedHand[i].TryGetComponent<SpriteRenderer>(out var sr))
                    {
                        sr.sortingOrder = i;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a card game object.
        /// </summary>
        private GameObject CreateCard(Cards.CardData cardData, int playerIndex, bool faceDown = false, int cardIndexInHand = 0)
        {
            if (_cardPrefab == null)
            {
                Debug.LogError("[GameSetup] Card prefab not assigned!");
                return null;
            }

            // Determine spawn position based on player index
            Transform spawnPosition = GetPlayerHandPosition(playerIndex);
            
            // Calculate card offset for fanning/spreading
            Vector3 offset = CalculateCardOffset(cardIndexInHand);
            Vector3 finalPosition = spawnPosition.position + offset;
            
            GameObject cardObject = Instantiate(_cardPrefab, finalPosition, spawnPosition.rotation);
            
            // Set sorting order so cards overlap correctly
            SpriteRenderer sr = cardObject.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = cardIndexInHand;
            }
            
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
        /// Calculates the position offset for a card based on its index in hand.
        /// </summary>
        private Vector3 CalculateCardOffset(int cardIndex)
        {
            // Center the cards around the hand position
            float totalWidth = (_cardsPerPlayer - 1) * _cardSpacing * (1 - _cardOverlap);
            float startOffset = -totalWidth / 2f;
            
            float xOffset = startOffset + (cardIndex * _cardSpacing * (1 - _cardOverlap));
            
            // Slight vertical offset for visual depth (optional)
            float zOffset = cardIndex * 0.01f; // Small Z offset for proper layering
            
            return new Vector3(xOffset, 0, zOffset);
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
