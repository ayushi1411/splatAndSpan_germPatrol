# Splat & Span: Germ Patrol
## Production-Grade Game Design Document (GDD) & Requirements

This document outlines the detailed system requirements, physics configuration, combat math, enemy AI behaviors, level layouts, audio design, and spritesheet specifications for **Splat & Span: Germ Patrol**.

---

## 1. Game Overview & Core Loop
*   **Genre:** 2D Top-Down Micro-Biological Shoot 'Em Up.
*   **Perspective:** 2D Orthographic (Top-Down microscope-style view looking down onto a biological cell plane).
*   **Target Engine:** Unity 2021.3 LTS / 2022.3 LTS.
*   **Core Loop:**
    ```mermaid
    graph TD
        A[Enter Level] --> B[Navigate Maze/Hazards]
        B --> C[Defeat Pathogens & Collect Elements]
        C --> D[Rescue Host Cells]
        D --> E[Obtain Boss Perks]
        E --> F[Defeat Parasite Core]
    ```

### 1.1 Sound Effects (SFX) & Audio Specification
To achieve a highly satisfying "juiced" game feel, audio is designed with distinct biological and mechanical feedback cues:
*   **Sanitization Foam Spray:** A soft, continuous, wet "pfft-sputter" bubble spraying sound loop.
*   **Foam Bubbles Popping:** High-frequency, crisp, tiny "plip" and "plop" sounds firing in rapid succession.
*   **Solvent Spray:** A high-pressure, continuous spray hiss combined with a low sizzling/fizzing chemical dissolution sound.
*   **Concentrated Bleach:** A loud, rushing wave-wash splash ("WHOOSH-SPLAT") followed by a crackling chemical sizzle.
*   **Suction Mode (Vacuum Cleaner):** A deep mechanical hum that dynamically rises in frequency/pitch from low to mid-range as collectibles are pulled closer, culminating in a wet suction "shloop" when items are consumed.
*   **Vacuum Overheat Alarm:** A harsh double alarm buzzer ("BZZT-BZZT") followed by a steam venting release hiss ("HSSSSS").
*   **Shield Deflection:** A high-frequency metallic or glass-like shimmer when absorbing damage.
*   **Shield Pop (Break):** A clean, resonant pop of a large bubble bursting.
*   **Buster Damage Response:** A squeaky rubber-toy squish sound when taking direct health damage.
*   **Ally Rescued Fanfare:** A short, upbeat 8-bit biological electronic jingle.
*   **Level Completed Fanfare:** A triumphant synth fanfare combined with bubble-popping flourishes.
*   **Game Over Sound:** A descending low-frequency synth drone that dissolves into popping bubbles.
*   **Backpack Upgrade Collected SFX:** A powerful mechanical clanking sound followed by a pressurized steam hiss ("CLANG-PFFT"), indicating the expansion of Buster's chemical tanks.

---

## 2. Player Specification & Input Mapping

The player controls **Buster the Micro-Janitor (Sani-Sentry V4)**, a whimsical bio-engineered single-cell sanitation worker wearing a tiny backward blue cap, equipped with a high-pressure hose and a suction tank backpack to clean up pathogens.

### 2.0 Biological Entities & Collectibles Directory

#### Collectibles
1.  **Carbon (C) Core:** Glowing blue-silver atom. Drifts slowly. Fuel for Solvent.
2.  **Oxygen (O2) Capsule:** Glowing soft coral-pink capsule. Fuel for Solvent and Bleach charging.
3.  **Hydro-Restore (H2O) Droplet:** Bright cyan liquid droplet. Restores 25 Health (only consumed if Buster is below 100 HP).
4.  **Bio-Matter (BM):** Bright green-yellow sparkle. Accumulates score.
5.  **Hyper-Capacity Backpack Upgrade:** A bulky, glowing double-cylinder canister item. Increases Buster's maximum Carbon and Oxygen storage limits permanently. *No elements are refilled on pickup; players must harvest to fill the expanded tank.*

#### Enemies
1.  **Amoeba:** Purple, blob-like pathogen. Chases the player to deal contact damage. Splits into 3 Mini-Amoebas on death.
2.  **Spore-Bomber:** Stationary purple flower-like turret. Shoots circular spore bombs that split into slow-moving spores.
3.  **Viral Capsid:** Heavily armored orange-pink capsid charger. Invulnerable from the front, vulnerable from the rear.
4.  **Parasite Core:** Pulsing red-black heart-like nucleus. Spits spiral waves, acid rain, and spawns organic walls.

### A. Input Configurations & Vacuum Mechanics
*   **Movement:** WASD or Arrow Keys. Controls 8-way movement via `Input.GetAxisRaw`.
    *   *Facing Direction:* Buster's base body sprite faces and flips horizontally/vertically to match the current movement vector.
*   **Aiming:** Mouse Cursor. Buster's dual-spray cleaning nozzle rotates independently in 360 degrees to point at the cursor position in world space.
    *   *Rationale:* Decoupling movement facing from aiming nozzle enables a true twin-stick shooter combat flow. The player can walk backward or retreat from chasing pathogens while continuing to shoot them from the front.
*   **Primary Shot (Sanitization Foam):** Left Mouse Button (Hold for continuous fire). Free.
*   **Fusion Shot (Solvent Spray):** Right Mouse Button (Hold for continuous beam). Costs Carbon and Oxygen.
    *   *Rationale:* Placing both offensive spray weapons on Left and Right Mouse clicks streamlines combat. The player does not swap weapons; they simply click the corresponding mouse button to fire.
*   **Suction Mode (Vacuum Cleaner):** Spacebar (Hold to activate suction cone).
    *   *Passive Collection Vacuum:* Buster automatically pulls in molecular elements (Carbon, Oxygen) and Bio-Matter within a 5-unit radius. No keys needed.
    *   *Active Pathogen Vacuum:* Holding Spacebar activates a 60-degree suction cone extending 6 units in front of Buster.
        *   It pulls in collectibles at triple velocity (even through solid walls).
        *   It pulls small enemies (e.g. mini-amoebas) toward Buster at 4 units/sec, disabling their attacks and dealing 5 damage/sec while trapped in the suction flow.
        *   *Vulnerability:* While active vacuuming is held, Buster cannot fire Foam or Solvent Spray, and his nozzle is locked to the vacuum angle.
    *   *Vacuum Heat & Overheat Mechanic:* Activating the active vacuum increases the Vacuum Heat gauge (starts at 0, max 100) at +25 heat/second. Continuous use for 4.0 seconds causes an Overheat.
        *   *Overheat Penalty:* Locks the vacuum for 2.0 seconds (gauge flashes red, suction is disabled). Buster's movement speed is reduced by 25% due to steam venting.
        *   *Cooling:* Releasing the Spacebar allows the vacuum to cool down passively at -33.3 heat/second.
*   **Ultimate Attack (Concentrated Bleach):** Q Key (Press when Bleach Charge is 100%).
    *   *Charging Bleach:* Collecting molecular elements charges the Bleach tank by +2% each. Swallowing mini-amoebas via the active vacuum charges the tank by +10%. Defeating standard pathogens charges it by +5%.
    *   *Effect:* Instantly discharges a massive, screen-sweeping high-velocity piercing spray wave (wide blue-white visual) that penetrates up to 3 enemies, dealing 150 damage and dissolving all standard organic blockages. Consumes 100% of charge.
*   **Pause Menu:** Escape or P.

### B. Numerical Stats & Physics Parameters
*   **Base Health (HP):** 100.
*   **Base Shield:** 100.
    *   *Damage Absorption Formula:* The Shield absorbs 80% of incoming damage; the remaining 20% bleeds through to Health.
        *   *Formula:* $\text{Shield Damage} = \text{Damage} \times 0.80$, $\text{Health Damage} = \text{Damage} \times 0.20$.
        *   *Example:* Buster takes 20 damage from an Amoeba. The Shield takes 16 damage (dropping from 100 to 84). Health takes 4 damage (dropping from 100 to 96).
        *   *Depletion:* If the Shield is at 0, 100% of damage is deducted from Health.
    *   *Regeneration:* Regenerates at +5 units/sec after 4 seconds of not taking damage. Taking damage resets the 4-second delay.
*   **Movement Speed:** Dynamic speed (Base: 8.0 units/sec, linear drag: 3.0).
    *   *Vacuum Overheat Debuff:* Speed reduced to 6.0 units/sec (-25%) during the 2-second lockout.
    *   *Cilia Slow Debuff:* Speed reduced to 4.8 units/sec (-40%) for 1.5 seconds.
    *   *Arterial Current (Level 3):* Constant forward drift adds +3.0 units/sec to horizontal speed, but steering vertical speed is reduced by 50%.
*   **Rotational Speed:** Immediate (matches nozzle alignment to cursor position).
*   **Mass:** 1.0 (RigidBody2D setting).
*   **Healing (H2O):** Collecting a Hydro-Restore (H2O) droplet restores 25 HP.
    *   *Full HP Rule:* If Buster is already at 100 HP, H2O droplets are *not* consumed. They remain physical pickups on the floor, allowing the player to return and collect them later (tactical backtracking).
*   **Bio-Matter (Score/Currency):** Collected from neutralized pathogens, swallowed mini-amoebas, and environment deposits.

---

### C. Physics Layer Collision Matrix
To avoid unnecessary CPU cycles in Unity, collision rules are defined as follows:

| Layer Name | Collides With | Ignores |
| :--- | :--- | :--- |
| **Player** | Obstacles, Enemies, EnemyProjectiles, Collectibles | PlayerProjectiles |
| **PlayerProjectiles** | Obstacles, Enemies | Player, Collectibles |
| **Enemies** | Obstacles, Player, PlayerProjectiles | EnemyProjectiles |
| **EnemyProjectiles** | Obstacles, Player | Enemies, Collectibles |
| **Collectibles** | Player | Everything Else |

*   **Projectiles colliding with Obstacles:** Standard player and enemy projectiles are immediately absorbed and destroyed upon colliding with static obstacles (cilia, borders, cartilage walls). They do *not* bounce or reflect back, with the exception of standard foam projectiles bouncing off boss shield walls and Cholesterol Gates with a squishy splash effect (dealing 0 damage).
*   **Cross-Projectile Collisions:** Player weapons (Foam/Solvent) and Enemy projectiles do *not* collide with each other. They pass through each other to prevent the player from easily canceling enemy threats.

---

### D. UI Canvas HUD Requirements
**HUD** stands for **Heads-Up Display**. It is the screen-space UI overlay that displays Buster's real-time parameters without obstructing the central gameplay viewport.
The HUD Canvas must be anchored along the top edge of the screen to present all metrics, items, and details in a unified top-bar layout:
1.  **Top-Left Section (Status Gauges):** Stacked horizontal sliders showing player **Health (magenta)**, **Shield (electric teal)**, and **Vacuum Heat (orange)**. The vacuum heat gauge overflows and locks if held continuously for more than 4.0 seconds, requiring a 2.0-second cooldown during which the bar flashes red.
2.  **Top-Center Section (Canister & Allies):**
    *   **Active Weapon Icon:** A circular canister display showing the spray currently being fired. It remains dimmed until a mouse click is registered: highlights **Sanitization Foam** (soap bubble) on Left-Click, **Solvent Spray** (acid spray bottle) on Right-Click, and flashes bright yellow when the **Concentrated Bleach** ultimate is fully charged and ready (Q key).
    *   **Friend Status Icons:** Translucent grey icons representing Lymphocyte and WBC Tank. They light up bright green when rescued, signifying their presence and support in the current level and the final boss fight.
3.  **Top-Right Section (Inventory & Score):** Dual element counters showing Carbon (C) and Oxygen (O2) quantities out of dynamic max capacities (initially 60 C and 30 O2, upgrading up to 100 C and 50 O2), alongside the rolling Bio-Matter score counter.

---

## 3. Combat Mechanics & Element Fusion

The player collects atomic components to fuel advanced chemical sanitation solutions.
*   **Starting Balance:** Buster starts Level 1 with 0 Carbon and 0 Oxygen, requiring immediate harvesting. In Levels 2-4, Buster carries over his end-of-level inventory.
*   **Inventory Capacity & Ratios:**
    *   To prevent wasted element storage, Buster's backpack maintains a strict **2:1 ratio of Carbon to Oxygen**, aligning with the chemical usage cost of Solvent Spray (2 Carbon + 1 Oxygen per second).
    *   **Base Backpack Capacity:** Holds up to **60 Carbon (C) and 30 Oxygen (O2)**.
    *   **Hyper-Capacity Upgrade Items:** Spawns dynamically. Up to 2 upgrades can be collected throughout the game, with each upgrade increasing maximum limits by **+20 Carbon and +10 Oxygen** (maintaining the 2:1 ratio).
        *   *No Free Refill:* Upgrades only expand capacity; they do *not* refill Buster's current inventory. The player must actively harvest Carbon and Oxygen to fill the newly expanded tank space.
        *   *Max Possible Capacity:* **100 Carbon and 50 Oxygen** (achieved after collecting both upgrades).
        *   *Dynamic Spawner Rules:* Upgrades do not spawn at fixed level spots. Instead, the GameManager dynamically allocates the 2 available upgrades to spawn in random secret alcoves, behind breakable protein cages, or behind Cholesterol Gates across **any of the first three levels (Levels 1, 2, or 3)**.
*   **Harvesting:** Pathogen kills drop elements (60% Carbon drop rate, 40% Oxygen drop rate). Static molecular cluster nodes can be shot to drop 3-5 elements.

```
       [Carbon: C]  +  [Oxygen: O2]  ===>  [Solvent Spray]
       (Cost: 2 C)        (Cost: 1 O2)        (Continuous Melt)
```

### Weapon System Blueprint

| Weapon Mode | Element Cost | Base Damage | Fire Rate (Shots/Sec) | Velocity | Special Effect / Input Mapping |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Sanitization Foam** | Free | 10 | 6.0 | 15.0 | Dense soap bubbles that coat enemies; slows target movement speed by 30% for 2.0 seconds. Activated via **Left Mouse Button (Hold)**. |
| **Solvent Spray** | 2 Carbon + 1 Oxygen per second | 45 (DPS) | Continuous beam | 20.0 | High-pressure green chemical spray; melts organic blocks and does 3x damage to Boss Shields. Activated via **Right Mouse Button (Hold)**. |
| **Concentrated Bleach** | 100% Bleach Charge | 150 | 1.0 (Instant sweep) | 25.0 | High-velocity piercing spray wave; passes through up to 3 enemies and clears obstacles. Activated via **Q Key (Press)**. |

---

## 4. Detailed Level Designs

> [!NOTE]
> **Measurement Convention:** All dimensional values listed as "units" refer to Unity's spatial coordinate system. Under the standard 2D configuration (where Pixels Per Unit is set to 32 or 64), **1 Unity world unit corresponds to 1 grid cell tile or 1 meter in physics calculations**.
> All levels display a **Level Intro Dialog (Mission Card)** at start-up, which pauses the game and presents level objectives, hazards, and mechanics. Clicking the "Start Mission" button dismisses the card and resumes gameplay.

### 🧫 Level 1: The Petri-Dish Primordial (Playtime: ~3 Minutes)
*   **Environment & Art Style:** Translucent agar gel cells, glowing green mitosis rings, and rigid brown cellular tissue walls.
*   **Perspective:** 2D Top-down microscope view.
*   **Level Boundaries:** 500 units wide x 80 units high.
*   **Camera Configuration:** Cinemachine Virtual Camera tracking the Player with soft dead zones (0.1 width/height) and camera limits confined using a PolygonCollider2D boundary shape.
*   **Enemies & Wave Spawning:**
    *   Total Enemies: 15 standard Amoebas.
    *   *Spawner Triggers:* Located at x = 100, 200, 320, and 410. Crossing a trigger spawns a wave of 3-4 Amoebas from random offsets around the corridor edges.
*   **Collectibles & Upgrades:** 
    *   4 static **Molecular Nuclei** (collectible clusters) placed at x = 150, 280, 350, 440. Shooting them splits them into 3 Carbon and 2 Oxygen pickups. 3 H2O health droplets are tucked away in side niches.
    *   *Backpack Upgrade Spawning:* A Backpack Upgrade has a chance to spawn dynamically inside a hidden protein cell cage at (220, 25).
*   **Functional Level Requirements:**
    1.  **S-Curve Pathing:** The corridor layout must force the player to snake left-to-right-to-left. Corridor width must not exceed 12 Unity world units (equivalent to 12 grid tiles) to ensure the split Amoeba creates congestion. The player must use Sanitization Foam to slow down the split swarms.
    2.  **Tissue Walls:** Painted on a Tilemap with a TilemapCollider2D and CompositeCollider2D (geometry type set to Outlines).
    3.  **Cell Cages:** Pulsing protein structures containing a **Lymphocyte Friend**. Rescuing the Lymphocyte requires breaking a 150 HP Protein Cage at x = 200. The Lymphocyte then follows Buster, shooting slow-debuffing cyan Antibody bubbles at enemies, and stands ready to cast a 4.0-second Bubble Shield during the Level 4 Boss phase.
    4.  **Petri Acid Pools:** Floor hazard zones painted on a separate tile layer. Triggers `OnTriggerStay2D` to deal 5 damage/sec to the player's shield. Buster can use the active Vacuum (Spacebar) to safely pull elements out of acid pools.
    5.  **Exit Gate Condition:** A glowing membrane portal situated at (490, 40). Reaching it loads Level 2.

---

### 🫁 Level 2: The Respiratory Spore-Fields (Playtime: ~4 Minutes)
*   **Environment & Art Style:** Purple mucosal lining, swaying alveolar hair (cilia), floating pollen grains, and rising toxic lung acid.
*   **Level Boundaries:** 100 units wide x 600 units high (Vertical Climb).
*   **Camera Configuration:** Auto-scrolling camera moving upwards at a constant rate of 2.2 units/sec. The camera viewport sets a bottom kill plane. Falling below the viewport edge triggers instant death.
*   **Enemies & Wave Spawning:**
    *   Total Enemies: 8 stationary Spore-Bombers and 10 roaming Amoebas.
    *   Spore-Bombers are placed behind moving cartilage blocks at y = 120, 250, 380, and 490.
*   **Collectibles & Upgrades:** 
    *   15 Carbon, 10 Oxygen, and 5 H2O droplets placed near swaying Cilia hazards.
    *   *Backpack Upgrade Spawning:* A Backpack Upgrade has a chance to spawn dynamically in a side airway alcove at coordinates (35, 420) guarded by Spore-Bombers.
*   **Functional Level Requirements:**
    1.  **Vertical Force (Wind):** Area effectors (Wind Zones) apply a constant downward acceleration force of 5.0 m/s² to Buster.
    2.  **Swaying Cilia:** Dynamic obstacle prefabs. Touching their triggers applies a 40% speed reduction debuff to Buster for 1.5 seconds.
    3.  **Alveolar Acid Floor:** A rising hazard plane locked to the bottom edge of the Cinemachine camera viewport. Falling into it deals 20 damage/sec.
    4.  **WBC Tank Friend:** Trapped inside a Cartilage Cage at y = 350. Rescuing the WBC Tank friend enables auto-targeting antibody missiles during the Level 4 Boss fight.
    5.  **Exit Gate Condition:** Reaching a stable epithelial platform at y = 580 triggers the transition to Level 3.

---

### 🩸 Level 3: The Capillary Expressway (Playtime: ~3 Minutes)
*   **Environment & Art Style:** Crimson flowing vessel walls, rushing red blood cell particles, cholesterol plaque blocks, and glowing DNA strands drifting in the background.
*   **Level Boundaries:** 800 units wide x 120 units high (Horizontal Split Paths).
*   **Camera Configuration:** Forward tracking camera, locking Y-axis alignment unless branching.
*   **Enemies & Wave Spawning:**
    *   Total Enemies: 15 Viral Capsids.
    *   Capsids patrol the branching corridors and charge at Buster when he enters their sightlines.
*   **Functional Level Requirements:**
    1.  **Arterial Current:** A constant push force vector (3.0, 0, 0) is applied to Buster, accelerating horizontal speed while reducing vertical steering precision by 50%.
    2.  **Procedural Branching Junctions:** At x = 250, the path splits into three channels (A, B, and C). The layout randomizes every time the level is loaded:
        *   *Swarm Channel (Path A):* Populated by dense patrols of Viral Capsids (12 capsids).
        *   *Clear Channel (Path B):* Open corridor with moderate walls, low enemy count.
        *   *Dead-End Channel (Path C):* Blocked by a solid 500 HP Cholesterol Gate. Breaking it yields a rich reward room:
            *   *Backpack Upgrade Spawning:* A Backpack Upgrade has a chance to spawn dynamically here.
            *   **Carbon-Oxygen Fusion Core:** Refills the *current* Carbon and Oxygen tanks to Buster's current maximum capacities.
            *   10 Oxygen drops and a rare H2O health pack.
    3.  **Cholesterol Gate:** Configured with the `OrganicBarrier` tag. The player must either shoot it with Sanitization Foam (absorbs 50 shots, dealing 0 damage but cracking progressively) or melt it with the Solvent Spray (melts in 3.0 seconds).
    4.  **Exit Gate Condition:** Reaching the Arterial Valve door at x = 780 loads Level 4.

---

### 👑 Level 4: The Boss Arena (Playtime: ~3-5 Minutes)
*   **Environment & Art Style:** Giant pulsing cell nucleus chamber, glowing DNA strands, and shifting chromatin walls.
*   **Level Boundaries:** Single-screen room: 48 units wide x 27 units high.
*   **Camera Configuration:** Fixed room camera (zero movement).
*   **Enemies & Boss Combat Math:**
    *   **Parasite Core Boss:** HP 1500, Shield 500.
    *   *Solvent usage:* Spraying Solvent (Acid) deals 45 DPS to health and 3x (135 DPS) to shields.
        *   Melting Phase 1 Boss Shield (500 HP) takes $\approx 3.7$ seconds (costs 7.4 C and 3.7 O2).
        *   Melting Phase 2 Boss Walls (2 walls, 100 HP each) takes $\approx 4.4$ seconds (costs 8.8 C and 4.4 O2).
        *   Melting Boss Health (1500 HP) using Solvent exclusively takes $\approx 33.3$ seconds (costs 66.6 C and 33.3 O2).
        *   *Total Element Cost:* 83 Carbon and 41.5 Oxygen. This fits perfectly within the upgraded **100 Carbon / 50 Oxygen** capacity, rewarding exploration. Players with base capacity must supplement combat using Sanitization Foam (free) and actively harvest elements during the fight.
    *   **No Soft-Lock Replenishment Loop:** To prevent a player from running out of fuel and facing a soft-lock, Level 4 includes dynamic resource spawns:
        1.  **Molecular Recyclers:** Fixed structures in the top-left and top-right corners of the arena. Shooting or vacuuming them spawns 5 Carbon and 3 Oxygen elements every 15 seconds.
        2.  **Amoeba Waves:** The boss periodically spawns 3 Amoebas. Defeating them drops elements (60% C, 40% O2) which the player can vacuum.
*   **Functional Level Requirements:**
    1.  **Shifting Layouts:** The boss core triggers structural changes:
        *   *State 1 (Phase 1, 100%-60% HP):* Open arena layout.
        *   *State 2 (Phase 2, 60% HP):* Boss grows two vertical walls at x = 16 and x = 32, dividing the arena into three compartments. The player is confined to one unless they destroy the walls.
    2.  **Organic Walls:** Spawned dynamically. Must only take damage from the Solvent Spray (Acid) projectile type. Standard foam attacks bounce off with zero impact.
        *   *Wall Visual Feedback:* Walls do not have health bars. Instead, they use a cracking sprite system: displaying 3 stages of cracking textures (Healthy, Cracked, Crumbling) until destroyed.
    3.  **Friend Support Platforms:**
        *   *Lymphocyte (Left Corner):* Active only if rescued in Level 1. Automatically instantiates a shield bubble around the player when the boss starts casting Phase 2 acid rain.
        *   *WBC Tank (Right Corner):* Active only if rescued in Level 2. Fires targeting antibody missiles at the Boss Core.
    4.  **Win Condition:** Boss Health reaching 0 triggers an explosion animation, stops all projectile scripts, and shows the Victory UI Canvas.

---

## 5. Enemy Codex & AI Algorithms

### Standard Enemy Damage Feedback
To keep the UI clean and maximize performance, standard enemies do *not* have floating health bars.
*   **Hit Flash:** Enemies flash bright white on projectile impact.
*   **Mass Reduction:** Enemies shrink slightly in size as their HP is depleted.
*   **Pulsing State:** When health drops below 30% HP, the enemy pulses dark red and emits leaking cytolytic fluid particles.
*   **Critical Points:** Capsids and Bosses play yellow spark effects when hit in weak spots.

---

### A. The Amoeba (Level 1 & 2)
*   **Stats:** HP: 30, Move Speed: 2.5, Damage: 15.
*   **AI State Machine:**
    1.  *Patrol:* Move randomly between cellular nodes.
    2.  *Chase:* If player is within 12 units, move directly toward player's transform position.
    3.  *On Death:* Trigger split behavior. Instantiates 3 mini-amoebas (HP: 10, Speed: 4.5, Size: 0.4x).
        *   *Radial Force Offset:* The 3 mini-amoebas are ejected outwards at 120-degree angles (0°, 120°, 240°) with a physical impulse force to scatter them around the player.
        *   *Suction interaction:* Mini-amoebas can be vacuumed (Spacebar) to deal 5 damage/sec, charge the Bleach tank, and be swallowed.

### B. The Spore-Bomber (Level 2)
*   **Stats:** HP: 80, Move Speed: 0 (Stationary), Damage: 10 per spore.
*   **AI State Machine:**
    1.  *Idle:* Pulsing texture animation.
    2.  *Attack:* When player enters a distance of 18 units, begin rotating clockwise at 45 degrees/sec while spawning spore bombs from 4 nozzle directions every 0.8 seconds. Spore bombs detonate into smaller slow-moving spores on contact or after 3.0 seconds.

### C. The Viral Capsid (Level 3)
*   **Stats:** HP: 120, Move Speed: 1.5 (Base) / 12.0 (Charge), Damage: 30.
*   **AI State Machine:**
    1.  *Patrol:* Horizontal back-and-forth pathing.
    2.  *Target Locked:* Active if player is aligned along X or Y axis within a line of sight of 15 units.
    3.  *Charge:* Play charging animation (eyes turn red, flashes) for 0.5 seconds, then charge forward in a straight line. While charging, the capsid shoots secondary spike projectiles out to its left and right sides.
    4.  *Stun:* If capsid hits a wall/obstacle, it enters a stunned state for 2.0 seconds, exposing its rear glowing core.
*   **Armor Rules:** Incoming damage checks bullet collision angle against capsid forward vector. If angle is within +/- 75 degrees of front face, damage is reduced to 0 and plays a metallic spark effect. If bullet hits from the rear (+/- 45 degrees from tail), it deals 2x critical damage.

### D. The Parasite Core (Level 4 Boss)
*   **Stats:** HP: 1500, Shield: 500, Damage: 25.
*   **AI State Machine:**
    *   **Phase 1 (100% to 60% HP):**
        *   Fires spiral projectile waves (8 streams, rotating at 30 degrees/sec).
        *   Spawns amoeba waves to distract the player.
    *   **Phase 2 (60% to 0% HP):**
        *   Becomes invulnerable for 4 seconds while growing organic walls to split the arena.
        *   Launches acid rain from the ceiling (each drop deals 15 shield damage).
        *   Weak points open only when organic walls are destroyed by the player using Solvent Spray.

---

## 6. Spritesheet Specifications & Art Palette

### A. Color Palette Recommendation (Aesthetic Wow-Factor)
To achieve a high-end, premium aesthetic, use a cohesive palette of glowing neon hues over a dark, high-contrast background:
*   **Background Dark:** #090d16 (Deep Cyber-Indigo)
*   **Player Primary:** #00f2fe (Neon Cyan - Soap Bubble Glow)
*   **Player Shield:** #00ff87 (Glowing Electric Teal - Foam Defense Shield)
*   **Pathogen Red:** #ff007f (Vibrant Magenta)
*   **Acid/Environmental:** #d4ff00 (Acidic Lime-Green - Disinfectant Spray)
*   **Carbon Element:** #a6c1ee (Metallic Silver-Blue - Soap Concentrate)
*   **Oxygen Element:** #ff9a9e (Soft Coral Red - Pressurized Air)

---

### B. Unity Sprite Import Settings (Global Rules)
To maintain crisp, pixel-perfect visuals, all assets should be imported into Unity with the following settings:
*   **Texture Type:** Sprite (2D and UI)
*   **Sprite Mode:** Multiple (for sheets) or Single (for single sprites)
*   **Pixels Per Unit (PPU):** 32 (for standard 32x32px tiles) or 64 (for 64x64px assets)
*   **Filter Mode:** Point (No Filter) — Critical to prevent blurry pixels.
*   **Compression:** None (keeps colors vibrant and glowing).

---

### C. Spritesheet Specifications Tables

#### 🚀 1. Player & Combat Effects Sheets

| Asset Filename | Slicing Mode | Cell Size | Total Frames | Frame-by-Frame Breakdown | Unity Pivot / Collision Offset |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `buster_janitor_sheet.png` | Multiple | 64x64px | 9 | **0-3:** Determined wiggle movement (idle)<br>**4-5:** Vacuum suction intake recoil<br>**6-7:** Backward cap adjust turn<br>**8:** Flattened squeegee squish (damage flash) | Center (0.5, 0.5) / Circle Collider radius 0.4 |
| `janitor_gear_sheet.png` | Multiple | 32x32px | 8 | **0-3:** Nozzle fire flare<br>**4-7:** Vacuum attachments and suction exhaust particles | Left Center (0.0, 0.5) for proper hose attachment |
| `vacuum_suction_sheet.png` | Multiple | 64x64px | 12 | **0-3:** Suction cone wind spirals<br>**4-7:** Trapped debris swirl rotation<br>**8-11:** Micro-vacuum vortex pop | Left Center (0.0, 0.5) to project from nozzle |
| `sanitization_foam_sheet.png` | Multiple | 32x32px | 16 | **0-3:** Foam bubble launch/grow<br>**4-7:** Splattering foam impact carpet<br>**8-11:** Foam bubbles popping/decaying<br>**12-15:** Slow-debuff overlay texture for enemies | Center (0.5, 0.5) / Custom Circle Collider |
| `solvent_spray_sheet.png` | Multiple | 64x64px | 12 | **0-3:** Solvent high-pressure stream loop<br>**4-7:** Corrosive green splash droplets<br>**8-11:** Dissolving sizzle smoke puff | Left Center (0.0, 0.5) / Box Collider 2D |
| `bubble_shield_sheet.png` | Multiple | 128x128px | 8 | **0-3:** Concentrated soap foam dome loop<br>**4-5:** Bubble wiggle impact deflection<br>**6-7:** Bubble pop decay | Center (0.5, 0.5) |

---

#### 🦠 2. Enemies & Hazards Sheets

| Asset Filename | Slicing Mode | Cell Size | Total Frames | Frame-by-Frame Breakdown | Unity Pivot / Collision Offset |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `amoeba_sheet.png` | Multiple | 64x64px | 12 | **0-3:** Pulsing Swim cycle<br>**4-9:** Cell Division / Splitting transition<br>**10-11:** Death Dissolve | Center (0.5, 0.5) / Capsule Collider 2D |
| `spore_bomber_sheet.png` | Multiple | 96x96px | 8 | **0-3:** Core Rotate Loop (dormant)<br>**4-7:** Spike expansion & bullet nozzle charge-up | Center (0.5, 0.5) / Static Circle Collider |
| `viral_capsid_sheet.png` | Multiple | 64x64px | 12 | **0-3:** Heavy Crawl cycle<br>**4-7:** Dash Charging flare<br>**8-9:** Wall Bounce Stun rotation<br>**10-11:** Disintegration death | Center (0.5, 0.5) / Box Collider 2D (Front shield offset) |
| `parasite_boss_sheet.png` | Multiple | 256x256px | 16 | **0-3:** Heart Beat Core Loop (Phase 1)<br>**4-7:** Tentacle Growths & Wall Sprouting posture (chromatin walls)<br>**8-11:** Acid Rain Spit action<br>**12-15:** Core Detonation (Phase 2 death) | Center (0.5, 0.5) / Dynamic Composite Colliders |

---

#### 📦 3. Environment, Collectibles & UI Sheets

| Asset Filename | Slicing Mode | Cell Size | Total Frames | Frame-by-Frame Breakdown | Unity Pivot / Collision Offset |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `petri_dish_tileset.png` | Grid (32x32px) | 32x32px | 32 | **0-3:** Floor Agar variations<br>**4-11:** Border Cell Walls (Tilemap compatible)<br>**12-15:** Breakable Protein Cage walls<br>**16-19:** Lymphocyte (Friend) static frame loop<br>**20-23:** Glowing background DNA strands | Custom Tilemap Slicing / Grid Pivot Center |
| `lungs_tileset.png` | Grid (32x32px) | 32x32px | 24 | **0-5:** Moving Airway cartilage blocks<br>**6-11:** Cilia strands (Waving animation loop)<br>**12-17:** Acid Alveoli hazard top fluid loop | Bottom Center (0.5, 0.0) for Cilia strands to pivot |
| `capillary_tiles.png` | Grid (64x64px) | 64x64px | 20 | **0-3:** Deep Red Vessel walls<br>**4-7:** Cholesterol Plaque blocks (breakable)<br>**8-11:** Red Blood Cell background drifting loop<br>**12-15:** Glowing capillary DNA strand decals | Center (0.5, 0.5) |
| `collectibles_sheet.png` | Multiple | 32x32px | 36 | **0-7:** Carbon (C) Core spin loop (blue-silver)<br>**8-15:** Oxygen (O2) Capsule float loop (soft pink)<br>**16-23:** Hydro-Restore (H2O) droplet (cyan)<br>**24-31:** Bio-Matter currency sparkle (green-yellow)<br>**32-35:** Hyper-Capacity Backpack Upgrade float | Center (0.5, 0.5) / Circle Collider triggers |
| `ui_elements_sheet.png` | Custom Slices | Variable | - | **N/A:** HUD Health/Shield bar panel frame, elements sliders, Lymphocyte/WBC Tank status icons, vacuum heat slider, Main Menu buttons (Play, Level Select, How to Play, Quit), Locked/Unlocked padlock icons, Control keys graphics, **Level Intro Card briefing frames**, **Game Over / Victory overlay frames**, and Retry buttons. | Set slice margins (9-slicing enabled) |

---

## 7. Main Menu & Game Navigation Flow

To provide a complete player loop, the game incorporates a dedicated Main Menu Scene with full UI panel routing.

### A. Main Menu Structure
A Canvas overlay containing the following active UI panels:
1.  **Main Panel:**
    *   **Title:** "Splat & Span: Germ Patrol" in glowing cyan/magenta lettering.
    *   **Play Button:** Loads Level 1 directly.
    *   **Level Select Button:** Shows the Level Select Panel.
    *   **How To Play Button:** Shows the Instructions Panel.
    *   **Quit Button:** Quits the application (calls Application.Quit).
2.  **Level Select Panel:**
    *   **Layout:** Grid display with 4 buttons corresponding to Levels 1, 2, 3, and 4 (Boss).
    *   **Lock State & Replay Rules:**
        *   Level 1 is always unlocked. Levels 2, 3, and 4 are unlocked sequentially upon level completion.
        *   *Replay Rules:* Players can replay any completed level. However, **inventory and friend counts do not accumulate**.
        *   When replaying, the player's starting inventory is reset to that level's defaults, and completing the level overwrites the status. For example, rescuing a Lymphocyte sets the `LymphocyteRescued` flag to `true` (max 1), preventing duplicate allies. High scores are updated, but currency is not accumulated across multiple replays.
    *   **Back Button:** Returns to the Main Panel.
3.  **How to Play (Instructions) Panel:**
    *   **Visual Guide & Controls Description:**
        *   **WASD / Arrows:** Move Buster the Sani-Sentry (character body faces movement).
        *   **Mouse Cursor:** Aim cleaning nozzle.
        *   **Left-Click (Hold):** Spray Sanitization Foam (slows pathogens).
        *   **Right-Click (Hold):** Spray Solvent (dissolves organic barriers & shields). *Consumes Carbon & Oxygen.*
        *   **Spacebar (Hold):** Active Vacuum (pulls elements & mini-amoebas; builds heat).
        *   **Q Key:** Fired Concentrated Bleach (Ultimate - active at 100% Bleach charge).
        *   **Backpack Upgrades:** Hidden canisters can spawn in Levels 1-3. Collecting an upgrade expands Buster's maximum Carbon and Oxygen capacities permanently. *Upgrades do NOT automatically refill the tanks; players must harvest and vacuum elements to fill the expanded tank spaces.*
    *   **Back Button:** Returns to the Main Panel.
4.  **Game Over & Victory Canvas overlays:**
    *   *Game Over Screen:* Triggered when HP drops to 0. Displays a "DEFEATED" title, final score, and buttons to Retry (reloads current level) or return to Main Menu.
    *   *Victory Screen:* Triggered when Level 4 Boss is defeated. Displays a "VICTORY" banner, final Bio-Matter score, and buttons to Replay or return to Main Menu.

### B. Scene Loading Logic
The menu is controlled by a central UI manager that interfaces with Unity's SceneManager API:
*   Scene 0: `MainMenu`
*   Scene 1: `Level_1_PetriDish`
*   Scene 2: `Level_2_Lungs`
*   Scene 3: `Level_3_Capillaries`
*   Scene 4: `Level_4_BossArena`

---

## 8. Terminology & Glossary
*   **HUD (Heads-Up Display):** A graphical screen overlay displaying player statistics (Health, Shield, heat, items) during gameplay.
*   **DPS (Damage Per Second):** A metric indicating cumulative damage dealt to a target held in a continuous stream (like Solvent Spray) for one second.
*   **Debuff:** A temporary negative status effect applied to the player or enemies (e.g. slow, vacuum overheat lockout).
*   **Kill Plane:** An invisible boundary line locked to the bottom of Level 2. Falling below it triggers instant death.
*   **Corridor:** A narrow pathway bounded by solid structural cells that constrains Buster's movement.
*   **Cilia:** Animated hair-like cells in Level 2 that trigger a 40% speed slow.
*   **Chromatin / DNA Strands:** Shifting biological structures used as level obstacles and background decorations.
