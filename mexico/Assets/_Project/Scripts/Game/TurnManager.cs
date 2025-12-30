using UnityEngine;

namespace CardGame.Game
{
    /// <summary>
    /// Manages the turn-based system of the game.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        [Header("Turn Settings")]
        [SerializeField] private float _turnDuration = 60f; // seconds per turn
        [SerializeField] private int _currentTurn = 0;
        [SerializeField] private bool _isPlayerTurn = true;

        private float _turnTimer;

        public int CurrentTurn => _currentTurn;
        public bool IsPlayerTurn => _isPlayerTurn;
        public float TurnTimeRemaining => _turnTimer;

        // Events
        public event System.Action OnTurnStart;
        public event System.Action OnTurnEnd;
        public event System.Action<bool> OnTurnChanged; // true = player turn, false = opponent turn

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.PlayerTurn &&
                GameManager.Instance.CurrentState != GameState.OpponentTurn)
                return;

            _turnTimer -= Time.deltaTime;

            if (_turnTimer <= 0)
            {
                EndTurn();
            }
        }

        /// <summary>
        /// Starts a new turn.
        /// </summary>
        public void StartTurn(bool isPlayerTurn)
        {
            _isPlayerTurn = isPlayerTurn;
            _currentTurn++;
            _turnTimer = _turnDuration;

            Debug.Log($"[TurnManager] Turn {_currentTurn} started. {(isPlayerTurn ? "Player" : "Opponent")} turn.");

            OnTurnStart?.Invoke();
            OnTurnChanged?.Invoke(isPlayerTurn);

            if (isPlayerTurn)
            {
                GameManager.Instance.ChangeState(GameState.PlayerTurn);
                // TODO: Draw card, restore mana, etc.
            }
            else
            {
                GameManager.Instance.ChangeState(GameState.OpponentTurn);
                // TODO: AI or network opponent logic
            }
        }

        /// <summary>
        /// Ends the current turn.
        /// </summary>
        public void EndTurn()
        {
            Debug.Log($"[TurnManager] Turn {_currentTurn} ended.");
            OnTurnEnd?.Invoke();

            // Switch turns
            StartTurn(!_isPlayerTurn);
        }

        /// <summary>
        /// Resets the turn manager for a new game.
        /// </summary>
        public void ResetTurns()
        {
            _currentTurn = 0;
            _turnTimer = _turnDuration;
            Debug.Log("[TurnManager] Turn manager reset.");
        }
    }
}
