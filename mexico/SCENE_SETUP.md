# Scene Setup Guide

## Game Configuration

**Card Distribution:**
- Total cards in game: **32 cards**
- Players: **3**
- Cards per player: **10 cards**
- Talon (face down): **2 cards**

## Scene Hierarchy

```
CardGameScene
├── GameManager
│   └── GameSetup (Component: GameSetup.cs)
├── NetworkManager
│   ├── NetworkManager (Unity Netcode)
│   └── CardGameNetworkManager (Component: CardGameNetworkManager.cs)
├── PlayArea
│   ├── Table
│   ├── PlayerPositions
│   │   ├── Player1Position
│   │   │   ├── HandArea
│   │   │   └── PlayArea
│   │   ├── Player2Position
│   │   │   ├── HandArea
│   │   │   └── PlayArea
│   │   └── Player3Position
│   │       ├── HandArea
│   │       └── PlayArea
│   └── Center
│       ├── Talon (2 face-down cards)
│       └── DiscardPile
├── UI
│   ├── Canvas
│   │   ├── PlayerUI
│   │   │   ├── HealthDisplay
│   │   │   ├── ManaDisplay
│   │   │   └── TurnIndicator
│   │   ├── GameUI
│   │   │   ├── EndTurnButton
│   │   │   └── GameStateText
│   │   └── MatchmakingUI
│   │       ├── HostButton
│   │       ├── JoinButton
│   │       └── AddressInput
│   └── EventSystem
├── Camera
│   └── Main Camera
└── Lighting
    ├── Directional Light
    └── Environment
```

## Setup Instructions

### 1. Create Game Manager

1. Create empty GameObject named "GameManager"
2. Add `GameSetup` component
3. Configure settings:
   - Total Cards: 32
   - Cards Per Player: 10
   - Remaining Cards: 2
   - Player Count: 3
4. Assign Card Prefab
5. Create and assign spawn positions (see below)

### 2. Set Up Player Positions

Create three player positions arranged in a triangle:

**Player 1 (Bottom/Local Player):**
- Position: (0, 0, 0)
- Rotation: (0, 0, 0)
- Create child "HandArea" at (0, -2, 0)
- Create child "PlayArea" at (0, 0, 0)

**Player 2 (Top Left):**
- Position: (-5, 0, 5)
- Rotation: (0, 60, 0)
- Create child "HandArea" at local (0, -2, 0)
- Create child "PlayArea" at local (0, 0, 0)

**Player 3 (Top Right):**
- Position: (5, 0, 5)
- Rotation: (0, -60, 0)
- Create child "HandArea" at local (0, -2, 0)
- Create child "PlayArea" at local (0, 0, 0)

### 3. Create Center Area

**Talon Position:**
- Position: (0, 0, 3)
- This holds the 2 face-down cards
- Cards are not visible to any player

**Discard Pile:**
- Position: (1, 0, 3)
- Visible to all players

### 4. Assign Positions to GameSetup

In the GameSetup component:
- Player 1 Hand Position → Player1Position/HandArea
- Player 2 Hand Position → Player2Position/HandArea
- Player 3 Hand Position → Player3Position/HandArea
- Talon Position → Center/Talon

### 5. Use Imported Card Asset

Using the free Playing Cards asset pack:

1. **Choose a Card Prefab:**
   - Navigate to `Assets/Asset_PlayingCards/Prefabs/Deck01/`
   - Choose any card prefab (e.g., `Deck01_Heart_A.prefab`)
   - These already have proper 3D models and textures

2. **Add Required Components:**
   - Drag the chosen prefab into the scene
   - Add your custom components:
     - `Card` (your script)
     - `NetworkObject` (for networking)
     - `NetworkCard` (for network sync)
   - Apply changes back to prefab or save as a new prefab variant

3. **Create a Card Template:**
   - Option A: Use one of the existing prefabs and add your scripts
   - Option B: Create a prefab variant in `Assets/_Project/Prefabs/`
   - Assign to GameSetup's "Card Prefab" field

**Note:** The asset pack includes individual card prefabs (Deck01_Heart_A, Deck01_Spade_K, etc.). You can use any as your base template - your Card script will handle the card data/logic.

### 6. Camera Setup

Position main camera to view all three player positions:
- Position: (0, 10, 0)
- Rotation: (90, 0, 0)
- Field of View: 60

Or use an angled view:
- Position: (0, 8, -6)
- Rotation: (45, 0, 0)
- Field of View: 60

### 7. UI Setup

Create Canvas with:

**Player UI:**
- Health/Mana displays for local player
- Turn indicator

**Game UI:**
- End Turn button
- Game state text
- Card count displays

**Matchmaking UI:**
- Host/Join buttons
- Server address input

## Card Distribution Logic

The `GameSetup.cs` script handles card distribution:

1. **Deck Creation:** Creates 32 cards from CardDatabase
2. **Shuffling:** Uses Fisher-Yates algorithm for random distribution
3. **Distribution:**
   - Round-robin dealing: Each player gets 1 card in turn
   - Continues for 10 rounds (each player receives 10 cards)
   - Final 2 cards go to talon (face down)

## Network Synchronization

When using networking:

1. Only the **server** should call `GameSetup.SetupGame()`
2. Card objects need `NetworkObject` component
3. Cards are spawned on server and replicated to clients
4. Talon cards are set as face-down (hidden from all players)

## Testing Without Network

To test scene setup without networking:

1. Add GameSetup component to scene
2. Assign all required references
3. In Unity editor, select GameManager
4. In Inspector, click "Setup Game" (add a button in the script if needed)
5. Verify cards are distributed correctly:
   - 10 cards per player position
   - 2 cards at talon position

## Script Integration

Call `SetupGame()` from GameManager when game starts:

```csharp
public class GameManager : MonoBehaviour
{
    private GameSetup _gameSetup;
    
    private void Start()
    {
        _gameSetup = GetComponent<GameSetup>();
        
        if (IsServer) // In networked game
        {
            _gameSetup.SetupGame();
        }
    }
}
```

## Visual Layout

```
      Player 2              Player 3
        (10)                  (10)
         
         
              Talon  Discard
               (2)     
         
         
           Player 1 (Local)
              (10)
```

Numbers in parentheses indicate card count.

---

This scene structure provides a clear, organized layout for a 3-player card game with proper card distribution and positioning.
