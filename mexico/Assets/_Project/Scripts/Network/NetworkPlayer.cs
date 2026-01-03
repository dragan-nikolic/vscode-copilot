using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

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
        private NetworkVariable<bool> _isPlayerTurn = new NetworkVariable<bool>();
        private NetworkVariable<int> _currentBid = new NetworkVariable<int>(0);
        private NetworkVariable<bool> _hasPassed = new NetworkVariable<bool>(false);

        [Header("References")]
        // [SerializeField] private Player.Player _localPlayer; // TODO: Create Player class

        public string PlayerName => _playerName.Value.ToString();
        public bool IsPlayerTurn => _isPlayerTurn.Value;
        public int CurrentBid => _currentBid.Value;
        public bool HasPassed => _hasPassed.Value;

        // Events
        public event System.Action<bool> OnTurnChanged;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                Debug.Log($"[NetworkPlayer] Server initialized player {NetworkObjectId}");
            }

            // Subscribe to network variable changes on all clients
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

            Debug.Log($"[NetworkPlayer] Player {PlayerName} plays card {cardNetId}");
            
            // TODO: Implement card play logic
            // - Validate card in hand
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
        /// Client calls this to place a bid.
        /// </summary>
        [ServerRpc]
        public void SubmitBidServerRpc(int bidAmount)
        {
            // Basic validation: must be higher than 4 and the current high bid
            // In Mexico, 10 is the special "Meksiko" bid
            _currentBid.Value = bidAmount;
            _hasPassed.Value = false;
            
            Debug.Log($"[NetworkPlayer] {PlayerName} bid {bidAmount}");
            
            // Logic to move to the next player's turn would go here in your Bidding Manager
        }

        /// <summary>
        /// Client calls this to pass.
        /// </summary>
        [ServerRpc]
        public void PassServerRpc()
        {
            _hasPassed.Value = true;
            Debug.Log($"[NetworkPlayer] {PlayerName} passed.");
        }    
    }
}
