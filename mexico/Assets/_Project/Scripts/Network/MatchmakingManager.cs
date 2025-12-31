using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

namespace CardGame.Network
{
    /// <summary>
    /// Manages matchmaking and lobby functionality.
    /// </summary>
    public class MatchmakingManager : Managers.Singleton<MatchmakingManager>
    {
        [Header("Matchmaking Settings")]
        [SerializeField] private string _serverAddress = "127.0.0.1";
        [SerializeField] private ushort _serverPort = 7777;

        [Header("Lobby State")]
        private bool _isSearching = false;
        private bool _isInLobby = false;
        private List<string> _lobbyPlayers = new List<string>();

        public bool IsSearching => _isSearching;
        public bool IsInLobby => _isInLobby;
        public List<string> LobbyPlayers => new List<string>(_lobbyPlayers);

        // Events
        public event System.Action OnMatchmakingStarted;
        public event System.Action OnMatchFound;
        public event System.Action OnMatchmakingCanceled;
        public event System.Action<string> OnPlayerJoinedLobby;
        public event System.Action<string> OnPlayerLeftLobby;

        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Starts searching for a match.
        /// </summary>
        public void StartMatchmaking()
        {
            if (_isSearching)
            {
                Debug.LogWarning("[MatchmakingManager] Already searching for a match!");
                return;
            }

            Debug.Log("[MatchmakingManager] Starting matchmaking...");
            _isSearching = true;
            OnMatchmakingStarted?.Invoke();

            // For now, simply connect to server
            // TODO: Implement proper matchmaking service
            ConnectToServer();
        }

        /// <summary>
        /// Cancels matchmaking search.
        /// </summary>
        public void CancelMatchmaking()
        {
            if (!_isSearching)
            {
                Debug.LogWarning("[MatchmakingManager] Not currently searching!");
                return;
            }

            Debug.Log("[MatchmakingManager] Canceling matchmaking...");
            _isSearching = false;
            OnMatchmakingCanceled?.Invoke();

            // Disconnect if connected
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }

        /// <summary>
        /// Creates a new lobby as host.
        /// </summary>
        public void CreateLobby()
        {
            Debug.Log("[MatchmakingManager] Creating lobby...");
            
            if (CardGameNetworkManager.Instance != null)
            {
                CardGameNetworkManager.Instance.StartHost();
                _isInLobby = true;
                OnMatchFound?.Invoke();
            }
            else
            {
                Debug.LogError("[MatchmakingManager] CardGameNetworkManager not found!");
            }
        }

        /// <summary>
        /// Joins an existing lobby.
        /// </summary>
        public void JoinLobby(string address = null)
        {
            if (!string.IsNullOrEmpty(address))
            {
                _serverAddress = address;
            }

            Debug.Log($"[MatchmakingManager] Joining lobby at {_serverAddress}...");
            ConnectToServer();
        }

        /// <summary>
        /// Leaves the current lobby.
        /// </summary>
        public void LeaveLobby()
        {
            if (!_isInLobby)
            {
                Debug.LogWarning("[MatchmakingManager] Not in a lobby!");
                return;
            }

            Debug.Log("[MatchmakingManager] Leaving lobby...");
            
            if (CardGameNetworkManager.Instance != null)
            {
                CardGameNetworkManager.Instance.StopNetwork();
            }

            _isInLobby = false;
            _lobbyPlayers.Clear();
        }

        private void ConnectToServer()
        {
            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("[MatchmakingManager] NetworkManager not found!");
                return;
            }

            // Set up transport with server address
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            if (transport != null)
            {
                transport.SetConnectionData(_serverAddress, _serverPort);
            }

            if (CardGameNetworkManager.Instance != null)
            {
                CardGameNetworkManager.Instance.StartClient();
                _isInLobby = true;
                OnMatchFound?.Invoke();
            }
            else
            {
                Debug.LogError("[MatchmakingManager] CardGameNetworkManager not found!");
            }
        }

        /// <summary>
        /// Sets the server address for matchmaking.
        /// </summary>
        public void SetServerAddress(string address)
        {
            _serverAddress = address;
            Debug.Log($"[MatchmakingManager] Server address set to {_serverAddress}");
        }

        /// <summary>
        /// Gets the current server address.
        /// </summary>
        public string GetServerAddress()
        {
            return _serverAddress;
        }

        /// <summary>
        /// Called when a player joins the lobby.
        /// </summary>
        public void PlayerJoined(string playerName)
        {
            if (!_lobbyPlayers.Contains(playerName))
            {
                _lobbyPlayers.Add(playerName);
                Debug.Log($"[MatchmakingManager] Player joined: {playerName}");
                OnPlayerJoinedLobby?.Invoke(playerName);
            }
        }

        /// <summary>
        /// Called when a player leaves the lobby.
        /// </summary>
        public void PlayerLeft(string playerName)
        {
            if (_lobbyPlayers.Remove(playerName))
            {
                Debug.Log($"[MatchmakingManager] Player left: {playerName}");
                OnPlayerLeftLobby?.Invoke(playerName);
            }
        }

        /// <summary>
        /// Quick play - creates or joins first available game.
        /// </summary>
        public void QuickPlay()
        {
            Debug.Log("[MatchmakingManager] Quick Play - creating/joining game...");
            
            // For now, just create a new lobby
            // TODO: Implement proper quick play that searches for existing games first
            CreateLobby();
        }
    }
}
