using UnityEngine;

namespace CardGame.Game
{
    /// <summary>
    /// Represents the current state of the game.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Lobby,
        GameStarting,
        Bidding,      // Players bid 5-10
        TalonPhase,   // Declarer picks up 2 hidden cards and discards
        PlayerTurn,   // Gameplay starts
        OpponentTurn,
        GameEnding,
        GameOver
    }

    /// <summary>
    /// Main game manager that controls the overall game flow.
    /// </summary>
    public class GameManager : Utilities.Singleton<GameManager>
    {
        [Header("Game State")]
        [SerializeField] private GameState _currentState = GameState.MainMenu;

        [Header("Game Settings")]
        [SerializeField] private int _totalCards = 32;
        [SerializeField] private int _cardsPerPlayer = 10;

        public GameState CurrentState => _currentState;

        // Events
        public event System.Action<GameState> OnGameStateChanged;

        protected override void Awake()
        {
            base.Awake();
            // Additional initialization
        }

        private void Start()
        {
            Debug.Log("[GameManager] Game Manager initialized.");
        }

        /// <summary>
        /// Changes the current game state.
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (_currentState == newState) return;

            Debug.Log($"[GameManager] State changed: {_currentState} -> {newState}");
            _currentState = newState;
            OnGameStateChanged?.Invoke(newState);

            HandleStateTransition(newState);
        }

        private void HandleStateTransition(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    // Load main menu scene
                    break;
                case GameState.Lobby:
                    // Initialize lobby
                    break;
                case GameState.GameStarting:
                    StartGame();
                    break;
                case GameState.Bidding:
                    // Enable Bidding UI (Buttons 5, 6, 7, 8, 9, 10, Pass)
                    break;
                case GameState.TalonPhase:
                    // 1. Identify Declarer
                    // 2. Reveal the 2 cards in _talonPosition to the Declarer
                    // 3. Allow Declarer to swap 2 cards from their hand
                    break;
                case GameState.PlayerTurn:
                    // Begin player's turn
                    break;
                case GameState.OpponentTurn:
                    // Begin opponent's turn
                    break;
                case GameState.GameEnding:
                    EndGame();
                    break;
                case GameState.GameOver:
                    // Show game over screen
                    break;
            }
        }

        private void StartGame()
        {
            Debug.Log("[GameManager] Starting Mexico match...");
            
            // Find the GameSetup component and trigger spawning
            GameSetup setup = FindFirstObjectByType<GameSetup>();
            if (setup != null)
            {
                setup.SetupGame();
                Debug.Log("[GameManager] SetupGame called.");
            }

            // Only move to Bidding after the server has finished spawning
            ChangeState(GameState.Bidding);
        }

        private void EndGame()
        {
            Debug.Log("[GameManager] Ending game...");
            // TODO: Clean up game state
            // - Calculate rewards
            // - Show results
            // - Cleanup resources
        }

        /// <summary>
        /// Quits the application.
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("[GameManager] Quitting game...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
