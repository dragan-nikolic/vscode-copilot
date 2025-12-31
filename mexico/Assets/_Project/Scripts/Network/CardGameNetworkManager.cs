using UnityEngine;
using Unity.Netcode;

namespace CardGame.Network
{
    /// <summary>
    /// Custom Network Manager for the card game.
    /// Handles client-server connections and game initialization.
    /// </summary>
    public class CardGameNetworkManager : MonoBehaviour
    {
        public static CardGameNetworkManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private int _minPlayers = 2;
        [SerializeField] private int _maxPlayers = 2;

        [Header("Spawn Points")]
        [SerializeField] private Transform[] _playerSpawnPoints;

        [Header("Player Prefab")]
        [SerializeField] private GameObject _playerPrefab;

        private int _connectedPlayers = 0;

        public int ConnectedPlayers => _connectedPlayers;
        public int MaxPlayers => _maxPlayers;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
                NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            }
        }

        private void OnServerStarted()
        {
            Debug.Log("[NetworkManager] Server started");
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            if (_connectedPlayers >= _maxPlayers)
            {
                response.Approved = false;
                response.Reason = "Server is full";
                Debug.LogWarning($"[NetworkManager] Player rejected - game is full ({_connectedPlayers}/{_maxPlayers})");
                return;
            }

            response.Approved = true;
            response.CreatePlayerObject = true;
            response.Position = GetStartPosition().position;
            response.Rotation = GetStartPosition().rotation;

            Debug.Log($"[NetworkManager] Player connection approved");
        }

        private void OnClientConnected(ulong clientId)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                _connectedPlayers++;
                Debug.Log($"[NetworkManager] Player connected. Total players: {_connectedPlayers}/{_maxPlayers}");

                // Check if we have enough players to start the game
                if (_connectedPlayers >= _minPlayers)
                {
                    CheckGameStart();
                }
            }

            if (NetworkManager.Singleton.IsClient && clientId == NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log("[NetworkManager] Successfully connected to server");
            }
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                _connectedPlayers--;
                Debug.Log($"[NetworkManager] Player disconnected. Total players: {_connectedPlayers}/{_maxPlayers}");
            }

            if (NetworkManager.Singleton.IsClient && clientId == NetworkManager.Singleton.LocalClientId)
            {
                Debug.Log("[NetworkManager] Disconnected from server");
            }
        }

        private void CheckGameStart()
        {
            if (_connectedPlayers >= _minPlayers)
            {
                Debug.Log($"[NetworkManager] Minimum players reached ({_connectedPlayers}/{_minPlayers}). Game can start!");
                // Signal game start
                GameReadyClientRpc();
            }
        }

        [Unity.Netcode.Rpc(SendTo.Everyone)]
        private void GameReadyClientRpc()
        {
            Debug.Log("[NetworkManager] Game is ready to start!");
            
            if (Game.GameManager.Instance != null)
            {
                Game.GameManager.Instance.ChangeState(Game.GameState.GameStarting);
            }
        }

        private Transform GetStartPosition()
        {
            if (_playerSpawnPoints == null || _playerSpawnPoints.Length == 0)
                return transform;

            // Get spawn point based on connected players count
            int spawnIndex = _connectedPlayers % _playerSpawnPoints.Length;
            return _playerSpawnPoints[spawnIndex];
        }

        /// <summary>
        /// Starts the server for hosting a game.
        /// </summary>
        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("[NetworkManager] Started as Host");
        }

        /// <summary>
        /// Starts the server only (no local player).
        /// </summary>
        public void StartServer()
        {
            NetworkManager.Singleton.StartServer();
            Debug.Log("[NetworkManager] Started as Server");
        }

        /// <summary>
        /// Connects as a client to the specified address.
        /// </summary>
        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("[NetworkManager] Connecting to server");
        }

        /// <summary>
        /// Stops the network manager (server or client).
        /// </summary>
        public void StopNetwork()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
                Debug.Log("[NetworkManager] Network stopped");
            }
        }
    }
}
