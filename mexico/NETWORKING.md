# Mirror Networking Setup Guide

## Installation

1. **Install Mirror from Git URL**:
   - Open Unity
   - Window > Package Manager
   - Click "+" > Add package from git URL
   - Enter: `https://github.com/MirrorNetworking/Mirror.git`
   - Wait for installation to complete

## Created Network Scripts

### Core Networking

#### `CardGameNetworkManager.cs`
- Custom NetworkManager extending Mirror's NetworkManager
- Handles player connections/disconnections
- Manages game start conditions (min/max players)
- Controls host/server/client modes

**Key Features:**
- Max 2 players per game
- Auto-detects when game is ready to start
- Spawn point management
- Connection validation

#### `NetworkPlayer.cs`
- Represents a networked player
- Synchronizes player state (health, mana, turn)
- Handles player actions (play card, end turn)
- Server-authoritative validation

**SyncVars:**
- `_playerName` - Player display name
- `_playerHealth` - Current health
- `_playerMana` - Current mana
- `_isPlayerTurn` - Turn state

#### `NetworkCard.cs`
- Synchronizes card state across network
- Handles card ownership and validation
- Manages card lifecycle (deck → hand → play → discard)
- Server-authoritative card actions

**SyncVars:**
- `_cardId` - Unique card identifier
- `_ownerId` - Network ID of owning player
- `_cardState` - Current card state (InDeck, InHand, InPlay, etc.)

#### `MatchmakingManager.cs`
- Manages matchmaking and lobby functionality
- Creates/joins lobbies
- Handles quick play
- Tracks lobby players

## Setup Instructions

### 1. Create NetworkManager GameObject

In your main game scene:

1. Create empty GameObject named "NetworkManager"
2. Add `CardGameNetworkManager` component
3. Configure settings:
   - Network Address: localhost (for testing)
   - Max Players: 2
   - Transport: Use default KCP Transport (comes with Mirror)

### 2. Create Player Prefab

1. Create a new GameObject named "NetworkPlayer"
2. Add components:
   - `NetworkIdentity` (Mirror component)
   - `NetworkPlayer` (your script)
3. Save as prefab in `Assets/_Project/Prefabs/`
4. Assign to NetworkManager's "Player Prefab" field

### 3. Configure Spawn Points

1. Create 2 empty GameObjects for spawn positions
2. Position them on opposite sides of the play area
3. Assign to CardGameNetworkManager's "Player Spawn Points" array

### 4. Setup Card Prefabs

For each card prefab:
1. Add `NetworkIdentity` component
2. Add `NetworkCard` component
3. Ensure `Card` component is attached

## Network Architecture

### Server-Authoritative Model
- **Server** validates all game actions
- **Clients** send commands (CmdPlayCard, CmdEndTurn)
- **Server** processes and broadcasts results (RpcCardPlayed, RpcHealthChanged)

### State Synchronization
- **SyncVars** automatically sync from server to clients
- **Hooks** trigger events when SyncVars change
- **RPCs** send method calls across network

## Testing Locally

### Method 1: Host + Client
1. Build your game
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
2. Create a clone project
3. Run both Unity instances simultaneously
4. One hosts, one joins

## Usage Examples

### Starting a Game

```csharp
// As Host (server + local player)
CardGameNetworkManager networkManager = FindFirstObjectByType<CardGameNetworkManager>();
networkManager.StartHost();

// As Client
networkManager.StartClient("192.168.1.100"); // Server IP
```

### Using Matchmaking Manager

```csharp
// Quick Play
MatchmakingManager.Instance.QuickPlay();

// Create Lobby
MatchmakingManager.Instance.CreateLobby();

// Join Specific Lobby
MatchmakingManager.Instance.JoinLobby("192.168.1.100");

// Leave Lobby
MatchmakingManager.Instance.LeaveLobby();
```

### Playing a Card

```csharp
// On the client
NetworkCard networkCard = cardObject.GetComponent<NetworkCard>();
networkCard.CmdPlayCard();

// Server validates and broadcasts to all clients
```

## Common Issues & Solutions

### "NetworkIdentity not found"
- Add `NetworkIdentity` component to all networked prefabs

### "Authority not allowed"
- Check that commands are called from the local player
- Ensure NetworkIdentity has correct authority settings

### "Server not responding"
- Check firewall settings
- Verify network address is correct
- Ensure Transport is properly configured

### "SyncVar not updating"
- Only server can modify SyncVars directly
- Use [Server] methods or Commands to update values

## Next Steps

1. ✅ Install Mirror package
2. ✅ Create network scripts
3. ⬜ Set up NetworkManager in scene
4. ⬜ Create player prefab
5. ⬜ Test basic connection
6. ⬜ Implement card spawning on network
7. ⬜ Test full gameplay loop

## Resources

- [Mirror Documentation](https://mirror-networking.gitbook.io/)
- [Mirror Discord](https://discord.gg/N9QVxbM)
- [Mirror Examples](https://github.com/MirrorNetworking/Mirror/tree/master/Assets/Mirror/Examples)

---

All networking scripts follow Mirror's best practices and are ready to use!
