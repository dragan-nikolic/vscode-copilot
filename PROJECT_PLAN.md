# Mexico Card Game - Project Plan

## Project Overview
A multiplayer card game built with Unity, featuring online gameplay, card 
mechanics, and player progression.

## Development Phases

### Phase 1: Core Foundation (COMPLETE)
- [x] Define Card Data structures (Suits, Ranks, Values)
- [x] Create ScriptableObject-based Card Database
- [x] Implement basic Card prefab with visual states (Face Up/Down)
- [x] Basic Game Manager with State Machine (MainMenu, Lobby, Bidding, etc.)

### Phase 2: Networking & Synchronization (COMPLETE)
- [x] Integrate Unity Netcode for GameObjects (NGO)
- [x] Configure NetworkManager with Player and Card prefabs
- [x] Implement Matchmaking UI (Host/Join logic)
- [x] Server-authoritative card spawning and distribution
- [x] Client-side visual synchronization via ClientRpc (Ownership-based visibility)

### Phase 3: Bidding & Talon Phase (IN PROGRESS)
- [ ] **Bidding Logic**:
    - [x] Synchronized Bidding UI buttons (5-10, Meksiko, Pass)
    - [x] Networked bidding state (Current bid, Current bidder) using NetworkVariables
    - [ ] Active Player Enforcement (Hide UI for players whose turn it isn't)
- [ ] **Talon Phase**:
    - [ ] Identify the Declarer (winner of the bid)
    - [ ] Reveal 2 Talon cards specifically to the Declarer
    - [ ] Implement swapping logic (Declarer chooses 2 cards to discard)

### Phase 4: Gameplay Loop (UPCOMING)
- [ ] Turn Management system
- [ ] Card playing validation (Matching suits/ranks)
- [ ] Trick evaluation logic
- [ ] Score tracking per round

### Phase 5: UI/UX & Polish (UPCOMING)
- [ ] Player avatars and name displays
- [ ] Turn timer/indicator
- [ ] Card animations (moving from hand to table)
- [ ] Sound effects for dealing and playing cards

## Technical Stack

### Core Technologies
- **Engine**: Unity 6.3
- **Language**: C#
- **Networking**: Mirror (recommended) or Unity Netcode for GameObjects
- **Version Control**: Git + GitHub/GitLab

### Recommended Packages
- **DOTween**: For smooth animations
- **TextMeshPro**: For better text rendering
- **Unity UI**: For interface elements
- **Newtonsoft.Json**: For data serialization

## Key Systems to Build

### 1. Card System
- Card data (ScriptableObjects)
- Card rendering
- Card effects/abilities
- Card states (in deck, in hand, in play, discarded)

### 2. Game Manager
- Turn management
- Game state machine
- Rule enforcement
- Win/loss detection

### 3. Deck Manager
- Deck building
- Deck shuffling
- Draw mechanics
- Discard pile management

### 4. Network Manager
- Client-server communication
- State synchronization
- Matchmaking
- Lobby system

### 5. UI System
- Menu navigation
- Deck builder interface
- In-game HUD
- Card preview/tooltips

## Risk Management

### Technical Risks
- **Network latency**: Implement client-side prediction
- **State synchronization bugs**: Use authoritative server model
- **Memory leaks**: Profile regularly, dispose properly

### Design Risks
- **Game balance**: Continuous playtesting
- **Complexity creep**: Start simple, iterate
- **Poor UX**: Regular user testing

## Success Metrics
- Stable 2-player matches with <100ms latency
- 0 critical bugs in core gameplay
- 30+ unique, balanced cards
- Complete tutorial for new players
- Successful deployment to target platform

## Next Steps
Proceed with Phase 3 by enforcing the active bidder logic, so only the player 
whose turn it is can see the bidding buttons.
