# Project-Roguelite — Architecture & Roadmap

> **Document State**: Alive roadmap — updated during development.
> **Last Updated**: 2026-09-01

---

## 1. Project Identity

| Field | Value |
|-------|-------|
| **Engine** | Unity 6.3 LTS (6000.3.11f1) |
| **Template** | Universal 2D |
| **Render Pipeline** | URP 17.3.0 — 2D Renderer |
| **Language** | C# (.NET Standard 2.1) |
| **Input** | Unity Input System 1.19.0 (New only) |
| **Color Space** | Linear |
| **Goal** | Learn software engineering through Unity |

### Visual Specs

| Property | Value |
|----------|-------|
| **Logic Resolution** | 320×180 |
| **Pixels Per Unit** | 16 PPU |
| **Filter Mode** | Point (no blur) |
| **Compression** | None |
| **Tile Size** | 16×16 |
| **Entity Size** | 32×32 |

### Branches

- `main` — stable, reviewed work
- `develop` — active integration branch

---

## 2. Architecture Overview

The project follows a **composition-over-inheritance** architecture. Entities (`Player`, `Enemy`) are thin shells that assemble behavior through `[RequireComponent]` MonoBehaviours. Data lives in ScriptableObjects. Communication uses C# `Action` events.

### Core Patterns

| Pattern | Where | Purpose |
|---------|-------|---------|
| **Entity Shell** | `Player.cs`, `Enemy.cs` | Thin wrappers; all behavior in components |
| **Strategy** | `WeaponFireStrategy` (abstract SO) → `MeleeFireStrategy`, `ProjectileFireStrategy` | Swap fire behavior per weapon without code changes |
| **Observer** | `Action` events on Health, Weapon, WeaponHolder, ModifierInventory | Decoupled cross-component communication |
| **Pipeline** | `ModifierCalculator` — Add-then-Multiply | Deterministic stat modification across providers |
| **State Machine** | `EnemyBehavior` FSM (Chasing, SeekingWeapon, Attacking) | Enemy AI decision-making |
| **Interface Segregation** | `IMoveInput`, `IInteractable`, `IModifierProvider` | Contracts with 1–2 members; classes choose what they implement |

### Directory Structure

```
Scripts/
├── Interfaces/          # Contracts (IMoveInput, IInteractable, IModifierProvider)
├── Entities/            # Composition shells
│   ├── Player/          # Player.cs
│   └── Enemy/           # Enemy.cs
├── Components/
│   ├── Base/            # Domain-agnostic reusable components
│   │   ├── HealthRelated/   # Health, HealthVisualFeedback, HealthAudioFeedback
│   │   ├── Weapon/          # WeaponPickup, WeaponDetection
│   │   ├── Interaction.cs
│   │   ├── LookDirection.cs
│   │   └── Movement.cs
│   ├── Weapons/         # Weapon system (Strategy pattern)
│   │   ├── Weapon.cs, WeaponHolder.cs, WeaponData.cs
│   │   ├── WeaponFireStrategy.cs (abstract), MeleeFireStrategy.cs, ProjectileFireStrategy.cs
│   │   ├── WeaponFireContext.cs, WeaponFireResult.cs
│   │   ├── WeaponAmmoType.cs
│   │   └── WeaponAudioFeedback.cs, WeaponVisualFeedback.cs
│   ├── Projectile/      # Projectile.cs, ProjectileFireStrategy.cs
│   ├── Modifiers/       # Stat modifier pipeline
│   │   ├── ModifierDefinition.cs (abstract), StatModifierDefinition.cs
│   │   ├── DoubleShotModifierDefinition.cs
│   │   ├── ModifierInstance.cs, ModifierInventory.cs, ModifierCalculator.cs
│   │   ├── ModifierStat.cs, ModifierOperation.cs
│   │   └── ModifierPickup.cs, TemporaryModifierPickup.cs
│   ├── Enemy/           # Enemy AI
│   │   ├── EnemyAIInput.cs, EnemyCombat.cs, EnemyWeaponDecision.cs
│   │   └── EnemyBehavior/  # EnemyBehavior.cs, EnemyState.cs
│   ├── PlayerInput/     # Unity Input System bridge
│   │   ├── PlayerInput.cs, AimDevice.cs
│   └── Projectile/      # Projectile + ProjectileFireStrategy
├── UI/
│   └── WeaponAmmoUI/    # WeaponAmmoUI.cs
└── Debug/               # Debug loggers and testers
    ├── HealthDebugger.cs, HealthTester.cs
    ├── WeaponDebugger.cs, ModifierDebugger.cs
```

---

## 3. Systems Documentation

### 3.1 Health System

**Files**: `Health.cs`, `HealthVisualFeedback.cs`, `HealthAudioFeedback.cs`

| Component | Role |
|-----------|------|
| `Health` | Core HP container. `TakeDamage()`, `Heal()`, `RestoreFullHealth()`. Events: `DamageTaken(float)`, `HealthChanged(float,float)`, `Died` |
| `HealthVisualFeedback` | Sprite flash on damage (coroutine-based color swap) |
| `HealthAudioFeedback` | Plays damage and death audio clips via `AudioSource.PlayOneShot` |

**Design Decisions**:
- Health is clamped `[0, MaxHealth]`; `IsDead` check guards all operations
- Visual feedback uses coroutine with `StopCoroutine` to prevent stacking
- Audio requires both clips at `Awake` (fail-fast with `MissingReferenceException`)

**Status**: ✅ Complete

---

### 3.2 Movement System

**File**: `Movement.cs`

- Reads `IMoveInput.MoveInput` (works for Player and Enemy via interface)
- Applies speed through `ModifierInventory.CalculateValue` (modifiers affect movement)
- Uses `Rigidbody2D.linearVelocity` in `FixedUpdate`
- Clamps input magnitude to 1

**Status**: ✅ Complete

---

### 3.3 Look Direction System

**File**: `LookDirection.cs`

- Stores normalized `Forward` direction
- Sets `transform.rotation` via `Quaternion.Euler(0, 0, angle)` — 2D rotation around Z
- Ignores zero vectors

**Status**: ✅ Complete

---

### 3.4 Weapon System

**Core Files**: `Weapon.cs`, `WeaponHolder.cs`, `WeaponData.cs`, `WeaponFireStrategy.cs`, `WeaponFireContext.cs`, `WeaponFireResult.cs`, `WeaponAmmoType.cs`

#### Data Layer (`WeaponData` ScriptableObject)
| Property | Type | Description |
|----------|------|-------------|
| `_weaponName` | string | Display name |
| `_icon` | Sprite | UI icon |
| `_damage` | float | Base damage |
| `_fireRate` | float | Shots per second |
| `_magazineSize` | int | Magazine capacity |
| `_reloadDuration` | float | Reload time in seconds |
| `_maxReserveAmmo` | int | Max reserve ammo |
| `_weaponFireStrategy` | WeaponFireStrategy | Fire behavior SO |
| `_ammoType` | WeaponAmmoType | Magazine or Infinite |

#### Runtime (`Weapon`)
- `TryFire()` returns `WeaponFireResult` enum (Success, NotEquipped, Cooldown, NoAmmo, Reloading)
- Applies `WeaponFireModifierDefinition` modifiers to `WeaponFireContext` before strategy execution
- Calculates modified stats (Damage, FireRate, ReloadDuration) via `ModifierCalculator` with weapon + holder providers
- Tracks ammo, cooldowns via `Time.time`

#### Weapon Holder (`WeaponHolder`)
- Manages equip/drop lifecycle with socket parenting
- `Equip()` handles mutual exclusion (drops current, steals from other holders)
- Events: `WeaponChanged(Weapon)` for UI binding

#### Fire Strategies
| Strategy | Implementation |
|----------|---------------|
| `MeleeFireStrategy` | `Physics2D.OverlapCircleAll` with spread angle, deduplication via `HashSet<Health>`, minimum radius multiplier |
| `ProjectileFireStrategy` | Instantiates `Projectile` prefab per attack count with spread angle |

**Assets**:
| Weapon | Damage | FireRate | Mag | Reload | Reserve | Spread | Strategy | Ammo |
|--------|--------|----------|-----|--------|---------|--------|----------|------|
| Pistol | 10 | 3 | 7 | 2s | 21 | 10° | Projectile | Magazine |
| Colt | 5 | 10 | 30 | 5s | 90 | 70° | Projectile | Magazine |
| Mosin | 30 | 1 | 5 | 5s | 15 | 10° | Projectile | Magazine |
| Katana | 10 | 1 | 1 | 0s | 0 | 90° | Melee | Infinite |

**Status**: ✅ Complete

---

### 3.5 Projectile System

**Files**: `Projectile.cs`, `ProjectileFireStrategy.cs`

- `Projectile` moves via `Rigidbody2D.linearVelocity` in direction
- Deals damage on `OnTriggerEnter2D` via `Health.TakeDamage`
- Auto-destroys after 5 seconds
- `ProjectileFireStrategy` instantiates from SO reference

**Status**: ✅ Complete

---

### 3.6 Modifier System

**Core Files**: `ModifierDefinition.cs`, `StatModifierDefinition.cs`, `DoubleShotModifierDefinition.cs`, `ModifierInstance.cs`, `ModifierInventory.cs`, `ModifierCalculator.cs`, `ModifierStat.cs`, `ModifierOperation.cs`

#### Calculation Pipeline
```
result = baseValue
→ Add all Add modifiers (across all providers)
→ Multiply all Multiply modifiers (across all providers)
```

#### Modifier Types
| Type | Base | Implementation |
|------|------|---------------|
| `StatModifierDefinition` | `ModifierDefinition` | Modifies a `ModifierStat` with `ModifierOperation` (Add/Multiply) |
| `DoubleShotModifierDefinition` | `WeaponFireModifierDefinition` | Mutates `WeaponFireContext`: `AttackCount++`, `SpreadAngle += 15` |

#### Stats Affected
| Stat | Enum | Example |
|------|------|---------|
| Damage | `ModifierStat.Damage` | DoubleDamage: ×2 |
| Fire Rate | `ModifierStat.FireRate` | DoubleFireRate: ×2 |
| Reload Duration | `ModifierStat.ReloadDuration` | DoubleReload: ×0.1 |
| Movement Speed | `ModifierStat.MovementSpeed` | DoubleSpeed: ×2 |

#### Modifier Inventory
- `AddModifier()` — permanent
- `AddTemporaryModifier()` — auto-expires via `Time.time` check in `Update()`
- `RemoveModifier()` — manual removal
- Events: `ModifierAdded`, `ModifierRemoved`

#### Modifier Pickups
| Prefab | Behavior |
|--------|----------|
| `ModifierPickup` | `IInteractable` → adds permanent modifier → destroys self |
| `TemporaryModifierPickup` | `IInteractable` → adds temporary modifier with duration → destroys self |

**Status**: ✅ Complete

---

### 3.7 Interaction System

**File**: `Interaction.cs`

- `CircleCollider2D` (trigger) for overlap detection
- Maintains list of `IInteractable` objects in range
- `Interact()` finds closest interactable by squared distance
- Used for weapon pickup and modifier pickup

**Status**: ✅ Complete

---

### 3.8 Weapon Pickup & Detection

**Files**: `WeaponPickup.cs`, `WeaponDetection.cs`

| Component | Role |
|-----------|------|
| `WeaponPickup` | `IInteractable` → calls `WeaponHolder.Equip()` on interactor |
| `WeaponDetection` | `CircleCollider2D` trigger → tracks nearby weapons in a list (used by enemy AI) |

**Status**: ✅ Complete

---

### 3.9 Player Input System

**Files**: `PlayerInput.cs`, `AimDevice.cs`, `PlayerInputActions.cs` (auto-generated)

#### Input Actions
| Action | Mouse/Keyboard | Gamepad |
|--------|---------------|---------|
| Move | WASD | Left Stick + Dpad |
| Look Pointer | Mouse Position | — |
| Look Stick | — | Right Stick |
| Fire | Left Click | Right Shoulder |
| Interact | E | South Button (A) |
| Reload | R | East Button (B) |

- Implements `IMoveInput` for `Movement`
- Events: `FirePressed`, `FireHeld`, `ReloadPressed`, `InteractPressed`
- `AimDevice` enum tracks Mouse vs Gamepad for aim source switching

**Status**: ✅ Complete

---

### 3.10 Enemy AI System

**Files**: `EnemyAIInput.cs`, `EnemyCombat.cs`, `EnemyWeaponDecision.cs`, `EnemyBehavior.cs`, `EnemyState.cs`

#### FSM States
```
Chasing ──────► SeekingWeapon (no weapon / weapon empty)
  ▲                    │
  │                    │ weapon found & in range
  │◄───────────────────┘
  │
  │ in attack range + has weapon
  ▼
Attacking ────► SeekingWeapon (weapon empty / dropped)
```

| State | Behavior |
|-------|----------|
| `Chasing` | Follows target, looks at target. Transitions to SeekingWeapon if no weapon or empty. Transitions to Attacking if in range. |
| `SeekingWeapon` | Finds closest weapon via `WeaponDetection`, moves toward it, picks up when in range. Falls back to Chasing if no weapons. |
| `Attacking` | Faces target, fires weapon. Reloads if ammo=0. Drops weapon if empty. Transitions to Chasing if out of range. |

| Component | Role |
|-----------|------|
| `EnemyAIInput` | Implements `IMoveInput` — calculates direction toward target |
| `EnemyCombat` | Range check (`Vector2.Distance`), calls `WeaponHolder.TryFire` |
| `EnemyWeaponDecision` | `GetClosestAvailableWeapon()` from `WeaponDetection`, `IsWeaponInPickupRange()`, `TryEquipNearbyWeapon()` |

**Status**: ✅ Complete

---

### 3.11 UI System

**File**: `WeaponAmmoUI.cs`

- Subscribes to `WeaponHolder.WeaponChanged` and `Weapon.AmmoChanged`
- Displays `CurrentAmmo / ReserveAmmo` or "∞ ∞" for infinite ammo
- Uses TextMeshPro `TextMeshProUGUI`

**Status**: ✅ Complete (minimal)

---

### 3.12 Debug System

**Files**: `HealthDebugger.cs`, `HealthTester.cs`, `WeaponDebugger.cs`, `ModifierDebugger.cs`

| Component | Role |
|-----------|------|
| `HealthDebugger` | Logs health changes, damage taken, death |
| `HealthTester` | `ContextMenu` actions: TakeDamage, Heal, RestoreFullHealth |
| `WeaponDebugger` | Logs fire, reload start/complete, ammo changes |
| `ModifierDebugger` | Logs modifier added/removed with stat/operation/value/duration |

**Status**: ✅ Complete

---

## 4. Input System

**Asset**: `PlayerInputActions.inputactions`

| Action | Type | Bindings |
|--------|------|----------|
| Move | Value (Vector2) | WASD, Gamepad dpad, Gamepad leftStick |
| LookPointer | Value (Vector2) | Mouse position |
| LookStick | Value (Vector2) | Gamepad rightStick |
| Fire | Button | Mouse leftButton, Gamepad rightShoulder |
| Interact | Button | Keyboard E, Gamepad buttonSouth |
| Reload | Button | Keyboard R, Gamepad buttonEast |

> `PlayerInputActions.cs` is auto-generated — do not edit manually.

---

## 5. Scenes

| Scene | In Build | Status | Contents |
|-------|----------|--------|----------|
| **SampleScene** | ✅ Yes (index 0) | Active | Main gameplay — Player, Enemy, weapons, pickups, canvas |
| **Bootstrap** | ❌ No | Empty | Placeholder |
| **Gameplay** | ❌ No | Empty | Placeholder |
| **MainMenu** | ❌ No | Empty | Placeholder |

---

## 6. Prefabs

| Prefab | Description |
|--------|-------------|
| **Player** | Player entity shell with all components (100 HP, speed 5) |
| **Enemy** | Enemy entity shell with AI, combat, weapon detection |
| **Projectile** | Bullet — speed 15, auto-destroy 5s, trigger collider |
| **ProjectileWeapon** | Generic weapon prefab with WeaponPickup, visual/audio feedback |

---

## 7. Packages

### Core
| Package | Version | Purpose |
|---------|---------|---------|
| `com.unity.render-pipelines.universal` | 17.3.0 | URP 2D rendering |
| `com.unity.inputsystem` | 1.19.0 | New Input System |
| `com.unity.test-framework` | 1.6.0 | Unit testing (imported, unused) |
| `com.unity.timeline` | 1.8.12 | Timeline (unused) |
| `com.unity.visualscripting` | 1.9.12 | Visual scripting (unused) |

### 2D
| Package | Version |
|---------|---------|
| `com.unity.2d.animation` | 13.0.5 |
| `com.unity.2d.tilemap` | 1.0.0 |
| `com.unity.2d.tilemap.extras` | 6.0.2 |
| `com.unity.2d.sprite` | 1.0.0 |
| `com.unity.2d.spriteshape` | 13.0.0 |
| `com.unity.2d.psdimporter` | 12.0.2 |
| `com.unity.2d.aseprite` | 3.0.2 |

---

## 8. Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| **Composition over inheritance** | Entities are thin shells; behavior is in `[RequireComponent]` MonoBehaviours — easier to test, mix, and swap |
| **ScriptableObjects for data** | Weapon stats, strategies, modifiers are SO assets — no code changes for new weapons/modifiers |
| **Strategy pattern for fire behavior** | `WeaponFireStrategy` (abstract SO) — melee vs projectile vs future types are interchangeable |
| **Add-then-Multiply pipeline** | Deterministic, order-independent for same operation type — standard game stat formula |
| **Events for decoupling** | All cross-component communication via `Action` — no direct references between feedback and logic |
| **Interface segregation** | `IMoveInput`, `IInteractable`, `IModifierProvider` — minimal contracts, multiple implementations |
| **Enemy AI as FSM** | Simple 3-state machine — easy to extend, easy to debug |
| **No `.asmdef` files** | Default `Assembly-CSharp` — simpler for a learning project of this scale |
| **New Input System only** | Legacy input disabled — clean, future-proof |

---

## 9. What Has Been Done — Status Matrix

| System | Status | Notes |
|--------|--------|-------|
| Health | ✅ Done | Core + Visual + Audio feedback |
| Movement | ✅ Done | Interface-driven, modifier-aware |
| Look Direction | ✅ Done | 2D rotation |
| Weapon Core | ✅ Done | Equip, Fire, Reload, Ammo, Cooldown |
| Weapon Strategies | ✅ Done | Melee + Projectile |
| Projectile | ✅ Done | Movement, Damage, Auto-destroy |
| Modifier System | ✅ Done | Add/Multiply pipeline, temporary, pickups |
| Interaction | ✅ Done | Trigger-based, closest selection |
| Weapon Pickup | ✅ Done | IInteractable integration |
| Weapon Detection | ✅ Done | Area-based weapon scanning |
| Player Input | ✅ Done | Mouse + Gamepad, IMoveInput |
| Enemy AI | ✅ Done | 3-state FSM, weapon seeking, combat |
| Weapon UI | ✅ Done | Ammo display (minimal) |
| Debug Tools | ✅ Done | Loggers + testers for all systems |

---

## 10. What's Missing — Identified Gaps

> These are the areas that need to be built or are absent from the project.

### 10.1 Core Gameplay Gaps

| Gap | Priority | Description |
|-----|----------|-------------|
| **Player Health Death** | 🔴 High | `Health.Died` event fires but nothing handles it — no game over, no respawn |
| **Enemy Death** | 🔴 High | Enemy has `Health` but no death handling — no destroy, no loot drop, no feedback |
| **Enemy Spawning** | 🔴 High | No spawn system — enemies are manually placed in scene |
| **Level / Room System** | 🔴 High | No procedural generation or level structure — roguelite core loop missing |
| **Game Over / Restart** | 🔴 High | No game state management — no pause, no restart, no run end |
| **Run Meta-progression** | 🟡 Medium | Roguelite staple — persistent upgrades across runs not implemented |
| **Health Pickup** | 🟡 Medium | No way to heal — `Health.Heal()` exists but nothing triggers it |
| **Score / Currency** | 🟡 Medium | No scoring or currency system |
| **Drop System** | 🟡 Medium | Enemies don't drop weapons or modifiers on death |

### 10.2 Scene & Flow Gaps

| Gap | Priority | Description |
|-----|----------|-------------|
| **Scene Flow** | 🔴 High | Bootstrap, MainMenu, Gameplay scenes are empty — no scene loading |
| **Main Menu** | 🔴 High | No main menu UI |
| **Game State Manager** | 🔴 High | No singleton/global state for run tracking |
| **Loading / Transitions** | 🟡 Medium | No scene transitions or loading screens |

### 10.3 UI Gaps

| Gap | Priority | Description |
|-----|----------|-------------|
| **Health Bar** | 🔴 High | No player health UI |
| **Weapon HUD** | 🟡 Medium | Only ammo shown — no weapon icon, no cooldown indicator |
| **Modifier Display** | 🟡 Medium | No active modifier HUD |
| **Pause Menu** | 🟡 Medium | No pause functionality |
| **Settings Menu** | 🟢 Low | No audio/graphics settings |
| **Damage Numbers** | 🟢 Low | No floating combat text |
| **Enemy Health Bar** | 🟢 Low | No enemy health display |

### 10.4 Audio Gaps

| Gap | Priority | Description |
|-----|----------|-------------|
| **Audio Manager** | 🟡 Medium | No global audio management — individual AudioSources only |
| **Music System** | 🟡 Medium | No background music |
| **Ambient Audio** | 🟢 Low | No environmental sounds |
| **SFX Pooling** | 🟢 Low | No object pooling for audio |

### 10.5 Visual Gaps

| Gap | Priority | Description |
|-----|----------|-------------|
| **Player Sprite** | 🟡 Medium | Cyan placeholder square |
| **Enemy Sprite** | 🟡 Medium | Red placeholder square |
| **Tilemap** | 🟡 Medium | No environment tiles — empty scenes |
| **Animations** | 🟡 Medium | No sprite animations (2D Animation package imported but unused) |
| **Particle Effects** | 🟢 Low | No VFX — muzzle flash is sprite swap only |
| **Screen Shake** | 🟢 Low | No camera effects |

### 10.6 Technical Gaps

| Gap | Priority | Description |
|-----|----------|-------------|
| **Object Pooling** | 🟡 Medium | `Instantiate`/`Destroy` for projectiles — will cause GC pressure |
| **Scene Management** | 🔴 High | No `SceneManager` usage — no scene loading flow |
| **Save System** | 🟢 Low | No persistence |
| **Localization** | 🟢 Low | No i18n |
| **Unit Tests** | 🟡 Medium | `test-framework` imported but zero test files |
| **CI/CD** | 🟢 Low | No automated build pipeline |

---

## 11. Roadmap — Work Plan

### Phase 1: Core Loop (🔴 Critical)

> Make the game playable — survive, fight, die, restart.

| # | Task | Description | Depends On |
|---|------|-------------|------------|
| 1.1 | **Enemy Death Handling** | Destroy enemy on `Health.Died`, add death feedback (flash/destroy) | — |
| 1.2 | **Player Death Handling** | Handle `Health.Died` on player — game over state, disable input | — |
| 1.3 | **Game State Manager** | Singleton managing Run/GameOver/Paused states | — |
| 1.4 | **Scene Flow** | Bootstrap → MainMenu → Gameplay scene loading | 1.3 |
| 1.5 | **Main Menu UI** | Start Game, Quit buttons | 1.4 |
| 1.6 | **Game Over Screen** | Show score/time, restart button | 1.2, 1.3 |
| 1.7 | **Health Bar UI** | Player health display | — |
| 1.8 | **Pause Menu** | ESC to pause, resume, quit to menu | 1.3 |

### Phase 2: Roguelite Loop (🟡 Important)

> Add the meta-game — drops, pickups, spawning, progression.

| # | Task | Description | Depends On |
|---|------|-------------|------------|
| 2.1 | **Enemy Spawner** | Wave/room-based enemy spawning | 1.3, 1.1 |
| 2.2 | **Weapon Drop on Death** | Enemies drop held weapon on death | 1.1 |
| 2.3 | **Modifier Drop System** | Enemies/pickups drop modifiers | 1.1 |
| 2.4 | **Health Pickup** | Consumable that heals player | — |
| 2.5 | **Score / Currency** | Track kills, currency for shop | 1.3 |
| 2.6 | **Run Timer** | Display run duration | 1.3 |
| 2.7 | **Modifier HUD** | Show active modifiers on screen | — |
| 2.8 | **Unit Tests** | Test Health, ModifierCalculator, Weapon fire logic | — |

### Phase 3: Polish & Content (🟢 Nice to Have)

> Make it feel good — visuals, audio, juice.

| # | Task | Description | Depends On |
|---|------|-------------|------------|
| 3.1 | **Placeholder Sprites** | Replace colored squares with temp art | — |
| 3.2 | **Sprite Animations** | Idle, walk, attack, death for player/enemy | 3.1 |
| 3.3 | **Tilemap Environment** | Basic room/arena with tiles | — |
| 3.4 | **Audio Manager** | Global audio with BGM, SFX layers | — |
| 3.5 | **Music System** | Background music per scene/state | 3.4 |
| 3.6 | **Screen Shake** | Camera juice on hit/fire | — |
| 3.7 | **Damage Numbers** | Floating combat text | — |
| 3.8 | **Projectile Pooling** | Object pool for projectiles | — |
| 3.9 | **Settings Menu** | Audio/graphics sliders | — |
| 3.10 | **Weapon HUD** | Full weapon display (icon, name, cooldown) | — |

### Phase 4: Roguelite Depth (Future)

> Advanced features for replayability.

| # | Task | Description | Depends On |
|---|------|-------------|------------|
| 4.1 | **Meta-progression** | Persistent upgrades between runs | 1.3 |
| 4.2 | **Shop System** | Spend currency on weapons/modifiers | 2.5 |
| 4.3 | **Room Generation** | Procedural room layouts | 3.3 |
| 4.4 | **Boss Enemies** | Special enemy types with unique mechanics | 1.1 |
| 4.5 | **More Weapon Types** | New fire strategies (beam, shotgun, etc.) | — |
| 4.6 | **More Modifiers** | New modifier definitions | — |
| 4.7 | **Challenge Runs** | Modifiers that increase difficulty | 4.1 |
| 4.8 | **Leaderboard** | High score persistence | 2.5 |

---

## 12. Asset Inventory

### ScriptableObject Assets

| Category | Assets |
|----------|--------|
| **Weapons** | Pistol, Colt, Mosin, Katana |
| **Fire Strategies** | Melee, Projectile |
| **Modifiers** | DoubleDamage, DoubleFireRate, DoubleReload, DoubleSpeed, DoubleShot |

### Audio

| Category | Count | Examples |
|----------|-------|---------|
| **Combat & Gore** | 15 | bone_snap, crunch, punch, slap, squelching |
| **Weapons** | 16 | sword_clash, shot_muffled, weapon_equip, weapon_upgrade |

### Sprites

| Asset | Notes |
|-------|-------|
| `MuzzleFlash.png` | Weapon VFX |

### Prefabs

| Prefab | Purpose |
|--------|---------|
| Player | Player entity |
| Enemy | Enemy entity |
| Projectile | Bullet entity |
| ProjectileWeapon | Weapon pickup (generic) |

---

## 13. Technical Notes

### ModifierCalculator — Add-then-Multiply Pipeline

```csharp
// Pseudocode
result = baseValue;

// Phase 1: All additions (across all providers)
foreach provider in providers:
    foreach modifier in provider.Modifiers:
        if modifier.Stat == targetStat AND modifier.Operation == Add:
            result += modifier.Value;

// Phase 2: All multiplications (across all providers)
foreach provider in providers:
    foreach modifier in provider.Modifiers:
        if modifier.Stat == targetStat AND modifier.Operation == Multiply:
            result *= modifier.Value;

return result;
```

This ensures consistent calculation order regardless of modifier application sequence.

### WeaponFireContext Pipeline

```
1. Weapon.TryFire() creates WeaponFireContext(this, direction)
2. ApplyModifiers() iterates weapon's ModifierInventory
3. ApplyModifiers() iterates holder's ModifierProvider
4. WeaponFireStrategy.Execute(context) uses modified context
5. Strategy handles spread angle, attack count, damage application
```

### Enemy AI State Transitions

```
Chasing
  ├─ no weapon → SeekingWeapon
  ├─ weapon empty → drop weapon → SeekingWeapon
  └─ in attack range → Attacking

SeekingWeapon
  ├─ no weapons available → Chasing (fallback)
  └─ weapon in range → pick up → Chasing

Attacking
  ├─ no weapon → SeekingWeapon
  ├─ weapon empty → drop weapon → SeekingWeapon
  ├─ ammo=0 → reload
  └─ out of attack range → Chasing
```

---

*This document is a living roadmap. Update it as development progresses.*
