using UnityEngine;
using Mirror;

namespace CardGame.Network
{
    /// <summary>
    /// Represents a networked player in the game.
    /// Handles player-specific network synchronization.
    /// </summary>
    public class NetworkPlayer : NetworkBehaviour
    {
        [Header("Player Info")]
        [SyncVar] private string _playerName;
        [SyncVar] private int _playerHealth;
        [SyncVar] private int _playerMana;
        [SyncVar] private bool _isPlayerTurn;

        [Header("References")]
        [SerializeField] private Player.Player _localPlayer;

        public string PlayerName => _playerName;
        public int PlayerHealth => _playerHealth;
        public int PlayerMana => _playerMana;
        public bool IsPlayerTurn => _isPlayerTurn;

        // Events
        public event System.Action<int> OnHealthChanged;
        public event System.Action<int> OnManaChanged;
        public event System.Action<bool> OnTurnChanged;

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            // Initialize server-side values
            _playerHealth = Game.GameManager.Instance.StartingHealth;
            _playerMana = Game.GameManager.Instance.StartingMana;
            
            Debug.Log($"[NetworkPlayer] Server initialized player {netId}");
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.Log($"[NetworkPlayer] Client initialized for player {netId}");
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            Debug.Log($"[NetworkPlayer] Local player started");
            
            // Set a default name (can be changed later)
            CmdSetPlayerName($"Player_{Random.Range(1000, 9999)}");
        }

        /// <summary>
        /// Sets the player's name (called by client).
        /// </summary>
        [Command]
        public void CmdSetPlayerName(string name)
        {
            _playerName = name;
            Debug.Log($"[NetworkPlayer] Player name set to {name}");
        }

        /// <summary>
        /// Updates player health (called by server).
        /// </summary>
        [Server]
        public void SetHealth(int health)
        {
            _playerHealth = Mathf.Max(0, health);
            RpcHealthChanged(_playerHealth);
        }

        [ClientRpc]
        private void RpcHealthChanged(int newHealth)
        {
            OnHealthChanged?.Invoke(newHealth);
            Debug.Log($"[NetworkPlayer] Health changed to {newHealth}");
            
            if (newHealth <= 0)
            {
                Debug.Log($"[NetworkPlayer] Player {_playerName} has been defeated!");
            }
        }

        /// <summary>
        /// Updates player mana (called by server).
        /// </summary>
        [Server]
        public void SetMana(int mana)
        {
            _playerMana = Mathf.Max(0, mana);
            RpcManaChanged(_playerMana);
        }

        [ClientRpc]
        private void RpcManaChanged(int newMana)
        {
            OnManaChanged?.Invoke(newMana);
            Debug.Log($"[NetworkPlayer] Mana changed to {newMana}");
        }

        /// <summary>
        /// Sets whether it's this player's turn (called by server).
        /// </summary>
        [Server]
        public void SetTurn(bool isTurn)
        {
            _isPlayerTurn = isTurn;
            RpcTurnChanged(_isPlayerTurn);
        }

        [ClientRpc]
        private void RpcTurnChanged(bool isTurn)
        {
            OnTurnChanged?.Invoke(isTurn);
            Debug.Log($"[NetworkPlayer] Turn state: {(isTurn ? "Your turn" : "Opponent's turn")}");
        }

        /// <summary>
        /// Client requests to play a card (validated by server).
        /// </summary>
        [Command]
        public void CmdPlayCard(uint cardNetId, int targetPlayerIndex = -1)
        {
            if (!_isPlayerTurn)
            {
                Debug.LogWarning("[NetworkPlayer] Cannot play card - not your turn!");
                return;
            }

            if (_playerMana < 1) // TODO: Check actual card cost
            {
                Debug.LogWarning("[NetworkPlayer] Cannot play card - not enough mana!");
                return;
            }

            Debug.Log($"[NetworkPlayer] Player {_playerName} plays card {cardNetId}");
            
            // TODO: Implement card play logic
            // - Validate card in hand
            // - Check mana cost
            // - Apply card effect
            // - Update game state
            
            RpcCardPlayed(cardNetId);
        }

        [ClientRpc]
        private void RpcCardPlayed(uint cardNetId)
        {
            Debug.Log($"[NetworkPlayer] Card {cardNetId} was played");
            // TODO: Play card animation and effects
        }

        /// <summary>
        /// Client requests to end their turn.
        /// </summary>
        [Command]
        public void CmdEndTurn()
        {
            if (!_isPlayerTurn)
            {
                Debug.LogWarning("[NetworkPlayer] Cannot end turn - not your turn!");
                return;
            }

            Debug.Log($"[NetworkPlayer] Player {_playerName} ends turn");
            
            // TODO: Notify server to switch turns
            if (NetworkServer.active)
            {
                // Switch to next player
                SetTurn(false);
            }
        }

        /// <summary>
        /// Deals damage to this player (called by server).
        /// </summary>
        [Server]
        public void TakeDamage(int damage)
        {
            int newHealth = _playerHealth - damage;
            SetHealth(newHealth);
            
            RpcShowDamage(damage);
        }

        [ClientRpc]
        private void RpcShowDamage(int damage)
        {
            Debug.Log($"[NetworkPlayer] {_playerName} took {damage} damage!");
            // TODO: Show damage visual effect
        }
    }
}
