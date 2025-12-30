using UnityEngine;
using Mirror;

namespace CardGame.Network
{
    /// <summary>
    /// Custom Network Manager for the card game.
    /// Handles client-server connections and game initialization.
    /// </summary>
    public class CardGameNetworkManager : NetworkManager
    {
        [Header("Game Settings")]
        [SerializeField] private int _minPlayers = 2;
        [SerializeField] private int _maxPlayers = 2;

        [Header("Spawn Points")]
        [SerializeField] private Transform[] _playerSpawnPoints;

        private int _connectedPlayers = 0;

        public int ConnectedPlayers => _connectedPlayers;
        public int MaxPlayers => _maxPlayers;

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            
            if (numPlayers > _maxPlayers)
            {
                conn.Disconnect();
                Debug.LogWarning($"[NetworkManager] Player rejected - game is full ({numPlayers}/{_maxPlayers})");
                return;
            }

            _connectedPlayers++;
            Debug.Log($"[NetworkManager] Player connected. Total players: {_connectedPlayers}/{_maxPlayers}");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            _connectedPlayers--;
            Debug.Log($"[NetworkManager] Player disconnected. Total players: {_connectedPlayers}/{_maxPlayers}");
            
            base.OnServerDisconnect(conn);
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // Get spawn position
            Transform startPos = GetStartPosition();
            
            // Create player object
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            // Spawn player on the network
            NetworkServer.AddPlayerForConnection(conn, player);
            
            Debug.Log($"[NetworkManager] Player added for connection {conn.connectionId}");

            // Check if we have enough players to start the game
            if (_connectedPlayers >= _minPlayers)
            {
                CheckGameStart();
            }
        }

        private void CheckGameStart()
        {
            if (_connectedPlayers >= _minPlayers)
            {
                Debug.Log($"[NetworkManager] Minimum players reached ({_connectedPlayers}/{_minPlayers}). Game can start!");
                // Signal game start
                RpcGameReady();
            }
        }

        [ClientRpc]
        private void RpcGameReady()
        {
            Debug.Log("[NetworkManager] Game is ready to start!");
            
            if (Game.GameManager.Instance != null)
            {
                Game.GameManager.Instance.ChangeState(Game.GameState.GameStarting);
            }
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("[NetworkManager] Successfully connected to server");
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("[NetworkManager] Disconnected from server");
        }

        public override Transform GetStartPosition()
        {
            if (_playerSpawnPoints == null || _playerSpawnPoints.Length == 0)
                return base.GetStartPosition();

            // Get spawn point based on connection ID
            int spawnIndex = numPlayers % _playerSpawnPoints.Length;
            return _playerSpawnPoints[spawnIndex];
        }

        /// <summary>
        /// Starts the server for hosting a game.
        /// </summary>
        public void StartHost()
        {
            NetworkManager.singleton.StartHost();
            Debug.Log("[NetworkManager] Started as Host");
        }

        /// <summary>
        /// Starts the server only (no local player).
        /// </summary>
        public void StartServer()
        {
            NetworkManager.singleton.StartServer();
            Debug.Log("[NetworkManager] Started as Server");
        }

        /// <summary>
        /// Connects as a client to the specified address.
        /// </summary>
        public void StartClient(string address = "localhost")
        {
            networkAddress = address;
            NetworkManager.singleton.StartClient();
            Debug.Log($"[NetworkManager] Connecting to {address}");
        }

        /// <summary>
        /// Stops the network manager (server or client).
        /// </summary>
        public void StopNetwork()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient();
            }
            else if (NetworkServer.active)
            {
                NetworkManager.singleton.StopServer();
            }

            Debug.Log("[NetworkManager] Network stopped");
        }
    }
}
