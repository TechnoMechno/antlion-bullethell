# Ant Lion — Architecture

Snapshot of the project's structure as of 2026-09-21. Design rules and conventions live in [CLAUDE.md](CLAUDE.md); this file describes what actually exists. New to Unity? Read [LAYERS.md](LAYERS.md) first — it explains the four layers and how everything is wired together.

## 1. The four layers

Every script belongs to one layer. Dependencies only point downward or sideways: nothing depends on UI, and Core depends on nothing.

```
┌──────────────────────────────────────────────────────────────┐
│ PRESENTATION   HealthBar, MainMenuScreen, ResultScreen,      │  listens to events,
│                InstructionsPanel                             │  never drives logic
├──────────────────────────────────────────────────────────────┤
│ CONTROLLERS    PlayerController, BossController              │  thin: wire systems
│                                                              │  together, no rules
├──────────────────────────────────────────────────────────────┤
│ SYSTEMS        Health, PlayerMovement, PlayerShooter,        │  MonoBehaviours
│                BossAttack, ProjectilePool, Projectile,       │  that do the work
│                PowerUpPickup, PowerUpSpawner,                │
│                GameStateManager, SceneLoader, ArenaBounds    │
├──────────────────────────────────────────────────────────────┤
│ DATA           AttackPattern (+3 patterns), ProjectileMotion │  ScriptableObjects:
│                (+StraightMotion), PowerUpEffect,             │  tuning lives in assets,
│                PlayerStats, BossStats                        │  not code
└──────────────────────────────────────────────────────────────┘
```

**Namespace dependencies** (`→` means "uses"):
```
AntLion.UI            → AntLion.Core
AntLion.Player        → AntLion.Projectiles
AntLion.Boss          → AntLion.Core, AntLion.Projectiles, AntLion.Boss.Patterns
AntLion.Boss.Patterns → AntLion.Projectiles
AntLion.PowerUps      → (nothing; effects act on a GameObject)
AntLion.Projectiles   → (nothing)
AntLion.Core          → (nothing)
```

## 2. How a frame of gameplay will flow

Much of this is still stubs. Section 3 says what actually runs today.

**Player side**
```
Input System ("Move", later "Attack")
   → PlayerController.Update()
        → PlayerMovement.SetDirection()      → FixedUpdate sets Rigidbody2D velocity
        → PlayerShooter.Fire(direction)      → ProjectilePool.Get()
                                             → Projectile.Launch()  (layer = PlayerProjectile)
```

**Boss side**
```
BossController builds an AttackContext { FirePoint, Player, Pool }
   → BossAttack asks PatternSelector.Next(patterns)
        → AttackPattern.Execute(context)  (coroutine)
             → Pool.Get() → Projectile.Launch()  (layer = BossProjectile)
```

**Hits, health and game state**
```
Projectile hits something (the collision matrix decides what it can hit)
   → Health.TakeDamage()
        → OnChanged(current, max) → HealthBar fills
        → OnDeath                 → GameStateManager → Win / Lose
                                        → SceneLoader.Load("WinScreen" / "LoseScreen")
   → Projectile goes back to the pool (on hit, or when its lifetime ends)
```

**Power-ups**
```
PowerUpSpawner → places a PowerUpPickup holding a PowerUpEffect asset
   → the player touches it (PowerUp layer ↔ Player layer)
   → effect.Apply(player)
```

## 3. Scripts

✅ = working · 🟡 = API declared, body empty · ⬜ = empty shell

Methods that return a value and aren't written yet throw `NotImplementedException`, so an unfinished piece fails loudly instead of silently doing nothing.

### Core (`AntLion.Core`)
| Script | Status | Contents |
|---|---|---|
| `ArenaBounds` | ✅ | Builds a closed oval EdgeCollider2D from Width, Height and Segments. Rebuilds whenever those change in the Inspector. |
| `Health` | 🟡 | `maxHealth` (10), `MaxHealth`, `event OnChanged(int current, int max)`, `event OnDeath`, `TakeDamage(int)`, `Heal(int)` |
| `GameState` | ✅ | enum `Playing`, `Win`, `Lose` |
| `GameStateManager` | 🟡 | refs `playerHealth`, `bossHealth`, `sceneLoader`; `State` property; `event OnStateChanged` |
| `SceneLoader` | 🟡 | `Load(string sceneName)` |

### Player (`AntLion.Player`)
| Script | Status | Contents |
|---|---|---|
| `PlayerController` | ✅ movement / 🟡 shooting | Reads the project-wide `Move` action and calls `movement.SetDirection()`. Holds a `shooter` reference; the Attack hookup is a TODO until aiming is decided. |
| `PlayerMovement` | ✅ | `speed` (5), `SetDirection()` limits input to length 1 so diagonals aren't faster, and `FixedUpdate` sets the Rigidbody2D's velocity |
| `PlayerShooter` | 🟡 | refs `pool`, `firePoint`; `Fire(Vector2 direction)` |
| `PlayerStats` | ⬜ | ScriptableObject (Create > AntLion > Stats > Player Stats) |

### Boss (`AntLion.Boss`, `AntLion.Boss.Patterns`)
| Script | Status | Contents |
|---|---|---|
| `BossController` | ⬜ wiring only | refs `health`, `attack`, `firePoint`, `player`, `pool` |
| `BossAttack` | ⬜ | `List<AttackPattern> patterns`, `PatternSelector selector`. Keeping the list swappable leaves room for a phase 2 later. |
| `PatternSelector` | 🟡 | `Next(IReadOnlyList<AttackPattern>)`, currently throws `NotImplementedException` |
| `BossStats` | ⬜ | ScriptableObject |
| `AttackPattern` | ✅ (abstract) | `abstract IEnumerator Execute(AttackContext)` |
| `AttackContext` | ✅ | struct with `FirePoint`, `Player`, `Pool` |
| `RadialBurstPattern`, `AimedSpreadPattern`, `SpiralPattern` | 🟡 | `Execute` does nothing yet (`yield break`); each can be created from Create > AntLion > Attack Patterns |

### Projectiles (`AntLion.Projectiles`)
| Script | Status | Contents |
|---|---|---|
| `Projectile` | 🟡 | `speed` (10), `damage` (1), `lifetime` (3), `motion`; read-only properties for each; `Launch(Vector2 direction)` |
| `ProjectilePool` | 🟡 | `prefab`, `prewarmCount` (200); `Get()` throws; `Return(Projectile)` |
| `ProjectileMotion` | ✅ (abstract) | `abstract Vector2 GetVelocity(direction, speed, age)` |
| `StraightMotion` | ✅ | returns `direction * speed` |

### PowerUps (`AntLion.PowerUps`)
| Script | Status | Contents |
|---|---|---|
| `PowerUpEffect` | ✅ (abstract) | `abstract void Apply(GameObject target)` |
| `PowerUpPickup` | ⬜ | ref `effect` |
| `PowerUpSpawner` | ⬜ | `pickupPrefab`, `List<PowerUpEffect> effects` |

### UI (`AntLion.UI`)
| Script | Status | Contents |
|---|---|---|
| `HealthBar` | ⬜ | refs `health`, `fill` (Image). Works for any Health. |
| `MainMenuScreen` | 🟡 | refs `sceneLoader`, `instructions`; `Play()`, `ShowInstructions()`, `Quit()` |
| `InstructionsPanel` | 🟡 | `Show()`, `Hide()` |
| `ResultScreen` | 🟡 | ref `sceneLoader`; `Retry()`, `BackToMenu()` |

## 4. Prefabs

All root objects are at scale (1, 1, 1). Colliders sit on the root, and art sits on a child.

**Player** (`Prefabs/Player/Player.prefab`): physics layer `Player`, tag `Player`
```
Player   Rigidbody2D (Dynamic, Gravity 0, Freeze Rotation, Interpolate, Continuous)
         CircleCollider2D r 0.33 · PlayerController · PlayerMovement · PlayerShooter · Health
├── visual     SpriteRenderer, Circle 0.77×0.69, sorting Characters
├── Shadow     black 30% oval, sorting Ground
└── FirePoint  (0.45, 0), the front of the ant (art faces +X)
Wired inside the prefab: Controller → Movement, Shooter · Shooter → FirePoint
```

**Boss** (`Prefabs/Boss/Boss.prefab`): physics layer `Boss`
```
Boss     Rigidbody2D (Kinematic, Full Kinematic Contacts, Interpolate)
         CircleCollider2D r 1.1 · BossController · BossAttack · Health
├── Visual     2.5-unit reddish-brown circle, sorting Characters
├── Shadow     flattened black 30% oval, sorting Ground
└── FirePoint  at the boss's center
Wired inside the prefab: Controller → Health, Attack, FirePoint
```

**Projectile** (`Prefabs/Projectiles/Projectile.prefab`): physics layer `BossProjectile` by default. The shooter will change the layer when it fires.
```
Projectile  Rigidbody2D (Kinematic) · CircleCollider2D trigger r 0.09 · Projectile
└── Visual  0.25 magenta circle, sorting Projectiles (magenta is reserved for bullets)
```

**PowerUpPickup** (`Prefabs/PowerUps/PowerUpPickup.prefab`): physics layer `PowerUp`
```
PowerUpPickup  CircleCollider2D trigger r 0.3 · PowerUpPickup
└── Visual     0.5 green circle, sorting Characters
```

**HealthBar** (`Prefabs/UI/HealthBar.prefab`): a 400×28 UI bar
```
HealthBar   HealthBar (fill → Fill)
├── Background  black 60%
└── Fill        red Image, fills horizontally
```

## 5. Scenes

**Build order:** MainMenu (0), Arena (1), WinScreen (2), LoseScreen (3).

**Arena.unity**
```
Main Camera        CinemachineBrain (takes its position from CM_Follow)
CM_Follow          Cinemachine Camera → Player; Position Composer (damping 1);
                   Confiner2D (no shape assigned yet); Orthographic Size 4.11
Global Light 2D    lights all 5 sorting layers
Arena              (scale 1)
├── Floor          plain placeholder circle tinted sand, 23.53 × 12.16, centered at (1.04, 0.54),
│                  sorting Background
├── Bounds         EdgeCollider2D + ArenaBounds (48-point oval matching the Floor), layer ArenaBounds
└── CameraBounds   PolygonCollider2D trigger, layer CameraBounds (not used yet)
Player             at (1.04, -3.46), 4 units below the boss
Boss               at (1.04, 0.54), the arena center
Systems            SceneLoader · GameStateManager (→ both Healths, SceneLoader)
├── ProjectilePool (→ Projectile prefab)
└── PowerUpSpawner (→ PowerUpPickup prefab)
HUD                Screen-space canvas, 1920×1080 reference
├── BossHealthBar    top center, 800 wide, → Boss Health
└── PlayerHealthBar  bottom left, → Player Health
```

**References set only in the scene** (prefabs can't point at scene objects): `Player.PlayerShooter.pool`, `Boss.BossController.player` and `Boss.BossController.pool`.

**MainMenu.unity:** a `MainMenu` canvas with SceneLoader and MainMenuScreen, which references both. It has a hidden `InstructionsPanel` child (80% black overlay) and an EventSystem using the Input System UI module.

**WinScreen.unity / LoseScreen.unity:** a `ResultScreen` canvas with SceneLoader and ResultScreen, plus an EventSystem.

Each of those three also still has its default camera and light.

## 6. Project settings

**Sorting layers** (draw order, back to front):
`Default` → `Background` → `Ground` → `Characters` → `Projectiles`

**Physics layers:**
`BossProjectile` (6), `ArenaBounds` (7), `Player` (8), `PlayerProjectile` (9), `CameraBounds` (10), `Boss` (11), `PowerUp` (12)

**Collision matrix** (anything not listed doesn't collide):
| Layer | Collides with |
|---|---|
| Player | ArenaBounds, Boss, BossProjectile, PowerUp |
| Boss | Player, PlayerProjectile, ArenaBounds |
| PlayerProjectile | Boss |
| BossProjectile | Player |
| PowerUp | Player |
| CameraBounds | nothing |

**Everything else**
- **Rendering:** URP with the 2D Renderer. Lit sprites need a 2D light that targets their sorting layer.
- **Input:** Input System using the project-wide `Assets/Settings/InputSystem_Actions` (Move, Attack, …).
- **Packages:** Cinemachine (6.6.0, which uses the 3.x API), 2D Sprite, Tilemap, Animation, PSD Importer.
- **Serialization:** Force Text, as CLAUDE.md requires.
- **Gravity:** the global 2D gravity is still (0, −9.81). The player sets Gravity Scale 0; the boss and bullets are kinematic, so gravity doesn't affect them.

## 7. Data assets
| Asset | Location | Status |
|---|---|---|
| `Stats_Player`, `Stats_Boss` | `Data/Stats/` | exist, no fields yet |
| Attack pattern assets | `Data/AttackPatterns/` | none yet; they come in Step 4 |
| Power-up effect assets | `Data/PowerUps/` | none; Step 5 |
| Placeholder sprites | `Art/Sprites/Environment/` | `ArenaFloor_Placeholder` (plain), `ArenaFloorShaded_Placeholder` |

## 8. Built-in extension points

| Want to… | Do this | Code to change |
|---|---|---|
| Add a boss attack | subclass `AttackPattern`, create an asset, add it to `BossAttack.patterns` | one new class |
| Retune an attack | edit the pattern asset in the Inspector | none |
| Add phase 2 (later) | swap `BossAttack`'s pattern list when HP crosses a threshold | small |
| Add a bullet movement type | subclass `ProjectileMotion` | one new class |
| Add a power-up | subclass `PowerUpEffect`, create an asset, add it to the spawner's list | one class, or none if the type already exists |
| Show HP anywhere | drop in a `HealthBar` and point it at any `Health` | none |

## 9. What's next and what's still open

**By build step**
| Step | Status |
|---|---|
| 1. Movement, shooting, pooled projectile | Movement ✅. Pool, Projectile and Shooter are stubs. |
| 2. Health and HP bars | Components and bars are in place and wired; the logic is empty. |
| 3. Boss with one hardcoded pattern, then playtest | BossController and BossAttack are empty. |
| 4. Patterns as ScriptableObjects | The base class and 3 stubs exist. |
| 5. Power-ups | The base classes, pickup prefab and spawner exist. |
| 6. Screens and polish | Canvases and scripts are in place; buttons and text aren't built yet. |

**Decisions still open**
1. Aim at the mouse, or shoot straight ahead? Needed for Step 1.
2. Fire rate while the button is held. Needed for Step 1; agree it with the team, given CLAUDE.md's "no shot cooldown".
3. Contact damage from touching the boss. Decide at the Step 3 playtest.
4. Follow camera or fixed whole-arena camera. Decide at the Step 3 playtest.

**Loose ends**
- **Camera:** CM_Follow's damping of 1 feels floaty (try 0.2–0.3), and its Confiner2D has no shape assigned.
- **Floor art:** the Floor uses the plain placeholder. If you switch to the shaded one, set ArenaBounds to about 90% of the Floor size.
- **Menu scenes:** each still has its default Directional Light, which does nothing with the 2D Renderer. Harmless.
