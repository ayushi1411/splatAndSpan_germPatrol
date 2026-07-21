# Implementation Plan - Splat & Span: Germ Patrol

This plan outlines the design and C# scripts implementation for the Unity 2D shooting game based on the detailed requirements.

---

## User Review Required

> [!IMPORTANT]
> The game requires specific Unity setup components (such as Cinemachine, 2D Tilemap Editor, and Physics2D collision layers). We will generate the modular C# scripts, and you will import them into your Unity project and assign them to your GameObjects.

> [!NOTE]
> We will create a clean folder structure within the workspace (`Assets/Scripts`, etc.) so you can copy the entire Assets directory directly into your Unity project.

---

## Proposed Changes

We will create a structured set of C# scripts in the workspace `c:\Users\ayushi\splatAndSpan_germPatrol` to represent all player systems, enemy behaviors, collectibles, and level logic.

### Directory Structure to Create
*   `Assets/Scripts/Player/`
*   `Assets/Scripts/Enemies/`
*   `Assets/Scripts/Collectibles/`
*   `Assets/Scripts/Environment/`
*   `Assets/Scripts/UI/`
*   `Assets/Scripts/Core/`
*   `Assets/Scripts/Tests/`
*   `Assets/Sprites/` (contains generated PNG sheets)

---

### Component 1: Player & Weapon Systems

#### [NEW] [PlayerController.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Player/PlayerController.cs)
*   **Movement:** 8-way WASD/Arrow movements using rigid body velocity. The base character body sprite flips/faces the movement vector.
*   **Aiming:** Nozzle/arm rotates independently in 360 degrees, aligning with the mouse cursor position in world space.
*   **Shield Absorption Math:** Absorbs 80% of incoming damage; 20% is applied to Health. If shield is 0, 100% goes to Health.
    *   *Regeneration:* After 4.0 seconds of not taking damage, recharges at +5 points/sec.
*   **Collectibles Inventory & Limits:** Holds Carbon and Oxygen.
    *   *Dynamic Capacities:* Starts with base `maxCarbon = 60` and `maxOxygen = 30` (strict 2:1 ratio).
    *   *Upgrade Method:* `UpgradeBackpack(int carbonInc, int oxygenInc)` increases limits (e.g. by +20 C / +10 O2, up to 100 C / 50 O2 maximum). *This method only changes the maximum limit values; it does NOT alter the current element stocks.* Communicates changes to `HUDController` to expand bar scales.
    *   *Health Pickup (H2O):* If health is exactly 100, passes over droplet without consuming it (allows backtracking).
*   **Suction Mode (Vacuum):**
    *   *Passive Magnet:* Automatically draws in elements and Bio-Matter within 5 units.
    *   *Active Vacuum (Spacebar Hold):* Activating Spacebar projects a 60-degree, 6-unit trigger cone. Pulls elements at triple speed through walls. Pulls Mini-Amoebas at 4 units/sec, deals 5 damage/sec, and disables enemy attacks.
    *   *Heat System:* Active vacuum generates +25 heat/sec. Overheats at 100 heat (4.0s of continuous hold).
    *   *Overheat lockout:* Locks active vacuum for 2.0s, flashes HUD bar red, and debuffs Buster's movement speed by 25% due to steam venting. Releases Spacebar to passively cool at -33.3 heat/sec.

#### [NEW] [PlayerWeapon.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Player/PlayerWeapon.cs)
*   **Controls consolidated on mouse clicks:**
    *   *Left Mouse Button (Hold):* Fires Sanitization Foam (free, slows enemies by 30% for 2.0 seconds).
    *   *Right Mouse Button (Hold):* Fires Solvent Spray (costs 2 C and 1 O2 per second, continuous acid beam, melts organic barriers and deals 3x boss shield damage).
    *   *Q Key (Press):* Discharges Concentrated Bleach ultimate when Bleach Charge is 100%. Sweeps a wide piercing wave (150 damage) through enemies and barriers.
*   **Bleach Tank Charging:** Charges by +2% per element, +5% per standard pathogen killed, and +10% per Mini-Amoeba vacuumed.
*   **HUD Highlighting:** Calls the HUD UI controller to highlight active canisters (Foam, Solvent, Bleach) dynamically on trigger discharge.

#### [NEW] [Projectile.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Player/Projectile.cs)
*   Handles speed, damage, and collisions for Foam, Solvent, Spore bombs, Acid Rain drops, and Capsid spikes.
*   *Wall Absorption:* Destroyed immediately on collision with boundaries/obstacles. Foam projectiles bounce off boss walls and Cholesterol Gates with 0 damage and play bubble splash particles.
*   Does *not* collide with other projectiles (passes through).

---

### Component 2: Enemies & AI Systems

#### [NEW] [AmoebaEnemy.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Enemies/AmoebaEnemy.cs)
*   Patrols randomly, chases Buster when within 12 units.
*   *On Death Split:* Spawns 3 Mini-Amoebas. Applies a physical impulse force at 120-degree radial offsets (0°, 120°, 240°) to scatter them around the death center.
*   *Damage Response:* Flashes white and shrinks slightly in size to represent mass loss. Pulses dark red and leaks fluid when below 30% HP. No floating health bars.

#### [NEW] [SporeBomberEnemy.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Enemies/SporeBomberEnemy.cs)
*   Stationary turret. Spins at 45 degrees/sec clockwise.
*   When Buster is within 18 units, fires spore bombs in 4 directions every 0.8 seconds. Spore bombs detonate on proximity/impact, splitting into 4 slow-moving spores.

#### [NEW] [ViralCapsidEnemy.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Enemies/ViralCapsidEnemy.cs)
*   Armored charger. If aligned horizontally/vertically with Buster within 15 units, flashes red/charges.
*   *Armor Shield Check:* Checks projectile collision angle against forward direction. Within +/- 75 degrees of face, damage is 0 (metallic spark). Rear collision (+/- 45 degrees of tail) deals 2x critical damage.
*   Shoots side-spike projectiles left/right while charging. Enters a 2.0-second stun (core exposed) if colliding with obstacles.

#### [NEW] [ParasiteBoss.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Enemies/ParasiteBoss.cs)
*   Boss controller with Phase 1 (rotating spiral sprays, amoeba spawns) and Phase 2 (spawns chromatin walls, acid rain).
*   Coordinates boss health HUD bar (Teal Shield: 500 HP, Magenta Health: 1000 HP).
*   Enables Lymphocyte (bubble shield) and WBC Tank (seeking missiles) support platforms if rescued in Level 1 & 2.

---

### Component 3: Collectibles & Environment

#### [NEW] [Collectible.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Collectibles/Collectible.cs)
*   Rotates, hovers, and pulls toward the player (passive/active vacuum).
*   Grants C, O2, HP, or Bio-Matter score on trigger overlap.
*   *BackpackUpgrade type:* Triggers `UpgradeBackpack(20, 10)` on player (modifying max caps only; leaving current stock level values unchanged), plays heavy clanking SFX, and triggers HUD capacity flashing animations.

#### [NEW] [RescuedFriend.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Collectibles/RescuedFriend.cs)
*   Rescued by breaking a 150 HP Protein Cage. Sets boolean flags (`LymphocyteRescued = true`, `WBCRescued = true`) in the GameManager.

#### [NEW] [OrganicWall.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Environment/OrganicWall.cs)
*   Spawns as boss barriers or Level 3 Cholesterol Gates.
*   *Cracking Visuals:* Updates Sprite renderer based on HP (100-67% Healthy, 66-34% Cracked, 33-0% Crumbling).
*   *Acid Vulnerability:* Takes damage *only* from Solvent Spray (or bleach). Foam projectiles bounce off with 0 damage.

#### [NEW] [AutoScroller.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Environment/AutoScroller.cs)
*   Scrolls Level 2 camera vertically at 2.2 units/sec.
*   Coordinates the bottom kill plane, dealing instant fatal damage if Buster falls off the bottom screen edge.

#### [NEW] [MolecularRecycler.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Environment/MolecularRecycler.cs)
*   Stationary nodes placed in Level 4 corners.
*   Periodically spawns 5 Carbon and 3 Oxygen collectibles every 15 seconds. Fades and pops outward, allowing the player to harvest elements mid-boss fight.

---

### Component 4: Core & Game Management

#### [NEW] [GameManager.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Core/GameManager.cs)
*   Singleton coordinating level transitions, persistent saves, and unlocked level select grids.
*   *Capacity Persistence:* Saves current `maxCarbon` and `maxOxygen` values so that backpack upgrades carry over across levels.
*   *Dynamic Backpack Spawning:* Manages dynamic spawner parameters. Decides whether to spawn a Backpack Upgrade, randomly selecting from qualifying spawner nodes in Levels 1-3 (capping total spawned upgrades throughout a full game run at 2).
*   *Replay Lock/Reset Rules:* Restores starting level default inventories (base 60 C / 30 O2, unless replaying Level 3/4 where upgrades found in previous levels are maintained) and prevents stacking bio-matter or duplicated rescued friends on replays. Overwrites high scores.

#### [NEW] [AudioManager.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Core/AudioManager.cs)
*   Singleton controlling playbacks of looping mechanical sound effects (spray hiss, suction hums) and one-shot sound effects (bursts, damage squeaks, alarms, fanfares, upgrade clanks).

#### [NEW] [EnemySpawner.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Environment/EnemySpawner.cs)
*   Triggers randomized wave spawnings (randomized coordinates and enemy counts) within corridors to increase replayability.

#### [NEW] [HUDController.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/UI/HUDController.cs)
*   Updates top-left HP/Shield/Heat sliders, highlights weapon canisters, and displays count numbers.
*   *Dynamic Limits Display:* Sets Carbon and Oxygen text to `current / max` and scales the physical widths of the bar frames when `UpgradeBackpack` events are fired.
*   *Level Intro Dialog (Mission Card):* Fades in a briefing card on level start, pauses Time.timeScale, and resumes play when "START MISSION" is clicked.
*   *Game Over / Victory Screens:* Displays final panels, retry buttons, and routes Scene transitions.

#### [NEW] [MainMenuController.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/UI/MainMenuController.cs)
*   Coordinates Main, Level Select, and Instructions Panels transitions using bio-wipe overlays.
*   Restricts level select buttons based on locked states.
*   *How to Play text update:* Explains dynamic Backpack Upgrades (only increases capacities, no refills, requires active harvesting).

---

### Component 5: Sprite & Visual Assets

All visual assets must be imported into Unity as multiple-frame point-filtered spritesheets under `Assets/Sprites/`:
*   `buster_janitor_sheet.png` (9 frames - idle, recoil, cap adjust, squish)
*   `janitor_gear_sheet.png` (8 frames - nozzles, particles)
*   `vacuum_suction_sheet.png` (12 frames - spirals, debris swirls)
*   `sanitization_foam_sheet.png` (16 frames - bubble launch, pops, overlays)
*   `solvent_spray_sheet.png` (12 frames - streams, splash, sizzle)
*   `bubble_shield_sheet.png` (8 frames - shield loops, breaks)
*   `amoeba_sheet.png` (12 frames - swim, division split, death)
*   `spore_bomber_sheet.png` (8 frames - turret rotations)
*   `viral_capsid_sheet.png` (12 frames - crawl, charge, stun, disintegrate)
*   `parasite_boss_sheet.png` (16 frames - phase 1 heartbeat, Phase 2 walls, spit, detonate)
*   `petri_dish_tileset.png` / `lungs_tileset.png` / `capillary_tiles.png` (Tilesets, DNA strands background decals)
*   `collectibles_sheet.png` (36 frames - C, O2, H2O, BM, Backpack Upgrades)
*   `ui_elements_sheet.png` (HUD frames, sliders, buttons, locked padlocks, briefing dialog cards, gameover/victory overlays)

---

### Component 6: Automated Unit Testing

#### [NEW] [PlayerTests.cs](file:///c:/Users/ayushi/splatAndSpan_germPatrol/Assets/Scripts/Tests/PlayerTests.cs)
*   Contains C# unit test cases utilizing NUnit in Unity Test Framework (UTF). Runs programmatically to verify core math and game rule logic.
*   **Test Cases Include:**
    *   `TestShieldAbsorption()`: Instantiates the player, applies 20 damage, and asserts that Shield decreases by 16 (80%) and Health by 4 (20%).
    *   `TestBackpackUpgrade()`: Calls `UpgradeBackpack(20, 10)` on the player. Asserts that `maxCarbon` becomes 80 and `maxOxygen` becomes 40, but current inventory levels remain unchanged.
    *   `TestH2OFullHP()`: Verifies that if player Health is 100, healing pickups do not trigger health increases and are not consumed. If Health is 80, asserts HP heals up to max 100 on collection.
    *   `TestVacuumOverheat()`: Simulates active vacuuming for 4.0 seconds, asserting that heat gauge reaches 100 and the vacuum locks out (suction state = disabled) and movement speed is reduced by 25%.
    *   `TestBleachCharge()`: Simulates collecting elements and asserts that Bleach Charge increments correctly by +2% per pickup and reaches 100%.

---

## Verification Plan

### Automated Verification
*   Open Unity and navigate to **Window > General > Test Runner**.
*   Select **PlayMode** or **EditMode** tab and click **Run All**.
*   Confirm all test cases in `PlayerTests.cs` pass (indicated by green checkmarks).

### Manual Verification in Unity
*   **Movement & Aiming:** Verify nozzle rotates to mouse cursor while body faces WASD.
*   **Backpack Upgrade Collection:** Place upgrade item in a test scene. Confirm picking it up increases limits but leaves current stocks unchanged.
*   **Molecular Recyclers:** Verify corner valves in Level 4 periodically spawn elements during the boss fight, preventing soft-locks.
*   **Level Intro Card:** Verify pop-up displays and pauses game until clicked.
