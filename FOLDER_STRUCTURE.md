# Unity Multiplayer Card Game - Folder Structure

## Recommended Unity Project Structure

```
Assets/
├── _Project/                          # Main project folder
│   ├── Scenes/                        # All game scenes
│   │   ├── Main.unity                 # Main menu scene
│   │   ├── Game.unity                 # Main gameplay scene
│   │   ├── DeckBuilder.unity          # Deck building scene
│   │   └── Lobby.unity                # Matchmaking/lobby scene
│   │
│   ├── Scripts/                       # All C# scripts
│   │   ├── Cards/                     # Card-related scripts
│   │   │   ├── Card.cs                # Base card class
│   │   │   ├── CardData.cs            # Card data ScriptableObject
│   │   │   ├── CardVisual.cs          # Card visual representation
│   │   │   ├── CardEffect.cs          # Card effect system
│   │   │   └── CardDatabase.cs        # Card collection manager
│   │   │
│   │   ├── Game/                      # Core game logic
│   │   │   ├── GameManager.cs         # Main game controller
│   │   │   ├── TurnManager.cs         # Turn-based system
│   │   │   ├── GameState.cs           # Game state machine
│   │   │   └── RuleEngine.cs          # Game rules enforcement
│   │   │
│   │   ├── Player/                    # Player-related scripts
│   │   │   ├── Player.cs              # Player base class
│   │   │   ├── PlayerHand.cs          # Hand management
│   │   │   ├── PlayerDeck.cs          # Deck management
│   │   │   └── PlayerStats.cs         # Health, mana, etc.
│   │   │
│   │   ├── Network/                   # Networking scripts
│   │   │   ├── NetworkManager.cs      # Network manager
│   │   │   ├── NetworkPlayer.cs       # Networked player
│   │   │   ├── NetworkCard.cs         # Networked card sync
│   │   │   ├── MatchmakingManager.cs  # Matchmaking logic
│   │   │   └── LobbyManager.cs        # Lobby system
│   │   │
│   │   ├── UI/                        # UI scripts
│   │   │   ├── MainMenu.cs            # Main menu controller
│   │   │   ├── GameHUD.cs             # In-game HUD
│   │   │   ├── DeckBuilderUI.cs       # Deck builder interface
│   │   │   ├── CardTooltip.cs         # Card hover tooltip
│   │   │   └── SettingsMenu.cs        # Settings panel
│   │   │
│   │   ├── Managers/                  # Singleton managers
│   │   │   ├── AudioManager.cs        # Audio controller
│   │   │   ├── UIManager.cs           # UI navigation
│   │   │   └── SaveManager.cs         # Save/load system
│   │   │
│   │   └── Utilities/                 # Helper scripts
│   │       ├── Singleton.cs           # Singleton pattern
│   │       ├── ObjectPool.cs          # Object pooling
│   │       └── Extensions.cs          # C# extensions
│   │
│   ├── Prefabs/                       # Prefabs
│   │   ├── Cards/                     # Card prefabs
│   │   │   ├── CardTemplate.prefab    # Base card prefab
│   │   │   └── CardBack.prefab        # Card back visual
│   │   │
│   │   ├── UI/                        # UI prefabs
│   │   │   ├── CardSlot.prefab        # Card slot UI
│   │   │   ├── PlayerPanel.prefab     # Player info panel
│   │   │   └── DamagePopup.prefab     # Damage number popup
│   │   │
│   │   ├── Effects/                   # VFX prefabs
│   │   │   ├── CardPlayEffect.prefab
│   │   │   ├── DamageEffect.prefab
│   │   │   └── WinEffect.prefab
│   │   │
│   │   └── Managers/                  # Manager prefabs
│   │       ├── GameManager.prefab
│   │       └── NetworkManager.prefab
│   │
│   ├── Data/                          # ScriptableObjects
│   │   ├── Cards/                     # Card data
│   │   │   ├── Creatures/            # Creature cards
│   │   │   ├── Spells/               # Spell cards
│   │   │   └── Enchantments/         # Enchantment cards
│   │   │
│   │   ├── Decks/                     # Deck presets
│   │   │   ├── StarterDeck1.asset
│   │   │   └── StarterDeck2.asset
│   │   │
│   │   └── Config/                    # Game configuration
│   │       ├── GameSettings.asset
│   │       └── NetworkSettings.asset
│   │
│   ├── Art/                           # Art assets
│   │   ├── Cards/                     # Card artwork
│   │   │   ├── Creatures/
│   │   │   ├── Spells/
│   │   │   └── CardFrame.png
│   │   │
│   │   ├── UI/                        # UI sprites
│   │   │   ├── Buttons/
│   │   │   ├── Panels/
│   │   │   └── Icons/
│   │   │
│   │   ├── Backgrounds/               # Background images
│   │   │   ├── MainMenu.jpg
│   │   │   └── GameBoard.jpg
│   │   │
│   │   └── Effects/                   # Effect sprites
│   │       └── Particles/
│   │
│   ├── Audio/                         # Audio files
│   │   ├── Music/                     # Background music
│   │   │   ├── MainMenu.mp3
│   │   │   └── GamePlay.mp3
│   │   │
│   │   └── SFX/                       # Sound effects
│   │       ├── CardPlay.wav
│   │       ├── CardDraw.wav
│   │       ├── Damage.wav
│   │       └── Victory.wav
│   │
│   ├── Animations/                    # Animation files
│   │   ├── CardAnimations/
│   │   │   ├── CardDraw.anim
│   │   │   ├── CardPlay.anim
│   │   │   └── CardHover.anim
│   │   │
│   │   └── Controllers/               # Animator controllers
│   │       └── CardAnimator.controller
│   │
│   ├── Materials/                     # Materials
│   │   ├── CardMaterial.mat
│   │   └── UIGlowMaterial.mat
│   │
│   ├── Fonts/                         # Font files
│   │   └── GameFont.ttf
│   │
│   └── Resources/                     # Runtime-loaded assets
│       ├── CardTemplates/
│       └── ConfigFiles/
│
├── Plugins/                           # Third-party plugins
│   ├── Mirror/                        # (if using Mirror)
│   └── DOTween/                       # (if using DOTween)
│
├── StreamingAssets/                   # Platform-specific assets
│
└── TextMesh Pro/                      # TextMeshPro assets (auto-generated)
```

## Key Folder Notes

### _Project Folder
- Contains all project-specific assets
- Leading underscore keeps it at the top of Assets folder
- Makes it easy to export/import project assets

### Scripts Organization
- Organized by feature/responsibility
- Each major system has its own folder
- Utilities folder for shared helpers
- Avoid deep nesting (max 3 levels recommended)

### Prefabs Organization
- Categorized by type (Cards, UI, Effects)
- Manager prefabs for scene-independent objects
- Network prefabs separate from local-only prefabs

### Data Folder (ScriptableObjects)
- All game data stored as ScriptableObjects
- Easy to modify without code changes
- Version control friendly
- Great for balancing and tweaking

### Art Organization
- Separate folders for different asset types
- Card art organized by card type
- UI elements kept separate from gameplay art

### Resources Folder
Use sparingly! Only for assets that must be loaded at runtime via `Resources.Load()`

## Git Ignore Recommendations

Make sure your `.gitignore` includes:
```
# Unity generated
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
Assets/AssetStoreTools*

# Visual Studio cache
.vs/

# Rider
.idea/

# User settings
*.csproj
*.unityproj
*.sln
*.suo
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb

# Unity3D generated meta files
*.pidb.meta
*.pdb.meta
*.mdb.meta

# Crash reports
sysinfo.txt
*.stackdump
```

## Next Steps

1. Create Unity project (Unity Hub → New Project → 3D or 2D template)
2. Create the folder structure shown above
3. Install required packages:
   - Mirror (Asset Store) or Netcode for GameObjects (Package Manager)
   - TextMesh Pro (Package Manager → Import TMP Essentials)
   - DOTween (Asset Store) - optional but recommended
4. Set up Git repository with proper .gitignore
5. Start with Phase 1 of PROJECT_PLAN.md
