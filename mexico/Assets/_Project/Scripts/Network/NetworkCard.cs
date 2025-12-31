using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace CardGame.Network
{
    /// <summary>
    /// Network synchronization for card objects.
    /// Handles card state across the network.
    /// </summary>
    public class NetworkCard : NetworkBehaviour
    {
        [Header("Card Reference")]
        [SerializeField] private Cards.Card _card;

        [Header("Network State")]
        private NetworkVariable<FixedString64Bytes> _cardId = new NetworkVariable<FixedString64Bytes>();
        private NetworkVariable<ulong> _ownerId = new NetworkVariable<ulong>();
        private NetworkVariable<CardNetworkState> _cardState = new NetworkVariable<CardNetworkState>(CardNetworkState.InDeck);

        public string CardId => _cardId.Value.ToString();
        public ulong OwnerId => _ownerId.Value;
        public CardNetworkState CardState => _cardState.Value;

        // Events
        public event System.Action<string> OnCardIdUpdated;
        public event System.Action<ulong> OnOwnerUpdated;
        public event System.Action<CardNetworkState> OnStateUpdated;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsServer)
            {
                if (_card != null && _card.Data != null)
                {
                    _cardId.Value = _card.Data.CardId;
                }
                
                Debug.Log($"[NetworkCard] Server initialized card {NetworkObjectId} - CardID: {CardId}");
            }

            // Subscribe to network variable changes
            _cardId.OnValueChanged += OnCardIdChangedCallback;
            _ownerId.OnValueChanged += OnOwnerChangedCallback;
            _cardState.OnValueChanged += OnCardStateChangedCallback;

            Debug.Log($"[NetworkCard] Client received card {NetworkObjectId}");
            
            // Initialize local card with network data
            if (_card != null && !string.IsNullOrEmpty(CardId))
            {
                // TODO: Load card data from CardDatabase using _cardId
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            // Unsubscribe from network variable changes
            _cardId.OnValueChanged -= OnCardIdChangedCallback;
            _ownerId.OnValueChanged -= OnOwnerChangedCallback;
            _cardState.OnValueChanged -= OnCardStateChangedCallback;
        }

        /// <summary>
        /// Initializes the network card with card data (server only).
        /// </summary>
        public void Initialize(Cards.CardData cardData, ulong ownerNetId)
        {
            if (!IsServer) return;

            if (cardData == null)
            {
                Debug.LogError("[NetworkCard] Cannot initialize with null CardData!");
                return;
            }

            _cardId.Value = cardData.CardId;
            _ownerId.Value = ownerNetId;
            _cardState.Value = CardNetworkState.InDeck;

            if (_card == null)
                _card = GetComponent<Cards.Card>();

            if (_card != null)
                _card.SetCardData(cardData);

            Debug.Log($"[NetworkCard] Initialized card {CardId} for player {ownerNetId}");
        }

        /// <summary>
        /// Changes the card's network state (server only).
        /// </summary>
        public void SetCardState(CardNetworkState newState)
        {
            if (!IsServer) return;

            _cardState.Value = newState;
            Debug.Log($"[NetworkCard] Card {CardId} state changed to {newState}");
        }

        /// <summary>
        /// Plays the card on the network (command from client).
        /// </summary>
        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void PlayCardServerRpc(RpcParams rpcParams = default)
        {
            ulong senderId = rpcParams.Receive.SenderClientId;

            // Get the player's NetworkObject
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(senderId, out var clientInfo))
            {
                Debug.LogWarning($"[NetworkCard] Client {senderId} not found!");
                return;
            }

            NetworkPlayer player = clientInfo.PlayerObject?.GetComponent<NetworkPlayer>();
            if (player == null || player.NetworkObjectId != _ownerId.Value)
            {
                Debug.LogWarning($"[NetworkCard] Player {senderId} tried to play card they don't own!");
                return;
            }

            // Verify it's the player's turn
            if (!player.IsPlayerTurn)
            {
                Debug.LogWarning($"[NetworkCard] Player tried to play card during opponent's turn!");
                return;
            }

            Debug.Log($"[NetworkCard] Playing card {CardId}");
            
            // Change state and play card
            SetCardState(CardNetworkState.InPlay);
            PlayCardClientRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void PlayCardClientRpc()
        {
            Debug.Log($"[NetworkCard] Card {CardId} played on all clients");
            
            if (_card != null)
            {
                _card.PlayCard();
            }
            
            // TODO: Trigger card play animation
        }

        /// <summary>
        /// Destroys the card on the network (server only).
        /// </summary>
        public void DestroyCard()
        {
            if (!IsServer) return;

            Debug.Log($"[NetworkCard] Destroying card {CardId}");
            SetCardState(CardNetworkState.Destroyed);
            DestroyCardClientRpc();
            
            // Destroy after a short delay to allow animations
            Invoke(nameof(DestroyNetworkObject), 0.5f);
        }

        [Rpc(SendTo.Everyone)]
        private void DestroyCardClientRpc()
        {
            Debug.Log($"[NetworkCard] Card {CardId} destroyed on all clients");
            
            if (_card != null)
            {
                _card.DestroyCard();
            }
        }

        private void DestroyNetworkObject()
        {
            if (IsServer && NetworkObject != null)
            {
                NetworkObject.Despawn();
            }
        }

        // Network Variable Callbacks
        private void OnCardIdChangedCallback(FixedString64Bytes oldId, FixedString64Bytes newId)
        {
            OnCardIdUpdated?.Invoke(newId.ToString());
        }

        private void OnOwnerChangedCallback(ulong oldOwner, ulong newOwner)
        {
            OnOwnerUpdated?.Invoke(newOwner);
        }

        private void OnCardStateChangedCallback(CardNetworkState oldState, CardNetworkState newState)
        {
            OnStateUpdated?.Invoke(newState);
        }
    }

    /// <summary>
    /// Represents the network state of a card.
    /// </summary>
    public enum CardNetworkState
    {
        InDeck,
        InHand,
        InPlay,
        Discarded,
        Destroyed
    }
}
