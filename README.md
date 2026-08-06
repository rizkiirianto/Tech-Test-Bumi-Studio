# Tech Test - Roguelike Resource Management

## Game Overview
This is a 2D roguelike deckbuilder prototype focusing on resource management. The player navigates through 7 procedurally generated nodes consisting of combat encounters and campfires. 

The core mechanic involves balancing two primary resources: **HP** and **Fatigue**. Engaging in combat increases Fatigue. If Fatigue reaches 100, the player becomes exhausted, adding a permanent "Exhaustion" debuff card to their deck for the remainder of the run. At campfires, the player must choose whether to rest (healing HP and resetting Fatigue) or train (increasing max HP but keeping Fatigue high). 

The win condition is surviving all 7 rooms and defeating the final boss. The lose condition is letting the player's HP drop to 0 during an encounter.

## How to Run
1. Open the `/Build` folder in the root directory.
2. Run the executable file (`.exe`).

- **Engine & version used:** Unity 6000.0.75f1
- **Build location:** `/Build/`

## Technical Decisions
- **Separation of Concerns:** The codebase is split into distinct namespaces (`Core`, `Combat`, `Data`, and `UI`). This modularity makes it easier to extend individual systems without creating tangled dependencies.
- **Data-Driven Design (Scriptable Objects):** `UnitData` and `CardData` are implemented as Scriptable Objects. This allows for rapid balancing and creation of new content (enemies, cards) directly in the Unity Inspector without modifying code. To prevent overwriting asset files during a run, data is cloned into memory at the start of the game via `RunManager`.
- **Centralized Run State:** `RunManager` acts as the single source of truth for the run's progression (current room, persistent HP, and Fatigue). It dynamically handles transitioning between combat nodes (spawning random enemies) and campfire nodes.
- **Isolated Deck Logic:** The draw, discard, and shuffle logic is encapsulated entirely within the `DeckManager`, keeping the `BattleManager` focused purely on turn order and resolving actions.
- **What I chose NOT to do:** I deliberately avoided using pre-built FSM frameworks or complex animation systems, prioritizing the implementation of robust core logic and resource management state flow from scratch within the tight timebox.

## What I Would Do With More Time
- **Meta-progression System:** Implement a system where completing runs rewards a persistent currency to unlock starting bonuses or new cards for future runs.
- **Complex Random Events:** Expand the node types beyond just Combat and Campfire to include text-based events that force the player to make difficult choices impacting their resources (e.g., sacrificing max HP to remove a debuff card).
- **Run Map UI:** Build a visual map (similar to Slay the Spire) instead of a linear counter, allowing the player to choose their path and anticipate upcoming nodes.
- **Audio & Visual Polish:** Add particle effects, screen shake, and sound effects to give combat actions and resource depletion a more visceral impact.

## Known Issues
- Currently, there is no mid-run save feature. Closing the application will forfeit the ongoing run.
- UI scaling may not perfectly adapt to ultra-wide or non-standard aspect ratios, as the prototype was built targeting standard PC resolutions.
