# Unity Netcode for GameObjects Setup Guide

## Installation

1. **Install Unity Netcode from Package Manager**:
   - Open Unity
   - Window > Package Manager
   - Click "+" > Add package by name
   - Enter: `com.unity.netcode.gameobjects`
   - Click Add
   - Wait for installation to complete

## Created Network Scripts

### Core Networking

#### `CardGameNetworkManager.cs`
- Custom manager that works with Unity's NetworkManager
- Handles player connections/disconnections
- Manages game start conditions (min/max players)
- Controls host/server/client modes
- Uses connection approval for validation

**Key Features:**
- Max 3 players per game
- Auto-detects when game is ready to start
- Spawn point management
- Connection validation via approval callback

#### `NetworkPlayer.cs`
- Represents a networked player
- Synchronizes player state (name, turn, bid) using NetworkVariables
- Handles player actions via ServerRPCs
- Server-authoritative validation

**NetworkVariables:**
- `_playerName` - Player display name
- `_isPlayerTurn` - Turn state
- `_currentBid` - Current bid amount
- `_hasPassed` - Whether player has passed

#### `NetworkCard.cs`
- Synchronizes card state across network using NetworkVariables
- Handles card ownership and validation
- Manages card lifecycle (deck → hand → play → discard)
- Server-authoritative card actions

**NetworkVariables:**
- `_cardId` - Unique card identifier
- `_ownerId` - Network ID of owning player
- `_cardState` - Current card state (InDeck, InHand, InPlay, etc.)

#### `MatchmakingManager.cs`
- Manages matchmaking and lobby functionality
- Creates/joins lobbies
- Handles quick play
- Tracks lobby players
- Uses Unity Transport (UTP) for connections

## Setup Instructions

### 1. Create NetworkManager GameObject

In your main game scene:

1. Create empty GameObject named "NetworkManager"
2. Add Unity's `NetworkManager` component
3. Add your `CardGameNetworkManager` component
4. Configure NetworkManager settings:
   - Transport: Unity Transport (UTP) - automatically added
   - Player Prefab: Assign your player prefab (see step 2)
5. Configure CardGameNetworkManager:
   - Player Prefab: Assign your player prefab
   - Min/Max Players: 3
   - Player Spawn Points: Assign spawn transforms

### 2. Create Player Prefab

1. Create a new GameObject named "NetworkPlayer"
2. Add components:
   - `NetworkObject` (Unity Netcode component)
   - `NetworkPlayer` (your script)
3. Save as prefab in `Assets/_Project/Prefabs/`
4. Assign to NetworkManager's "Player Prefab" field (in NetworkManager component)
5. Enable "Spawn With Observer" on NetworkObject

### 3. Configure Spawn Points

1. Create 3 empty GameObjects for spawn positions
2. Position them around the play area (e.g., in a triangle formation)
3. Assign to CardGameNetworkManager's "Player Spawn Points" array

### 4. Setup Card Prefabs

For each card prefab:
1. Add `NetworkObject` component
2. Add `NetworkCard` component
3. Ensure `Card` component is attached
4. Register prefab in NetworkManager's "Prefabs List"

## Network Architecture

### Server-Authoritative Model
- **Server** validates all game actions
- **Clients** send ServerRPCs (PlayCardServerRpc, EndTurnServerRpc)
- **Server** processes and broadcasts results via ClientRpcs

### State Synchronization
- **NetworkVariables** automatically sync from server to clients
- **OnValueChanged callbacks** trigger events when NetworkVariables change
- **RPCs** send method calls across network (ServerRpc for server, ClientRpc for all clients)

## Testing Locally

### Method 1: Host + Client (Recommended)
1. Build your game (File > Build Settings > Build)
2. Run the built executable (acts as host)
3. Run in Unity Editor (acts as client)
4. Both connect to same game

### Method 2: Multiple Builds
1. Build your game
2. Run two instances of the built executable
3. One starts as Host
4. Other joins as Client

### Method 3: ParrelSync (Unity Editor)
1. Install ParrelSync from Package Manager
2. Create a clone project (ParrelSync > Clones Manager)
3. Run both Unity instances simultaneously
4. One hosts, one joins

## Usage Examples

### Starting a Game

```csharp
// As Host (server + local player)
CardGameNetworkManager.Instance.StartHost();

// As Server only
CardGameNetworkManager.Instance.StartServer();

// As Client
CardGameNetworkManager.Instance.StartClient();
```

### Using Matchmaking Manager

```csharp
// Quick Play
MatchmakingManager.Instance.QuickPlay();

// Create Lobby
MatchmakingManager.Instance.CreateLobby();

// Join Specific Lobby
MatchmakingManager.Instance.SetServerAddress("192.168.1.100");
MatchmakingManager.Instance.JoinLobby();

// Leave Lobby
MatchmakingManager.Instance.LeaveLobby();
```

### Playing a Card

```csharp
// On the client
NetworkCard networkCard = cardObject.GetComponent<NetworkCard>();
networkCard.PlayCardServerRpc();

// Server validates and broadcasts to all clients
```

## Common Issues & Solutions

### "NetworkObject not found"
- Add `NetworkObject` component to all networked prefabs
- Register all networked prefabs in NetworkManager's "Prefabs List"

### "Ownership not allowed"
- Check that ServerRPCs are called from the owning client
- Use `RequireOwnership = false` in ServerRpc attribute if needed
- Ensure NetworkObject has correct ownership settings

### "Server not responding"
- Check firewall settings
- Verify network address is correct in Unity Transport
- Ensure Transport is properly configured with correct IP/port

### "NetworkVariable not updating"
- Only server can modify NetworkVariables directly
- Use ServerRPCs or server-side methods to update values
- Ensure NetworkBehaviour is spawned before accessing NetworkVariables

### "Prefab not registered"
- All networked prefabs must be in NetworkManager's "Prefabs List"
- Prefabs must have NetworkObject component

## Next Steps

1. ✅ Install Unity Netcode package
2. ✅ Create network scripts
3. ⬜ Set up NetworkManager in scene
4. ⬜ Create and register player prefab
5. ⬜ Test basic connection
6. ⬜ Implement card spawning on network
7. ⬜ Test full gameplay loop

## Key Differences from Mirror

**Mirror → Unity Netcode:**
- `NetworkManager.singleton` → `NetworkManager.Singleton` (capital S)
- `NetworkBehaviour` → Same name, different namespace
- `[SyncVar]` → `NetworkVariable<T>`
- `[Command]` → `[ServerRpc]`
- `[ClientRpc]` → `[Rpc(SendTo.Everyone)]` or `[ClientRpc]`
- `netId` → `NetworkObjectId`
- `isServer` → `IsServer`
- `isClient` → `IsClient`
- `hasAuthority` → `IsOwner`
- `NetworkServer.Spawn()` → `NetworkObject.Spawn()`
- `NetworkServer.Destroy()` → `NetworkObject.Despawn()`

## Resources

- [Unity Netcode Documentation](https://docs-multiplayer.unity3d.com/)
- [Unity Netcode API Reference](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@latest)
- [Unity Netcode Samples](https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop)
- [Unity Multiplayer Community](https://discord.gg/unity)

---

All networking scripts follow Unity Netcode best practices and are ready to use!
