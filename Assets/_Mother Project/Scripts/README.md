# Mother Project - Scripts Documentation

A **turn-based tactical RPG** built in Unity featuring a hex-grid combat system, multiple character classes, weapon-based action loadouts, a dice-based damage system, an inventory and item system, enemy AI, ultimate abilities, a wave-based encounter system, and full save/load persistence.

---

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Core Systems](#core-systems)
  - [Situation System (Combat)](#situation-system-combat)
  - [Turn System](#turn-system)
  - [Grid System](#grid-system)
  - [Action / Command System](#action--command-system)
  - [Calculation & Resolution](#calculation--resolution)
- [Character System](#character-system)
  - [Base Classes & Player Classes](#base-classes--player-classes)
  - [Temporary Stats (Runtime State)](#temporary-stats-runtime-state)
  - [Weapon System](#weapon-system)
- [Combat Actions](#combat-actions)
  - [Concrete Actions](#concrete-actions)
  - [Ultimate System](#ultimate-system)
  - [DOT / Status Effects](#dot--status-effects)
  - [Environmental & Area Actions](#environmental--area-actions)
- [Enemy AI](#enemy-ai)
- [Grid & Movement](#grid--movement)
- [Inventory System](#inventory-system)
- [Interaction System](#interaction-system)
- [Wave System](#wave-system)
- [Save / Load & Persistence](#save--load--persistence)
- [Scene Management](#scene-management)
- [UI & HUD](#ui--hud)
- [Camera System](#camera-system)
- [Audio System](#audio-system)
- [VFX & Particles](#vfx--particles)
- [Objective System](#objective-system)
- [Experience & Leveling](#experience--leveling)
- [Miscellaneous](#miscellaneous)
- [Deprecated / Unused Scripts](#deprecated--unused-scripts)
- [Folder Structure](#folder-structure)

---

## Architecture Overview

The project follows a **singleton manager pattern** where most core systems expose a static `instance` for cross-system communication. Combat actions use the **Command pattern** (`ICommand` interface) to queue, execute, and undo player/enemy turns. ScriptableObjects (`ImprovedActionStat`) define action data with dice-based damage tables, while a `DictionaryManager` maps action names to delegate calls at runtime.

**Key Design Patterns:**
- **Command Pattern** - All combat actions implement `ICommand` and are queued as `Turn` objects
- **Singleton Managers** - `TurnManager`, `GridSystem`, `TempManager`, `HandleTurnNew`, etc.
- **ScriptableObject Data** - Action stats (`ImprovedActionStat`), ultimate factories, and item definitions
- **Event-Driven Architecture** - C# events/delegates for grid generation, game state changes, death, wave transitions
- **Factory Pattern** - Ultimate abilities created via `UltimateActionsFactory`
- **Dice-Based Damage** - Actions use weighted dice rolls mapped to damage ranges via `RangeMapping` tables

---

## Core Systems

### Situation System (Combat)

The "Situation" is the game's term for a tactical combat encounter on the hex grid.

| Script | Purpose |
|--------|---------|
| `Managers/TempManager.cs` | Runtime combat state holder: tracks current attacker/defender, manages `GameStates` FSM transitions (StartTurn, MidTurn, TargetSelection, Simulation, etc.), rotates characters to face opponents. |
| `Managers/TemporaryStats.cs` | Per-character runtime combat stats (current HP, AP, dexterity, block/dodge/counter flags, grid position, team, mortality). Implements `IPersistableData` for save/load. |
| `Enums/Enums.cs` | Central enum definitions: `GameStates`, `MoveType`, `TargetType`, `CharClass`, `SubClass`, `Mood`, `Condition`, `Aggression`, `CurrentWeapon`, `TeamName`, `Mortality`, and more. |

**Game States Flow:**
```
StartTurn -> MidTurn -> TargetSelectionTurn -> Simulation -> StartTurn (next player)
                |-> MovementGridSelectionTurn
```

When all players in a round have taken their turns, `ResetTurn()` increments the round counter, resets per-round flags (block, dodge, counter, imbuement), regenerates AP based on Intelligence, and starts the next round.

### Turn System

| Script | Purpose |
|--------|---------|
| `Turn/Turn.cs` | Data class representing a single turn: holds the `Player` (CharacterBaseClasses), `ICommand`, `PriorityValue`, and `target`. |
| `Turn/TurnManager.cs` | Central turn orchestrator. Manages player order, starts/ends turns, populates target lists based on action range, handles round resets (AP regeneration, status clear). Calls `HandleTurnNew.PerformTurns()` on end turn and `EnemyAIController.StartEnemyTurn()` for non-player characters. |
| `Turn/HandleTurnNew.cs` | Active turn handler: manages the per-player action queue, executes turns sequentially with `async/await` (UniTask), supports **undo mechanics** (restores AP, reverts grid position for movement), saves turn data to JSON, and fires events (`OnTurnEnd`, `OnNewAction`, `OnActionExecution`). |
| `Turn/PlayerTurn.cs` | MonoBehaviour attached to each character, tracks `isMoveOn` (whether the player has used their free movement) and `myTurn` state. |
| `Turn/PlayerTurnInput.cs` | Handles player input during their turn. |
| `Turn/TargetButton.cs` | UI button for target selection during combat. |
| `Turn/DAOScriptableObject.cs` | Data Access Object that loads `ActionStat` and `ImprovedActionStat` ScriptableObjects from the Resources folder by name. |
| `TurnTimer/TurnTimer.cs` | Countdown timer for each turn with start/stop controls. |

**Turn Execution Flow:**
1. `TurnManager.StartTurn()` activates the current player, sets up UI, populates targets
2. Player queues actions (each creates an `ICommand` -> `Turn` -> added to `HandleTurnNew`)
3. Player presses Space to end turn -> `TurnManager.EndTurn()`
4. `HandleTurnNew.PerformTurns()` executes all queued turns sequentially via `await turn.Command.Execute()`
5. After execution, advances to next player or resets the round

### Grid System

| Script | Purpose |
|--------|---------|
| `Grid/GridSystem.cs` | Generates and manages the **hex grid**. Spawns grid cells as GameObjects in a 2D array, converts world-to-grid coordinates, handles grid visibility toggle (G key), calculates hex adjacency, fires events on generation (`OnGridGeneration`, `OnGridGenerationSpawn`). |
| `Grid/GridStat.cs` | Per-cell data: grid coordinates, occupancy, neighbor list, color state. |
| `Grid/GridMovement.cs` | Core movement engine. Handles path selection via mouse clicks, adjacent-matrix queries for range/targeting, line rendering for selected paths, NavMesh-based character movement along paths, and highlights for movement range. |
| `Grid/GridActivation.cs` | Manages character spawn positions on the grid and NavMesh surface baking. |
| `Grid/GridHover.cs` | Visual hover feedback when the mouse hovers over grid cells. |
| `Grid/GridPlayerAnimation.cs` | Controls character animations during grid movement. |
| `Grid/TeamManager.cs` | Tracks which characters belong to which team using a dictionary. Detects when a team is eliminated (win/lose condition). |
| `Grid/HoverEffect.cs` | Visual hover effect on grid tiles. |
| `Grid/SerializedDictionaryAttribute.cs` | Custom attribute for serializing dictionaries in the Inspector. |
| `Grid/Ghost/GhostTrail.cs` | Ghost trail visual for previewing movement paths. |
| `Grid/Ghost/LerpAndLoop.cs` | Lerp animation helper for ghost trail movement. |

### Action / Command System

| Script | Purpose |
|--------|---------|
| `Interfaces/ICommand.cs` | Core command interface: `Execute()` (UniTask), `GetPVValue()`, `GetAPValue()`, `GetActionName()`, `GetTarget()`, `GetAgent()`, `GetPaths()`, `GetActionType()`. |
| `Interfaces/IUltimate.cs` | Interface for ultimate abilities: `setValues()`, `ExecuteUltimate()`, `GetultimateThreshold()`. |
| `Actions/ActionArchive.cs` | Central action registry singleton. Contains methods for every combat action (60+). Each method fetches attacker/target from `TempManager`, creates an `ICommand` implementation, wraps it in a `Turn`, adds it via `HandleTurnNew.AddTurn()`, and deducts AP. Also provides utility methods: `GetOffenseActions()`, `GetDefenceActions()`, `GetRangedActions()`, `GetMeleeActions()`, `GetActionsWithinAP()`. |
| `Dictionary/DictionaryManager.cs` | Maps action name strings to `ActionDelegate` callbacks at runtime. Also maps `OffenseModifier` and `DefenseModifier` enums to character stat accessor lambdas for damage calculation. |
| `Scriptable/ImprovedActionStat.cs` | Primary action data ScriptableObject. Contains dice-based `RangeMapping[]` (min/max roll -> damage), accuracy, AP cost, priority value, range, VFX/sound references, body location strings for animation, and action stance (offense/defense). |
| `Scriptable/UltimateActionsFactory.cs` | ScriptableObject factory for creating ultimate ability instances. |

**Damage Calculation Flow (Current System):**
1. Roll a weighted dice using `DiceNumberGenerator.GetDiceValue(firstPct, secondPct, lastPct)`
2. Map the dice value to damage via `ActionResolver.CalculateNewDamage(diceValue, actionScriptable)` using `RangeMapping` tables
3. Multiply by `CurrentDamageMultiplier` (affected by buffs)
4. For ranged attacks, add dexterity bonus: `FloorToInt((CurrentDex + 3) / 2) + 1`
5. Check for Block (halve damage), Counter (reflect half damage back), Dodge (reduce accuracy by 50%)
6. Apply via `HealthManager.HealthCalculation(damage, currentHP)`

### Calculation & Resolution

| Script | Purpose |
|--------|---------|
| `Calculation Class/ActionResolver.cs` | Core resolution logic. `CalculateNewDamage()` maps dice rolls to damage via `RangeMapping` tables. `ActionAccuracyCalculation()` for hit/miss rolls. `APResolver()` and `APCarryOver()` for AP management. Also contains legacy formulas (`CalculateKillDamage`, `CalculateDealDamage`) that are no longer used. |
| `Calculation Class/DiceNumberGenerator.cs` | Weighted random dice generation. Rolls a value 1-100 and maps it to one of three tiers (first/second/last percentage) to produce the dice outcome. |
| `Calculation Class/HealthManager.cs` | HP calculation, health cap enforcement, and **player mortality handling** (removes dead characters from turn order, updates team lists, triggers win/lose conditions, fires `OnCharacterDeath` event, loads hub scene on defeat). |

---

## Character System

### Base Classes & Player Classes

| Script | Purpose |
|--------|---------|
| `Character/Character Base Class/CharacterBaseClasses.cs` | Abstract base class for all characters. Defines stats (Strength, Dexterity, Intelligence, Arcana, Endurance, Skill, Mind, HP, Resolve), equipped weapon (`CurrentWeapon` enum), available actions list (`ImprovedActionStat`), available items, ultimate ability, and abstract level-up methods. |
| `Player Classes/FighterClass.cs` | Fighter class (extends `CharacterBaseClasses`). |
| `Player Classes/CasterClass.cs` | Caster class. |
| `Player Classes/HunterClass.cs` | Hunter class. |
| `Player Classes/PerformerClass.cs` | Performer class. |
| `Player Classes/TalkerClass.cs` | Talker class. |
| `Player Classes/Other.cs` | Generic/other class. |
| `Character/Activator/PlayerActivator.cs` | Activates/deactivates player GameObjects. |
| `Character/CameraSwitcher.cs` | Switches camera focus between characters. |
| `Character/Player Companions/PlayerCompanions.cs` | Manages companion characters. |
| `Character/Player Companions/SwitchMC.cs` | Handles main character switching. Fires events `OnCharacterChange`, `OnCharacterRemove`, `OnPrevScene`. Manages linked/unlinked character lists across scenes. |

**Character Stats:**
- **Strength** - Physical attack modifier (melee offense)
- **Dexterity** - Ranged attack bonus, movement range on grid
- **Intelligence** - AP regeneration per round (`2 + Floor((Intelligence + 1) / 2)`)
- **Arcana** - Magical power modifier
- **Endurance** - Physical defense modifier
- **Skill** - Used in legacy clash calculations
- **Mind** - Mental defense modifier
- **HP** - Health Points (all attacks target this in the current system)
- **Resolve** - Resolve Points (legacy, not actively targeted)

### Temporary Stats (Runtime State)

`TemporaryStats` tracks all mutable combat state per-character:
- Current HP, AP, stat modifiers, damage multiplier
- Active status flags: `IsBlockActive`, `IsDodgeActive`, `IsImbuementActive`, `IsCounterActive`
- Grid position (`currentPlayerGridPosition`), team assignment (`TeamName`), mortality state
- Visibility multiplier (`playerVisiblity`) for stealth/smoke interactions
- UI references (action panel, item panel, selection particles, flying text parent)
- Save/Load via `IPersistableData` interface
- Weapon action setup via `SetWeaponActions()` on wave init

### Weapon System

| Script | Purpose |
|--------|---------|
| `Weapon/WeaponManager.cs` | Maps `CurrentWeapon` enum to action lists via a dictionary of factory functions. Supports 10 weapon types: Dagger, Sword, Talisman, BowAndArrow, Hammer, Axe, Spear, Staff, Spoon, Butcher. Handles weapon data save/load from JSON, weapon level-up (increases `RangeMapping.MappedValue` by 2 per level). Active weapon actions (Dagger, Talisman) are persisted to JSON and loaded on wave start. |

---

## Combat Actions

### Concrete Actions

All concrete actions implement `ICommand` and are located under `Situation System/Concretes/`. The current system uses `ImprovedActionStat` with dice-based damage tables for all actions.

**Offensive Actions (Melee):**
- `MeleeAttack.cs` - Core melee attack. Uses dice roll -> `RangeMapping` damage, checks for Block (halve), Counter (reflect), Dodge (accuracy penalty), and Imbuement (chains Puncture on Stab). Plays animation via `CutsceneManager` and VFX via `SpawnVFX`.
- `Concretes/New Concretes/Mon Actions/Assassinate.cs` - Instant kill against Talker class, otherwise dice-based damage
- `Concretes/New Concretes/Mon Actions/DaggerRising.cs` - Dagger rising attack
- `Concretes/New Concretes/Mon Actions/DaggerSweep.cs` - Sweeping dagger attack
- `Concretes/New Concretes/Mon Actions/Imbuement.cs` - Imbues weapon with venom (next Stab chains Puncture)
- `Concretes/New Concretes/Mon Actions/Puncture.cs` - Applies bleed DOT

**Offensive Actions (Ranged):**
- `RangedAttack.cs` - Core ranged attack. Adds dexterity bonus to damage. At low HP (<20%), shifts dice weights toward higher damage tier.

**Offensive Actions (Roud-Specific):**
- `Concretes/New Concretes/Roud Actions/BoneShield.cs` - Bone shield defense
- `Concretes/New Concretes/Roud Actions/Impale.cs` - Impale attack
- `Concretes/New Concretes/Roud Actions/MagicSiphon.cs` - Siphons magic from target
- `Concretes/New Concretes/Roud Actions/SkeletonGrab.cs` - Grabs target with skeleton (applies DOT)
- `Concretes/New Concretes/Roud Actions/SoulSteal.cs` - Steals target's soul
- `Concretes/New Concretes/Roud Actions/SoulTransfer.cs` - Transfers HP to ally

**Defensive Actions:**
- `Concretes/Survive/Block.cs` - Sets `IsBlockActive` flag, halves incoming damage next hit
- `Concretes/Survive/Counter.cs` - Sets `IsCounterActive` flag, reflects half damage back to attacker
- `Concretes/Survive/Dodge.cs` - Sets `IsDodgeActive` flag, reduces enemy accuracy by 50%
- `Concretes/Survive/KeenSenses.cs` - Enhanced senses
- `Concretes/Survive/BreakFree.cs` - Escape from grab/lock conditions
- `Concretes/Survive/Survive.cs` - Base survival action

**Movement:**
- `Move.cs` - Grid movement command. Takes a path of grid cells and a NavMeshAgent, moves the character along the path via `GridMovement.MoveCharacterGrid()`.
- `Concretes/New Concretes/Dash.cs` - Enhanced movement

**Support:**
- `Concretes/New Concretes/Buff.cs` - Doubles target's `CurrentDamageMultiplier`
- `Concretes/New Concretes/Debuff.cs` - Reduces target stats
- `Concretes/New Concretes/Heal.cs` - Restores HP

**PushBack System:**
- `Concretes/New Concretes/PushBack/PushBack.cs` - Knockback action
- `Concretes/New Concretes/PushBack/PushDetector.cs` - Detects push collision
- `Concretes/New Concretes/PushBack/ObjectToBePushed.cs` - Marks pushable objects

**Special:**
- `Concretes/New Concretes/Warp Surge.cs` - Teleportation action
- `Concretes/New Concretes/GroundBlast.cs` - AoE ground attack
- `Concretes/New Concretes/HangingTargetFall.cs` - Environmental interaction (drops hanging objects)

### Ultimate System

| Script | Purpose |
|--------|---------|
| `UltimateSystem/UltimateSystem.cs` | Manages ultimate ability availability (threshold check against ultimate point count), handles target selection for single-target vs AoE ultimates, queues the ultimate as an `ICommand` via `HandleTurnNew.AddTurn()`. |
| `UltimateSystem/Orb.cs` | Collectible orb that contributes to ultimate charge. |
| `UltimateSystem/OrbSpawner.cs` | Spawns orbs on the grid. |
| `UltimateSystem/RotateAndSIne.cs` | Visual animation for orbs (rotate + sine wave). |
| `UltimateSystem/UI/UltimateUI.cs` | Ultimate bar UI display. |
| `Concretes/Ultimates/MonUltimateCommand.cs` | Mon's ultimate ability command. |
| `Concretes/Ultimates/Ultimate2Command.cs` | Second character's ultimate ability command. |
| `Concretes/Ultimates/Factories/MonUltimateFactory.cs` | Factory ScriptableObject for Mon's ultimate. |
| `Concretes/Ultimates/Factories/Ultimate2Factory.cs` | Factory ScriptableObject for second ultimate. |

### DOT / Status Effects

| Script | Purpose |
|--------|---------|
| `Concretes/New Concretes/DOT/BaseDOThandler.cs` | Base class for Damage-Over-Time effects. |
| `Concretes/New Concretes/DOT/PunctureDOTHandler.cs` | Bleed DOT from Puncture action. |
| `Concretes/New Concretes/DOT/SkeletonGrabDOTHandler.cs` | DOT from Skeleton Grab ability. |

### Environmental & Area Actions

| Script | Purpose |
|--------|---------|
| `Concretes/New Concretes/VenomGrid/VenomCloud.cs` | Creates a venom cloud zone on grid cells. |
| `Concretes/New Concretes/VenomGrid/VenomEffector.cs` | Applies venom damage to characters standing in the zone. |
| `Concretes/New Concretes/VenomGrid/SmokeCloud.cs` | Creates a smoke cloud zone (reduces visibility). |
| `Concretes/New Concretes/VenomGrid/SmokeEffector.cs` | Applies smoke effects (reduces `playerVisiblity`). |
| `Concretes/NonCharacterTargets/INonCharacterTarget.cs` | Interface for non-character targetable objects. |
| `Concretes/NonCharacterTargets/HangingTargets.cs` | Destructible hanging target objects on the grid. |

---

## Enemy AI

| Script | Purpose |
|--------|---------|
| `Enemy/EnemyAIController.cs` | **AP Budget Planning AI**. Called by `TurnManager.StartTurn()` for non-player characters. Uses a budget-based approach to spend all available AP per turn. |

**AI Decision Flow (EnemyAIController):**
1. Categorize available actions into ranged offense, melee offense, and defense pools
2. **Phase 1 (Low HP):** If HP below threshold (default 40%), queue a defensive action first (targets self)
3. **Phase 2 (Spend AP loop):**
   - Pick the most expensive affordable action (ranged preferred over melee for range advantage)
   - Find a valid target within that action's specific `ActionRange`
   - If no target in range and free Move is available, auto-move toward closest enemy via A* pathfinding, then retry
   - Queue the action via `DictionaryManager.GiveAction()`, subtract AP
   - Repeat until AP is exhausted or no valid actions remain
4. End turn via `TurnManager.EndTurn()`

**Target Selection:** Prioritizes lowest-HP enemy in range. Uses `GridMovement.InAdjacentMatrix()` to check range from the AI's current grid position.

---

## Grid & Movement

| Script | Purpose |
|--------|---------|
| `Movement/AutoGridMovement.cs` | A* pathfinding for AI-controlled movement on the hex grid. Used by `EnemyAIController` to auto-navigate toward/away from targets. |
| `Movement/PlayerMove.cs` | Player-controlled grid movement. |
| `Movement/MoveClash.cs` | Handles movement conflicts when two characters move to the same cell. |
| `Ghost/ShadowOfPlayer.cs` | Ghost/shadow preview of player movement. |
| `Input/GridInput.cs` | Input handling specific to grid mode (cell selection, path building). |
| `Input/InputManager.cs` | Central input manager with events for interaction, grid, and UI inputs. |
| `Input/UiInput.cs` | UI-specific input handling. |
| `NumpadHotkeys.cs` | Numpad keyboard shortcuts for in-combat actions. |

---

## Inventory System

| Script | Purpose |
|--------|---------|
| `Inventory System/InventoryManager.cs` | Per-character inventory management using nested dictionaries (`GameObject` -> `ItemClass` -> `InventoryItem`). Handles add/remove/use items, integrates with `CurrencySystem` events, persists across character switches via `SwitchMC`. |
| `Inventory System/ItemClass.cs` | Abstract ScriptableObject base class for all items. Defines `UseObject(TemporaryStats)`, `GetItem()`, `GetToolObject()`, `GetConsumableObject()`. |
| `Inventory System/InventoryItem.cs` | Runtime inventory slot: wraps an `ItemClass` with a stack count. |
| `Inventory System/ConsumableObject.cs` | Consumable item type (healing, buffs). |
| `Inventory System/ToolObject.cs` | Tool item type. |
| `Inventory System/WeaponCardObject.cs` | Weapon card item. |
| `Inventory System/GroundBlastObject.cs` | Ground blast throwable item. |
| `Inventory System/SmokeCloudObject.cs` | Smoke cloud throwable item. |
| `Inventory System/AddingItem.cs` | Helper for adding items to inventory. |
| `Inventory System/StoreObjects.cs` | Store/shop system for purchasing items. |
| `Inventory System/CurrencySystem.cs` | Experience-based currency system with events: `OnItemAdded`, `OnItemRemoved`, `OnItemUsed`. |
| `Inventory System/Inventory_UI.cs` | Inventory UI display. |

---

## Interaction System

| Script | Purpose |
|--------|---------|
| `Interactions/IInteractable.cs` | Interface: `Interact(GameObject player)`, `IsGridTrigger()`. |
| `Interactions/Interactor.cs` | Radius-based interaction detection using `Physics.OverlapSphereNonAlloc` on the "Interactable" layer. Finds nearest interactable object and triggers on `I` key press. |
| `Interactions/InteractionPromptUI.cs` | Shows/hides interaction prompt popup. |
| `Interactions/GridInteract.cs` | Triggers grid/combat encounters. |
| `Interactions/ItemInteract.cs` | Item pickup interaction. |
| `Interactions/InventoryInteractor.cs` | Opens inventory interaction. |
| `Interactions/BookshelfInteractor.cs` | Bookshelf interaction (lore/story). |
| `Interactions/BoxInteract.cs` | Box/chest interaction. |
| `Interactions/CabinetInteractor.cs` | Cabinet interaction. |
| `Interactions/DadiSceneInteractor.cs` | Scene-specific NPC interaction. |
| `Interactions/GramoInteractor.cs` | Gramophone interaction. |
| `Interactions/SaveGameInteractor.cs` | Save point interaction. |
| `Interactions/SwordInteractor.cs` | Sword pickup interaction. |
| `Interactions/TVInteractor.cs` | TV interaction. |
| `Interactions/Weapon_Interactor.cs` | Weapon pickup/equip interaction. |

---

## Wave System

| Script | Purpose |
|--------|---------|
| `Wave System/WaveManager.cs` | Manages multi-wave combat encounters. Each wave defines which characters participate. Supports per-wave Timeline cutscenes, handles character spawning/despawning between waves, loads weapon data, fires events (`OnGridReady`, `OnGridInit`). |
| `Wave System/WaveWrapperClass.cs` | Data class: wave ID and list of `PlayerTurn` characters for that wave. |
| `Wave System/GridTrigger.cs` | Trigger zone that initiates a specific wave encounter. |

---

## Save / Load & Persistence

| Script | Purpose |
|--------|---------|
| `JsonFile/FileHandler.cs` | Generic JSON file I/O utility. Uses `JsonUtility` with a wrapper pattern to serialize/deserialize lists of any type to `Application.persistentDataPath`. |
| `JsonFile/IPersistableData.cs` | Interface: `SaveData(PlayerDataSave)`, `LoadData(PlayerDataSave)`. Implemented by `TemporaryStats`. |
| `JsonFile/InputHandlerForSaving.cs` | Manages save input triggers and collects turn data for saving. |
| `JsonFile/SaveTurnInformation.cs` | Data class for saving per-turn information: player/target names, HP, AP, grid positions, action name, AP cost, timestamp. |

---

## Scene Management

| Script | Purpose |
|--------|---------|
| `Load Scene/LoadSceneManager.cs` | Persistent scene manager (`DontDestroyOnLoad`). Handles async scene loading via UniTask, save/load game flow, new game initialization, continue game, and linked/unlinked character management across scenes. Tracks defeat counter. |
| `Load Scene/SceneLoading.cs` | Scene loading utilities. |
| `Load Scene/RandomSceneLoad.cs` | Random scene selection. |
| `Load Scene/RespawnLoadScene.cs` | Respawn/reload scene logic. |
| `Situation System/SceneManager/GameSceneManager.cs` | In-combat scene management. |
| `Situation System/SceneManager/SceneField.cs` | Custom serializable scene reference field for the Inspector. |
| `Situation System/SceneManager/SpawnManager.cs` | Character spawn point management. |
| `Situation System/SceneManager/AdditiveSceneLoad.cs` | Additive scene loading for layered environments. |

---

## UI & HUD

### HUD Scripts

| Script | Purpose |
|--------|---------|
| `HUD/PlayerHUD.cs` | Main player HUD display. |
| `HUD/PlayableCharacterUI.cs` | Per-character UI panel in the HUD. |
| `HUD/ActionActivator.cs` | Enables/disables action buttons based on available AP. Checks each button's action AP cost against current AP and toggles interactable state + color. Also checks ultimate availability. |
| `HUD/EndTurnConfirmation.cs` | End turn confirmation dialog. |
| `HUD/EndTurnGlow.cs` | Visual glow effect on end turn button. |
| `HUD/BlinkText.cs` | Blinking text animation. |
| `HUD/ButtonName.cs` | Stores and exposes the action name associated with a button. |
| `HUD/CameraSwitcherNumpad.cs` | Numpad camera switching controls. |
| `HUD/FlyingText.cs` | Floating damage/heal numbers above characters. |
| `HUD/HoverDisplayStats.cs` | Stats tooltip on hover. |
| `HUD/HoverEffectCharacter.cs` | Character hover highlight. |
| `HUD/HoverEffectController.cs` | Manages hover effect state. |
| `HUD/TextHoverEffect.cs` | Text hover animation. |
| `HUD/ToggleChildrenVisibility.cs` | Toggles child UI element visibility. |
| `HUD/UICircularLayoutGroup.cs` | Custom circular layout group for radial UI. |

### Game UI Scripts

| Script | Purpose |
|--------|---------|
| `Game UI Scripts/General/UI.cs` | Central UI manager: panel show/hide, flying text display, notification system, panel animation. |
| `Game UI Scripts/General/PlayerStatUI.cs` | Player stat summary and detail panels. Updates HUD for all characters each turn. |
| `Game UI Scripts/General/ActionNotification.cs` | Action notification popups. |
| `Game UI Scripts/CharacterStats_UI.cs` | Character stats display UI. |
| `Game UI Scripts/ButtonStackManager.cs` | Dynamically populates action button panels and item panels per character based on their `GetAvailableActions()` and `GetAvailableItems()`. |
| `Game UI Scripts/ButtonHoverAnimation.cs` | Button hover animation effect. |
| `Game UI Scripts/ButtonScale.cs` | Button scale animation on interaction. |
| `Game UI Scripts/TextFadeInOut.cs` | Text fade in/out animation for the action queue display (shows queued actions during a turn). |
| `Game UI Scripts/Radial UI/ButtonBehavior.cs` | Radial menu button behavior. |

### Action Selection UI

| Script | Purpose |
|--------|---------|
| `Action Selection/ActionSlot.cs` | Individual action slot in the loadout UI. |
| `Action Selection/ActionSpawner.cs` | Spawns action slots for the loadout screen. |
| `Action Selection/ObjectDragDrop.cs` | Drag-and-drop functionality for action slot assignment. |

---

## Camera System

| Script | Purpose |
|--------|---------|
| `Camera/CameraControlForGridSituation.cs` | Camera controls during grid combat. |
| `Camera/CameraController.cs` | General camera controller. |
| `Camera/Cinemachine_CameraShake.cs` | Cinemachine-based camera shake effects for impacts. |
| `Camera/ConfinerSwitcher.cs` | Switches Cinemachine confiner bounds between areas. |
| `Camera/IsoMetricToTPS.cs` | Transitions between isometric and third-person camera views. |
| `Situation System/Camera/CameraRoam.cs` | Free camera roaming during combat observation. |
| `Situation System/Camera/GridCamera.cs` | Dedicated grid-mode camera that activates on `OnGridGeneration` event. |

---

## Audio System

| Script | Purpose |
|--------|---------|
| `Sounds/SoundManager.cs` | Persistent singleton audio manager (`DontDestroyOnLoad`) with separate effects and music `AudioSource` channels. |
| `Sounds/AudioController.cs` | Audio playback controller for specific scenarios. |
| `Sounds/PlayerSoundBank.cs` | Per-character sound bank referenced by `ImprovedActionStat.SoundReferences`. |

---

## VFX & Particles

| Script | Purpose |
|--------|---------|
| `Particles/SpawnVFX.cs` | Core VFX manager on each character. Sets up VFX prefabs, target positions, sounds, and animations. Called by all `ICommand.Execute()` implementations during `HandleAnimation()`. |
| `Particles/VFXSpawnPosition.cs` | Defines named body location positions (e.g., "RightHand", "MidBody") for VFX attachment via a dictionary. |
| `Particles/CurveThrowVFX.cs` | Curved projectile VFX (e.g., dagger throw arcs). |
| `Particles/ThrowVFX.cs` | Linear throw VFX. |
| `Particles/EndVfx.cs` | Cleans up VFX after completion. |
| `Particles/DestroyObject.cs` | Timed object destruction (for VFX cleanup). |
| `Particles/Firefly_Audio/FireFliesParticleAudio.cs` | Audio-reactive firefly particle effects. |
| `Particles/Firefly_Audio/FireFlyParticleCollision.cs` | Firefly particle collision handling. |
| `VFX/StopVFX.cs` | Stops active VFX effects. |
| `Situation System/PrefabData/VisualCueData.cs` | Data container for visual cue prefab references. |

---

## Objective System

| Script | Purpose |
|--------|---------|
| `Objective/ObjectiveManager.cs` | Manages a list of objectives with states (`Locked`, `Active`, `Completed`, `Failed`). Supports dependency chains where completing one objective unlocks dependents. |
| `Objective/Objective.cs` | Base objective class with evaluation logic and state management. |
| `Objective/CollectBarrelObjective.cs` | Specific objective: collect barrels. |

---

## Experience & Leveling

| Script | Purpose |
|--------|---------|
| `Earning EXP/ExperienceManager.cs` | Singleton that fires `OnExperienceChanged` event when XP is earned. Characters subscribe via `TemporaryStats`. XP is also used as currency for purchasing items and leveling weapons. |

---

## Miscellaneous

| Script | Purpose |
|--------|---------|
| `Functions/CommandInTurn.cs` | Turn command utilities. |
| `Animation Scripts/SkipCutscene.cs` | Skip cutscene functionality. |
| `Cutscene/CutsceneManager.cs` | Timeline-based cutscene management. `PlayAnimationForCharacter()` is called by every action's `HandleAnimation()` to play attack/defense animations. |
| `Situation System/Miscellaneous/PreserveRotation.cs` | Locks an object's rotation. |
| `Situation System/Miscellaneous/RainSystem.cs` | Weather rain system. |
| `Situation System/Miscellaneous/RemoveCue.cs` | Removes visual cues from the scene. |
| `Situation System/Miscellaneous/StringData.cs` | Static string constants (resource directory paths, tag names like `"Player"`). |
| `Situation System/Miscellaneous/TimerHandler.cs` | General-purpose timer utility. |
| `Situation System/Couroutine/DelayClass.cs` | Coroutine delay helper. |

---

## Deprecated / Unused Scripts

The following scripts remain in the codebase but are **no longer actively used** due to design changes over time:

| Script | Reason Deprecated |
|--------|-------------------|
| `Managers/GameManager.cs` | Original command queue executor. Replaced by `HandleTurnNew` which handles per-player turn queues with undo support. |
| `Turn/HandleTurn.cs` | Legacy turn handler with priority sorting and clash detection. Replaced by `HandleTurnNew`. |
| `Turn/ClashCalculation.cs` | Clash system (two players attacking each other simultaneously) is no longer used. |
| `Enemy/EnemyAI.cs` | Original trait-based enemy AI. Replaced by `EnemyAIController` which uses an AP budget planning approach. |
| `Network/RelayNetwork.cs` | Unity Relay multiplayer integration (mostly commented out, never completed). |
| `Network/NetworkUIManager.cs` | Network lobby UI (unused). |
| `Network/PlayerNetworkManager.cs` | Per-player network state (unused). |
| `Scriptable/ActionStat.cs` | Legacy action data ScriptableObject with the Kill/Deal/Survive branch system and formula-based damage. Still referenced by some utility actions (Move, WarpSurge, Dash, GroundBlast) but the branch-based damage model is unused. `ImprovedActionStat` with dice-based `RangeMapping` is the current system. |
| `Concretes/Kill Actions/Choke.cs` | Legacy Kill-branch action (formula-based damage). |
| `Concretes/Kill Actions/Devour.cs` | Legacy Kill-branch action. |
| `Concretes/Kill Actions/GreaterStrike.cs` | Legacy Kill-branch action. |
| `Concretes/Kill Actions/ThirdRatePerformance.cs` | Legacy Kill-branch action. |
| `Concretes/Kill Actions/WitchsBolt.cs` | Legacy Kill-branch action. |
| `Concretes/Deal/CaptivatingPerformance.cs` | Legacy Deal-branch action (targeted Resolve damage). |
| `Concretes/Deal/FearTacticts.cs` | Legacy Deal-branch action. |
| `Concretes/Deal/Seduce.cs` | Legacy Deal-branch action. |
| `Concretes/Deal/Threaten.cs` | Legacy Deal-branch action. |

**Legacy Design (Kill/Deal/Survive):** The original system split actions into three branches -- Kill (HP damage using `CalculateKillDamage`), Deal (Resolve damage using `CalculateDealDamage`), and Survive (defensive). These used `ActionStat` ScriptableObjects with formula-based damage (`BasePower * offenseModifier / defenseModifier * critMultiplier`). The current system replaced this with `ImprovedActionStat` using dice-based `RangeMapping` tables where all actions simply deal HP damage with varying dice tier probabilities.

---

## Folder Structure

```
Scripts/
|-- Action Selection/          # Drag-and-drop action loadout UI
|-- Animation Scripts/         # Cutscene animation controls
|-- Camera/                    # Camera controllers and effects
|-- Character/                 # Character base classes, companions, activation
|   |-- Activator/
|   |-- Character Base Class/
|   |-- Player Companions/
|-- Cutscene/                  # Timeline cutscene management
|-- Earning EXP/               # Experience point system
|-- Enemy/                     # Enemy AI (EnemyAIController is active, EnemyAI is deprecated)
|-- Functions/                 # Command utilities
|-- Game UI Scripts/           # In-game UI (buttons, stats, radial menu)
|   |-- General/
|   |-- Radial UI/
|-- HUD/                       # Heads-up display elements
|-- Interactions/              # World interaction system (IInteractable)
|-- Inventory System/          # Items, inventory, currency, store
|-- JsonFile/                  # JSON save/load system
|-- Load Scene/                # Scene loading and transitions
|-- Objective/                 # Objective/quest system
|-- Particles/                 # VFX and particle systems
|   |-- Firefly_Audio/
|-- Player Classes/            # Character class implementations
|-- Situation System/          # Core combat system
|   |-- Actions/               # ActionArchive (central action registry)
|   |-- Calculation Class/     # Damage formulas, dice rolls, health management
|   |-- Camera/                # Grid camera controls
|   |-- Concretes/             # ICommand implementations for all actions
|   |   |-- Deal/              # [DEPRECATED] Resolve-targeting actions
|   |   |-- Kill Actions/      # [DEPRECATED] Formula-based HP actions
|   |   |-- New Concretes/     # Active action set (dice-based)
|   |   |   |-- DOT/           # Damage-over-time effects
|   |   |   |-- Mon Actions/   # Character-specific (Mon)
|   |   |   |-- PushBack/      # Knockback system
|   |   |   |-- Roud Actions/  # Character-specific (Roud)
|   |   |   |-- VenomGrid/     # Area denial (venom/smoke clouds)
|   |   |-- NonCharacterTargets/  # Destructible environment targets
|   |   |-- Survive/           # Defensive actions (Block, Dodge, Counter)
|   |   |-- Ultimates/         # Ultimate abilities and factories
|   |-- Couroutine/            # Coroutine helpers
|   |-- Dictionary/            # Action name -> delegate mapping
|   |-- Enums/                 # All game enumerations
|   |-- Ghost/                 # Movement preview shadows
|   |-- Grid/                  # Hex grid system
|   |   |-- Ghost/             # Grid ghost trail visuals
|   |-- Input/                 # Grid and UI input handlers
|   |-- Interfaces/            # ICommand, IUltimate
|   |-- Managers/              # TempManager, TemporaryStats (GameManager deprecated)
|   |-- Miscellaneous/         # Utility scripts
|   |-- Movement/              # Grid movement and A* pathfinding
|   |-- Network/               # [DEPRECATED] Multiplayer networking
|   |-- PrefabData/            # Visual cue data containers
|   |-- SceneManager/          # In-combat scene management
|   |-- Scriptable/            # ScriptableObject definitions (ImprovedActionStat)
|   |-- Turn/                  # Turn management (HandleTurnNew is active)
|   |-- TurnTimer/             # Turn countdown timer
|   |-- UltimateSystem/        # Ultimate ability system
|   |   |-- UI/
|   |-- Wave System/           # Multi-wave encounter system
|-- Sounds/                    # Audio management
|-- VFX/                       # VFX control scripts
|-- Weapon/                    # Weapon type management
```
