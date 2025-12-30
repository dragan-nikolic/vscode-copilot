using UnityEngine;
using Mirror;

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
        [SyncVar(hook = nameof(OnCardIdChanged))]
        private string _cardId;
        
        [SyncVar(hook = nameof(OnOwnerChanged))]
        private uint _ownerId;
        
        [SyncVar(hook = nameof(OnCardStateChanged))]
        private CardNetworkState _cardState = CardNetworkState.InDeck;

        public string CardId => _cardId;
        public uint OwnerId => _ownerId;
        public CardNetworkState CardState => _cardState;

        // Events
        public event System.Action<string> OnCardIdUpdated;
        public event System.Action<uint> OnOwnerUpdated;
        public event System.Action<CardNetworkState> OnStateUpdated;

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            if (_card != null && _card.Data != null)
            {
                _cardId = _card.Data.CardId;
            }
            
            Debug.Log($"[NetworkCard] Server initialized card {netId} - CardID: {_cardId}");
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Debug.Log($"[NetworkCard] Client received card {netId}");
            
            // Initialize local card with network data
            if (_card != null && !string.IsNullOrEmpty(_cardId))
            {
                // TODO: Load card data from CardDatabase using _cardId
            }
        }

        /// <summary>
        /// Initializes the network card with card data (server only).
        /// </summary>
        [Server]
        public void Initialize(Cards.CardData cardData, uint ownerNetId)
        {
            if (cardData == null)
            {
                Debug.LogError("[NetworkCard] Cannot initialize with null CardData!");
                return;
            }

            _cardId = cardData.CardId;
            _ownerId = ownerNetId;
            _cardState = CardNetworkState.InDeck;

            if (_card == null)
                _card = GetComponent<Cards.Card>();

            if (_card != null)
                _card.SetCardData(cardData);

            Debug.Log($"[NetworkCard] Initialized card {_cardId} for player {_ownerId}");
        }

        /// <summary>
        /// Changes the card's network state (server only).
        /// </summary>
        [Server]
        public void SetCardState(CardNetworkState newState)
        {
            _cardState = newState;
            Debug.Log($"[NetworkCard] Card {_cardId} state changed to {newState}");
        }

        /// <summary>
        /// Plays the card on the network (command from client).
        /// </summary>
        [Command(requiresAuthority = false)]
        public void CmdPlayCard(NetworkConnectionToClient sender = null)
        {
            if (sender == null) return;

            // Verify the player owns this card
            NetworkPlayer player = sender.identity.GetComponent<NetworkPlayer>();
            if (player == null || player.netId != _ownerId)
            {
                Debug.LogWarning($"[NetworkCard] Player {sender.connectionId} tried to play card they don't own!");
                return;
            }

            // Verify it's the player's turn
            if (!player.IsPlayerTurn)
            {
                Debug.LogWarning($"[NetworkCard] Player tried to play card during opponent's turn!");
                return;
            }

            Debug.Log($"[NetworkCard] Playing card {_cardId}");
            
            // Change state and play card
            SetCardState(CardNetworkState.InPlay);
            RpcPlayCard();
        }

        [ClientRpc]
        private void RpcPlayCard()
        {
            Debug.Log($"[NetworkCard] Card {_cardId} played on all clients");
            
            if (_card != null)
            {
                _card.PlayCard();
            }
            
            // TODO: Trigger card play animation
        }

        /// <summary>
        /// Destroys the card on the network (server only).
        /// </summary>
        [Server]
        public void DestroyCard()
        {
            Debug.Log($"[NetworkCard] Destroying card {_cardId}");
            SetCardState(CardNetworkState.Destroyed);
            RpcDestroyCard();
            
            // Destroy after a short delay to allow animations
            Invoke(nameof(DestroyNetworkObject), 0.5f);
        }

        [ClientRpc]
        private void RpcDestroyCard()
        {
            Debug.Log($"[NetworkCard] Card {_cardId} destroyed on all clients");
            
            if (_card != null)
            {
                _card.DestroyCard();
            }
        }

        [Server]
        private void DestroyNetworkObject()
        {
            NetworkServer.Destroy(gameObject);
        }

        // SyncVar Hooks
        private void OnCardIdChanged(string oldId, string newId)
        {
            OnCardIdUpdated?.Invoke(newId);
        }

        private void OnOwnerChanged(uint oldOwner, uint newOwner)
        {
            OnOwnerUpdated?.Invoke(newOwner);
        }

        private void OnCardStateChanged(CardNetworkState oldState, CardNetworkState newState)
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
