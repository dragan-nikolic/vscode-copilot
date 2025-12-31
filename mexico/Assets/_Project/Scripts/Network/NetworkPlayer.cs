using UnityEngine;
using Unity.Netcode;

namespace CardGame.Network
{
    /// <summary>
    /// Represents a networked player in the game.
    /// Handles player-specific network synchronization.
    /// </summary>
    public class NetworkPlayer : NetworkBehaviour
    {
        [Header("Player Info")]
        private NetworkVariable<FixedString64Bytes> _playerName = new NetworkVariable<FixedString64Bytes>();
        private NetworkVariable<int> _playerHealth = new NetworkVariable<int>();
        private NetworkVariable<int> _playerMana = new NetworkVariable<int>();
        private NetworkVariable<bool> _isPlayerTurn = new NetworkVariable<bool>();

        [Header("References")]
        [SerializeField] private Player.Player _localPlayer;

        public string PlayerName => _playerName.Value.ToString();
        public int PlayerHealth => _playerHealth.Value;
        public int PlayerMana => _playerMana.Value;
        public bool IsPlayerTurn => _isPlayerTurn.Value;

        // Events
        public event System.Action<int> OnHealthChanged;
        public event System.Action<int> OnManaChanged;
        public event System.Action<bool> OnTurnChanged;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                // Initialize server-side values
                _playerHealth.Value = Game.GameManager.Instance.StartingHealth;
                _playerMana.Value = Game.GameManager.Instance.StartingMana;
                
                Debug.Log($"[NetworkPlayer] Server initialized player {NetworkObjectId}");
            }

            // Subscribe to network variable changes on all clients
            _playerHealth.OnValueChanged += OnHealthChangedCallback;
            _playerMana.OnValueChanged += OnManaChangedCallback;
            _isPlayerTurn.OnValueChanged += OnTurnChangedCallback;

            Debug.Log($"[NetworkPlayer] Client initialized for player {NetworkObjectId}");

            if (IsOwner)
            {
                Debug.Log($"[NetworkPlayer] Local player started");
                
                // Set a default name (can be changed later)
                SetPlayerNameServerRpc($"Player_{Random.Range(1000, 9999)}");
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            // Unsubscribe from network variable changes
            _playerHealth.OnValueChanged -= OnHealthChangedCallback;
            _playerMana.OnValueChanged -= OnManaChangedCallback;
            _isPlayerTurn.OnValueChanged -= OnTurnChangedCallback;
        }

        /// <summary>
        /// Sets the player's name (called by client).
        /// </summary>
        [ServerRpc]
        public void SetPlayerNameServerRpc(string name)
        {
            _playerName.Value = name;
            Debug.Log($"[NetworkPlayer] Player name set to {name}");
        }

        /// <summary>
        /// Updates player health (called by server).
        /// </summary>
        public void SetHealth(int health)
        {
            if (!IsServer) return;

            _playerHealth.Value = Mathf.Max(0, health);
            HealthChangedClientRpc(_playerHealth.Value);
        }

        [Rpc(SendTo.Everyone)]
        private void HealthChangedClientRpc(int newHealth)
        {
            Debug.Log($"[NetworkPlayer] Health changed to {newHealth}");
            
            if (newHealth <= 0)
            {
                Debug.Log($"[NetworkPlayer] Player {PlayerName} has been defeated!");
            }
        }

        private void OnHealthChangedCallback(int oldHealth, int newHealth)
        {
            OnHealthChanged?.Invoke(newHealth);
        }

        /// <summary>
        /// Updates player mana (called by server).
        /// </summary>
        public void SetMana(int mana)
        {
            if (!IsServer) return;

            _playerMana.Value = Mathf.Max(0, mana);
            ManaChangedClientRpc(_playerMana.Value);
        }

        [Rpc(SendTo.Everyone)]
        private void ManaChangedClientRpc(int newMana)
        {
            Debug.Log($"[NetworkPlayer] Mana changed to {newMana}");
        }

        private void OnManaChangedCallback(int oldMana, int newMana)
        {
            OnManaChanged?.Invoke(newMana);
        }

        /// <summary>
        /// Sets whether it's this player's turn (called by server).
        /// </summary>
        public void SetTurn(bool isTurn)
        {
            if (!IsServer) return;

            _isPlayerTurn.Value = isTurn;
            TurnChangedClientRpc(_isPlayerTurn.Value);
        }

        [Rpc(SendTo.Everyone)]
        private void TurnChangedClientRpc(bool isTurn)
        {
            Debug.Log($"[NetworkPlayer] Turn state: {(isTurn ? "Your turn" : "Opponent's turn")}");
        }

        private void OnTurnChangedCallback(bool oldTurn, bool newTurn)
        {
            OnTurnChanged?.Invoke(newTurn);
        }

        /// <summary>
        /// Client requests to play a card (validated by server).
        /// </summary>
        [ServerRpc]
        public void PlayCardServerRpc(ulong cardNetId, int targetPlayerIndex = -1)
        {
            if (!_isPlayerTurn.Value)
            {
                Debug.LogWarning("[NetworkPlayer] Cannot play card - not your turn!");
                return;
            }

            if (_playerMana.Value < 1) // TODO: Check actual card cost
            {
                Debug.LogWarning("[NetworkPlayer] Cannot play card - not enough mana!");
                return;
            }

            Debug.Log($"[NetworkPlayer] Player {PlayerName} plays card {cardNetId}");
            
            // TODO: Implement card play logic
            // - Validate card in hand
            // - Check mana cost
            // - Apply card effect
            // - Update game state
            
            CardPlayedClientRpc(cardNetId);
        }

        [Rpc(SendTo.Everyone)]
        private void CardPlayedClientRpc(ulong cardNetId)
        {
            Debug.Log($"[NetworkPlayer] Card {cardNetId} was played");
            // TODO: Play card animation and effects
        }

        /// <summary>
        /// Client requests to end their turn.
        /// </summary>
        [ServerRpc]
        public void EndTurnServerRpc()
        {
            if (!_isPlayerTurn.Value)
            {
                Debug.LogWarning("[NetworkPlayer] Cannot end turn - not your turn!");
                return;
            }

            Debug.Log($"[NetworkPlayer] Player {PlayerName} ends turn");
            
            // TODO: Notify server to switch turns
            if (IsServer)
            {
                // Switch to next player
                SetTurn(false);
            }
        }

        /// <summary>
        /// Deals damage to this player (called by server).
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (!IsServer) return;

            int newHealth = _playerHealth.Value - damage;
            SetHealth(newHealth);
            
            ShowDamageClientRpc(damage);
        }

        [Rpc(SendTo.Everyone)]
        private void ShowDamageClientRpc(int damage)
        {
            Debug.Log($"[NetworkPlayer] {PlayerName} took {damage} damage!");
            // TODO: Show damage visual effect
        }
    }
}
