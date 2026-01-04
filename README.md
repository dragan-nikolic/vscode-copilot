# vscode-copilot

The purpose of this project is to learn using github copilot in vs code.

The project is to create multiplarer card game called [Mexico](https://www.pagat.com/auctionwhist/meksiko.html) using Unity game 
engine.

## setup copilot in vs code

Install extension: GitHub Copilot.

## Testing with ParrelSync

To test the multiplayer functionality (3 players required for Mexico), we use **ParrelSync** to run multiple Unity Editor instances simultaneously.

### 1. Setup Clones
- Open the **ParrelSync** menu in the top toolbar.
- Select **Clones Manager**.
- Click **Add new clone** to create at least two clones (Clone_0 and Clone_1).

### 2. Running a 3-Player Session
1. **Main Editor (Host):**
   - Click **Play** in the Unity Editor.
   - On the Matchmaking UI, click **Host** (this instance acts as Player 1).
2. **Clone 1 (Client):**
   - Click **Open in new editor** from the Clones Manager.
   - Click **Play** in the clone window.
   - Click **Join** with the address set to `127.0.0.1` (Player 2).
3. **Clone 2 (Client):**
   - Open and Play the second clone instance.
   - Click **Join** (Player 3).

### 3. Game Initialization
- Once the 3rd player joins, the `CardGameNetworkManager` detects the minimum player count and triggers the game start.
- The server shuffles and spawns 32 cards across all instances.
- **Verification**: You should see your 10 cards face-up, while opponent hands and the Talon remain face-down.

### Troubleshooting
- **Network UI missing?** Ensure the `MatchmakingUI` object is active in the scene.
- **Cards not appearing?** Check that the `CardDatabase` asset is assigned to the `GameSetup` component in the Inspector.
- **Focus Issues**: Enable **"Run In Background"** in *Project Settings > Player* to prevent clones from pausing when you click between windows.

## Todos

### Address Input Manager warning

This project uses Input Manager, which is marked for deprecation. To manage input in your project, use the Input System package instead.

## references

* [How to use GitHub Copilot (the complete beginner's guide)](https://www.youtube.com/watch?v=SJqGYwRq0uc)
* [Mexico game rules](https://www.pagat.com/auctionwhist/meksiko.html)

## diary

### 20251227

* Created this github project
* Ask copilot: I want to create a multiplayer card game using unity game development tool, can you help? 
  * Copilot said yes and provided project plan and folder structure

### 20251230

* Ask copilot: Start with Phase 1 (Project Setup & Core Architecture)
  * Copilot created folder structure in the root of this project
* Question: Move folder structure under subfolder called 'mexico'
  * Copilot did it.
* Question: Can I use Unity version 6.3 (latest version)
  * Yes, copilot updated documentation to reflect it.
