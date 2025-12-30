# Unity Multiplayer Card Game - Setup Guide

## Getting Started

Welcome to the Unity Multiplayer Card Game project! Follow these steps to get the project up and running.

## Prerequisites

- **Unity 2021.3 LTS or newer** (recommended: Unity 2022.3 LTS)
- **Git** for version control
- **Visual Studio** or **Visual Studio Code** with C# extension
- **Mirror** or **Unity Netcode for GameObjects** (to be installed)

## Project Structure

The project follows a modular structure under `Assets/_Project/`:

```
Assets/_Project/
├── Scenes/          # Unity scenes
├── Scripts/         # All C# code
│   ├── Cards/       # Card-related logic
│   ├── Game/        # Core game systems
│   ├── Player/      # Player management
│   ├── Network/     # Networking code
│   ├── UI/          # User interface
│   ├── Managers/    # Singleton managers
│   └── Utilities/   # Helper classes
├── Prefabs/         # Reusable game objects
├── Data/            # ScriptableObjects
├── Art/             # Sprites and textures
├── Audio/           # Music and sound effects
├── Materials/       # Unity materials
└── Animations/      # Animation clips
```

## Initial Setup Steps

### 1. Open the Project in Unity

1. Launch Unity Hub
2. Click "Add" and select this project folder
3. Open the project with Unity 2021.3 LTS or newer

### 2. Install Required Packages

Open the Package Manager (Window > Package Manager) and install:

#### Essential Packages:
- **TextMesh Pro** (Unity Registry)
- **Input System** (Unity Registry) - if using new input system

#### Networking Solution (Choose one):

**Option A: Mirror (Recommended for beginners)**
```
1. Window > Package Manager
2. Click "+" > Add package from git URL
3. Enter: https://github.com/MirrorNetworking/Mirror.git
```

**Option B: Unity Netcode for GameObjects**
```
1. Window > Package Manager
2. Unity Registry > Search "Netcode for GameObjects"
3. Install
```

#### Optional but Recommended:
- **DOTween** (Asset Store) - For smooth animations
- **Addressables** (Unity Registry) - For asset management

### 3. Import TextMesh Pro Essentials

1. Window > TextMesh Pro > Import TMP Essential Resources
2. Click "Import"

### 4. Configure Project Settings

#### Player Settings:
1. Edit > Project Settings > Player
2. Set Company Name and Product Name
3. Under "Other Settings":
   - Scripting Backend: IL2CPP (for production builds)
   - API Compatibility Level: .NET Standard 2.1

#### Quality Settings:
1. Edit > Project Settings > Quality
2. Adjust quality levels as needed for your target platform

### 5. Set Up Scenes

Create the basic scenes in `Assets/_Project/Scenes/`:

1. **Main.unity** - Main menu scene
2. **Game.unity** - Main gameplay scene
3. **Lobby.unity** - Matchmaking/lobby scene

To create a scene:
1. File > New Scene
2. File > Save As...
3. Save in `Assets/_Project/Scenes/`

### 6. Configure Build Settings

1. File > Build Settings
2. Add scenes in this order:
   - Main.unity
   - Lobby.unity
   - Game.unity
3. Select target platform (PC, Mac, Linux Standalone recommended for development)

## Core Scripts Overview

### Already Created:

#### `Singleton.cs`
Generic singleton pattern for manager classes. Use for global systems like AudioManager, GameManager, etc.

#### `GameManager.cs`
Main game controller that manages game state and flow. Handles transitions between menu, lobby, and gameplay.

#### `TurnManager.cs`
Manages the turn-based system. Controls whose turn it is and handles turn timers.

#### `CardData.cs`
ScriptableObject for storing card data. Create cards via: Right-click > Create > Card Game > Card Data.

#### `Card.cs`
Represents a card instance in the game. Handles card logic and effects.

#### `AudioManager.cs`
Controls all audio in the game. Manages music and sound effects.

## Creating Your First Card

1. In Project window, navigate to `Assets/_Project/Data/Cards/Creatures/`
2. Right-click > Create > Card Game > Card Data
3. Name it (e.g., "FireElemental")
4. In Inspector, fill in:
   - Card Name
   - Description
   - Card Type: Creature
   - Mana Cost
   - Attack and Health values
   - Add artwork (optional for now)

## Next Steps

### Phase 1 Tasks (Current):
- [x] Create folder structure
- [x] Set up core scripts
- [x] Create assembly definitions
- [ ] Install networking solution (Mirror or Netcode)
- [ ] Create basic scene setup
- [ ] Test basic game manager functionality

### Coming Up in Phase 2:
- Build card visual prefab
- Implement card UI display
- Create card hover/selection mechanics
- Build card database system

## Troubleshooting

### Missing References?
- Make sure all scripts are in the correct folders
- Check that namespaces match: `CardGame.*`
- Reimport scripts: Right-click folder > Reimport

### Compilation Errors?
- Ensure Unity version is 2021.3 LTS or newer
- Check that .NET Standard 2.1 is selected in Player Settings
- Close and reopen Unity if assembly definitions aren't recognized

### Git Issues?
- Make sure `.gitignore` is in the root folder
- Don't commit `Library/`, `Temp/`, or `Logs/` folders
- Use Git LFS for large binary files (recommended for Unity projects)

## Resources

- [Unity Manual](https://docs.unity3d.com/Manual/index.html)
- [Mirror Networking Docs](https://mirror-networking.gitbook.io/)
- [Unity Netcode Docs](https://docs-multiplayer.unity3d.com/)
- [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/)

## Development Tips

1. **Test Early and Often**: Run the game frequently to catch issues early
2. **Use Version Control**: Commit your changes regularly
3. **Comment Your Code**: Future you will thank present you
4. **Prefab Everything**: Make reusable prefabs for cards, UI elements, etc.
5. **Profile Performance**: Use Unity Profiler to identify bottlenecks

## Support

If you run into issues:
1. Check the console for error messages
2. Review the relevant script documentation
3. Consult Unity documentation
4. Check the project plan in `PROJECT_PLAN.md`

---

Happy coding! 🎮✨
