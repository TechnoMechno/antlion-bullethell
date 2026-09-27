# The Four Layers — a beginner's guide

This explains how the Ant Lion codebase is organised, assuming you have barely touched Unity. Read it before writing your first script for this project.

[CLAUDE.md](CLAUDE.md) holds the rules, [ARCHITECTURE.md](ARCHITECTURE.md) lists what currently exists. This file explains *why* it is shaped this way and how the pieces connect.

---

## Part 0 — Unity in five minutes

Skip this if you already know Unity.

**GameObject.** A thing in the scene. On its own it is just a name and a position. A GameObject does nothing at all by itself.

**Component.** A part you attach to a GameObject to give it behaviour. A `SpriteRenderer` makes it visible. A `Rigidbody2D` makes physics move it. Our own scripts are components too. A GameObject is a pile of components.

**MonoBehaviour.** The base class that makes one of our scripts attachable to a GameObject. Unity calls certain methods on it automatically:

| Method | When Unity calls it | We use it for |
|---|---|---|
| `Awake()` | once, when the object is created | caching references, setting starting values |
| `Start()` | once, just before the first frame | anything that needs other objects to exist already |
| `Update()` | every rendered frame (varies with frame rate) | reading input |
| `FixedUpdate()` | every physics step (a fixed 50 times a second) | anything that moves a Rigidbody2D |
| `OnEnable()` / `OnDisable()` | when the object is switched on / off | subscribing and unsubscribing from events |
| `OnTriggerEnter2D(other)` | when a trigger collider is touched | bullet hits |

**Prefab.** A saved GameObject with all its components and settings, stored as a file. Drop it into a scene to get a copy ("instance"). Edit the prefab and every copy updates. Our prefabs: `Player`, `Boss`, `Projectile`, `PowerUpPickup`, `HealthBar`.

**Prefab variant.** A prefab based on another prefab. It inherits everything from its parent and overrides only what differs; change the parent and the variant follows, except where it overrides. Ours: `Projectile_Player` and `Projectile_Boss`, both variants of `Projectile`.

**Scene.** A saved arrangement of GameObjects. Ours: `MainMenu`, `Arena`, `WinScreen`, `LoseScreen`, plus `Sandbox_A` / `Sandbox_B` for testing.

**Inspector.** The panel showing a selected object's components and their settings.

**`[SerializeField] private float speed = 5f;`** — a private variable that still appears in the Inspector, so it can be tuned without touching code. We use this instead of `public` so other scripts cannot change it by accident. You will see it everywhere in this project.

**ScriptableObject.** A script that becomes an *asset file* in the project instead of living on a GameObject in a scene. Good for data: settings, configurations, definitions. More on this below.

**Event.** A way for a script to announce that something happened, without knowing who is listening. Central to how our layers talk to each other, explained in Part 6.

---

## Part 1 — The one rule

The project is split into four layers. Each is allowed to know about the layers **below** it, and never about the ones above.

```
┌──────────────────────────────────────────────────────────────────────────┐
│ 4. PRESENTATION   what the player sees                                   │
│    HP bars, menus, result screens                                        │
│    Watches. Never decides anything.                                      │
│    Folders: Scripts/UI/                                                  │
├──────────────────────────────────────────────────────────────────────────┤
│ 3. CONTROLLERS    the wiring                                             │
│    PlayerController, BossController                                      │
│    Reads input / makes decisions, then delegates.                        │
│    Folders: Scripts/Player/, Scripts/Boss/                               │
├──────────────────────────────────────────────────────────────────────────┤
│ 2. SYSTEMS        the parts that do the work                             │
│    Health, movement, shooting, pooling, game state                       │
│    Each does one job and announces what happened.                        │
│    Folders: Scripts/Core/, Scripts/Player/, Scripts/Boss/,               │
│             Scripts/Projectiles/, Scripts/PowerUps/                      │
├──────────────────────────────────────────────────────────────────────────┤
│ 1. DATA           numbers and definitions                                │
│    Attack patterns, power-up effects, stats                              │
│    Assets you edit in the Inspector. No behaviour.                       │
│    Folders: Scripts/Boss/Patterns/, Scripts/Projectiles/,                │
│             Scripts/PowerUps/, Scripts/Player/, Scripts/Boss/            │
│    Assets:  Data/AttackPatterns/, Data/PowerUps/, Data/Stats/            │
└──────────────────────────────────────────────────────────────────────────┘
        knowledge flows DOWN ▼      events flow UP ▲
```

### Folders are grouped by feature, not by layer

Some folders appear under more than one layer above, because the folders follow *features* (player, boss, projectiles) rather than layers. That is deliberate: the files you edit together in one sitting stay together, and each of you owns folders rather than slices of every folder.

| Folder | Layers it holds | Example files |
|---|---|---|
| `Scripts/Core/` | Systems | `Health`, `GameStateManager`, `SceneLoader`, `ArenaBounds` |
| `Scripts/Player/` | Controller + Systems + Data | `PlayerController` · `PlayerMovement`, `PlayerShooter` · `PlayerStats` |
| `Scripts/Boss/` | Controller + Systems + Data | `BossController` · `BossAttack`, `PatternSelector` · `BossStats` |
| `Scripts/Boss/Patterns/` | Data | `AttackPattern`, `RadialBurstPattern`, `AttackContext` |
| `Scripts/Projectiles/` | Systems + Data | `Projectile`, `ProjectilePool` · `ProjectileMotion`, `StraightMotion` |
| `Scripts/PowerUps/` | Systems + Data | `PowerUpPickup`, `PowerUpSpawner` · `PowerUpEffect` |
| `Scripts/UI/` | Presentation | `HealthBar`, `MainMenuScreen`, `InstructionsPanel`, `ResultScreen` |

To tell which layer a file belongs to, read the comment at the top of it, or check the tables in Parts 2 to 4. The `Data/` folder in the project holds the ScriptableObject *assets* created from the data scripts, not scripts.

**Why bother with layers at all?** Three reasons that matter on a small student project:

1. **You can change one piece without breaking others.** Swapping the keyboard for a gamepad touches one file.
2. **Two people can work at once** without editing the same file, which Unity merges badly.
3. **You can tune the game without programming.** Speeds, damage and attack patterns are all editable in the Inspector, which is what your Step 3 playtest needs.

---

## Part 2 — Layer 1: Data

**What it is:** ScriptableObjects. Assets that hold numbers and definitions, with no behaviour of their own.

**The idea.** A normal script lives on a GameObject inside a scene. A ScriptableObject lives in the Project window as a file, like a texture or a sound. It is a container for data you can create, name, duplicate and edit without writing code.

Why that matters here: your CLAUDE.md says *"Difficulty tuning must not require code changes."* If an attack pattern is an asset, a designer can duplicate `Pattern_RadialBurst`, rename it `Pattern_RadialBurst_Hard`, change "bullet count" from 12 to 20 in the Inspector, and drop it into the boss's list. No code, no recompile.

**The data assets in this project**

| Script | Becomes an asset like | Holds |
|---|---|---|
| `AttackPattern` (abstract) + `RadialBurstPattern`, `AimedSpreadPattern`, `SpiralPattern` | `Pattern_RadialBurst` | how one boss attack fires |
| `ProjectileMotion` (abstract) + `StraightMotion` | `Motion_Straight` | how a bullet moves |
| `PowerUpEffect` (abstract) | `PowerUp_Shield` | what a power-up does when picked up |
| `PlayerStats`, `BossStats` | `Stats_Player` | tunable numbers for an entity |

**What an abstract data class looks like.** `AttackPattern` defines what every attack must be able to do, without saying how:

```csharp
public abstract class AttackPattern : ScriptableObject
{
    public abstract IEnumerator Execute(AttackContext context);
}
```

Each concrete pattern is a subclass, and the `[CreateAssetMenu]` attribute puts it in the right-click **Create** menu in the Project window:

```csharp
[CreateAssetMenu(menuName = "AntLion/Attack Patterns/Radial Burst", fileName = "Pattern_RadialBurst")]
public class RadialBurstPattern : AttackPattern { ... }
```

**Rules for this layer**

- **No references to scene objects.** An asset exists outside any scene, so it cannot point at "the boss in the Arena". If a pattern needs the boss's fire point, it is *passed in* when it runs (see `AttackContext` in Part 5).
- **Prefer no changing state.** One asset is shared by everyone using it, and in the editor, changes to it persist after you stop playing. Treat data assets as read-only at runtime.
- **Naming:** `Pattern_RadialBurst`, `PowerUp_Shield`, `Stats_Player`. Prefix by kind, so the Project window sorts sensibly.

---

## Part 3 — Layer 2: Systems

**What it is:** MonoBehaviours that do the actual work. Most of the code lives here.

| System | File | Job |
|---|---|---|
| `Health` | `Core/Health.cs` | HP, damage, death, invulnerability |
| `PlayerMovement` | `Player/PlayerMovement.cs` | moves the ant's Rigidbody2D |
| `PlayerShooter` | `Player/PlayerShooter.cs` | fires bullets from the fire point |
| `BossAttack` | `Boss/BossAttack.cs` | runs attack patterns |
| `ProjectilePool` | `Projectiles/ProjectilePool.cs` | hands out and recycles bullets |
| `Projectile` | `Projectiles/Projectile.cs` | one bullet: moves, hits, expires |
| `PowerUpPickup`, `PowerUpSpawner` | `PowerUps/` | places and applies power-ups |
| `ArenaBounds` | `Core/ArenaBounds.cs` | builds the oval wall around the pit |
| `GameStateManager`, `SceneLoader` | `Core/` | Playing / Win / Lose, scene changes |

**A system exposes a small API and keeps its own details private.** `PlayerMovement` is the clearest example:

```csharp
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;   // tunable in the Inspector

    private Rigidbody2D body;
    private Vector2 direction;

    private void Awake() => body = GetComponent<Rigidbody2D>();

    public void SetDirection(Vector2 newDirection)          // ← the whole public API
        => direction = Vector2.ClampMagnitude(newDirection, 1f);

    private void FixedUpdate() => body.linearVelocity = direction * speed;
}
```

Notice what is *not* there: no keyboard, no mention of the player, no knowledge of HP or bullets. It moves whatever it is attached to, in whatever direction it was last told. That is why a knockback effect or a cutscene could drive it later without changes.

**`Health` is the layer's other showcase.** One component used by *both* the player and the boss:

```csharp
public void TakeDamage(int amount)
{
    if (amount <= 0 || IsDead || IsInvulnerable) return;

    Current = Mathf.Max(0, Current - amount);
    OnChanged?.Invoke(Current, maxHealth);      // announce: "my HP changed"

    if (Current == 0) { IsDead = true; OnDeath?.Invoke(); }   // announce: "I died"
}
```

It never asks *who* it belongs to, and it never tells anyone what to do about it. It changes a number and announces the change. That single decision is why the same HP bar script draws both bars, and why `GameStateManager` can decide win versus lose just by listening to two different `Health` components.

**Rules for this layer**

- **One job per system.** If you cannot describe a script in one sentence, it is probably two scripts.
- **Never read input here.** Input belongs in a controller. A system is told what to do.
- **Never search the scene** with `GameObject.Find` or `FindObjectOfType`. Take what you need as a `[SerializeField]` reference or a method argument. Searching is slow, silently breaks when things are renamed, and hides the dependency.
- **Announce, do not command.** Raise an event and let interested parties react.

---

## Part 4 — Layer 3: Controllers

**What it is:** two thin scripts, `PlayerController` and `BossController`, that decide *when* things happen and pass the request to the systems.

```csharp
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerShooter shooter;

    private InputAction moveAction;

    private void Start() => moveAction = InputSystem.actions.FindAction("Move");

    private void Update() => movement.SetDirection(moveAction.ReadValue<Vector2>());
}
```

That is the entire controller: read input, hand it over. All of the movement rules, such as speed, diagonal handling and physics, live in `PlayerMovement`.

**What a controller may do**

- Read input (the player's controller) or make decisions (the boss's controller).
- Hold references to its systems and call their methods.
- Assemble information one system needs about another, such as building the `AttackContext` for the boss.

**What a controller must not do**

- Contain gameplay rules. No speed values, no damage numbers, no HP checks.
- Move a transform or a Rigidbody directly.
- Touch the UI.

**Why so thin?** Controllers are where beginners accumulate a 600-line "player script" that does everything and cannot be edited by two people at once. Keeping them to wiring is what makes the systems below reusable and testable.

---

## Part 5 — How things get wired together

Three different mechanisms, used for three different situations.

### a) Inspector references, inside a prefab

When two components live on the same prefab, drag one into the other's slot in the Inspector. Unity saves the connection in the prefab file.

Already wired in `Player.prefab`:
```
PlayerController.movement  →  PlayerMovement  (same object)
PlayerController.shooter   →  PlayerShooter   (same object)
PlayerShooter.firePoint    →  FirePoint       (child object)
```
Already wired in `Boss.prefab`:
```
BossController.health     →  Health
BossController.attack     →  BossAttack
BossController.firePoint  →  FirePoint
```

### b) Inspector references, inside a scene

**A prefab cannot reference something that only exists in a scene.** The prefab is a file on disk; the scene object is not part of it. So anything pointing from a prefab instance to a scene object must be set on the *instance*, in the scene.

Wired in `Arena.unity` (and in both sandbox scenes):
```
Player (instance) → PlayerShooter.pool      →  Systems/PlayerProjectilePool  (Projectile_Player)
Boss   (instance) → BossController.player   →  Player
Boss   (instance) → BossController.pool     →  Systems/BossProjectilePool    (Projectile_Boss)
HUD/…HealthBar.health                       →  the Player's / Boss's Health
GameStateManager.playerHealth / .bossHealth →  the two Health components
```

If you ever see "the field is empty in the prefab but filled in the scene", this is why, and it is correct.

### c) Passing references in, for data assets

Data assets cannot hold scene references at all (Part 2). So when a boss pattern runs, everything it needs is handed to it as an argument:

```csharp
public struct AttackContext
{
    public Transform FirePoint;   // where bullets come from
    public Transform Player;      // what to aim at
    public ProjectilePool Pool;   // where to get bullets
}
```

`BossController` builds this, `BossAttack` passes it to `pattern.Execute(context)`. The pattern asset stays a pure description of an attack and can be reused by any boss, in any scene, with no lookups.

---

## Part 6 — Events: how information flows back up

A system must not reach up into the UI. So how does the HP bar know to redraw?

The system **announces**; interested parties **listen**. In C# that is an `event`:

```csharp
// In Health (a system)
public event Action<int, int> OnChanged;   // current, max
public event Action OnDeath;
```

`Health` raises these. It has no idea who, if anyone, is listening.

```csharp
// In HealthBar (presentation)
private void OnEnable()  => health.OnChanged += Redraw;
private void OnDisable() => health.OnChanged -= Redraw;   // always unsubscribe

private void Redraw(int current, int max) => fill.fillAmount = (float)current / max;
```

**Always unsubscribe in `OnDisable`.** A listener that is destroyed while still subscribed causes errors later, and it is the single most common beginner bug with events.

Who listens to what in this project:

```
Health.OnChanged  →  HealthBar (player's bar and boss's bar)
Health.OnDeath    →  GameStateManager  →  decides Win or Lose  →  SceneLoader
```

Because the flow is one-way, deleting the HP bar cannot break combat, and changing the win screen cannot break the boss.

---

## Part 7 — Three complete walkthroughs

### Pressing W
```
Keyboard
  → PlayerController.Update()            reads the "Move" action          [controller]
  → PlayerMovement.SetDirection(0,1)     stores the direction             [system]
  → PlayerMovement.FixedUpdate()         sets Rigidbody2D velocity        [system]
  → Unity physics moves the ant; the ArenaBounds collider stops it at the rim
```

### Firing a bullet
```
Mouse held
  → PlayerController                     reads the "Attack" action        [controller]
  → PlayerShooter.Fire(direction)                                         [system]
  → ProjectilePool.Get()                 takes a recycled, hidden         [system]
                                         Projectile_Player from the player's pool
  → Projectile.Launch(position, direction)                                [system]
  → Projectile.FixedUpdate()             asks its ProjectileMotion asset  [data]
                                         how fast to move, then moves
  → after `lifetime` seconds it returns itself to the pool
```

Bullets are **recycled, never created and destroyed**, because the boss's patterns get dense and allocating mid-fight causes stutter. The pool creates 200 at load.

### A bullet hits the player
```
Projectile.OnTriggerEnter2D(other)                                        [system]
  → other has a Health? → Health.TakeDamage(1)                            [system]
  → Health lowers HP, raises OnChanged                                    [event ▲]
        → HealthBar redraws                                               [presentation]
  → if HP hit 0, Health raises OnDeath                                     [event ▲]
        → GameStateManager sets Win or Lose                                [system]
        → SceneLoader loads WinScreen / LoseScreen                         [system]
  → the bullet returns itself to the pool
```

Note what the bullet does *not* do: it does not check whether it hit the player or the boss. The physics layers do that. Player bullets are `Projectile_Player`, a variant set to the `PlayerProjectile` layer, which the collision matrix only lets touch `Boss`. Boss bullets are `Projectile_Boss` on `BossProjectile`. Each side has its own pool, so a bullet's layer is fixed by which pool it came from and no code ever sets it. One script, two variants, no branching code.

---

## Part 8 — Recipes

**Add a new boss attack**
1. Create `Scripts/Boss/Patterns/MyPattern.cs`, subclassing `AttackPattern`.
2. Implement `Execute(AttackContext)`: get bullets from `context.Pool` (the boss's pool, already on the right layer), launch them from `context.FirePoint`, `yield return new WaitForSeconds(...)` between shots.
3. Add `[CreateAssetMenu(menuName = "AntLion/Attack Patterns/My Pattern", fileName = "Pattern_MyPattern")]`.
4. In the Project window: **Create > AntLion > Attack Patterns > My Pattern**, save it in `Data/AttackPatterns/`.
5. Drag the asset into the Boss's `BossAttack.patterns` list.

**Add a power-up**
1. Subclass `PowerUpEffect`, implement `Apply(GameObject target)`.
2. Create the asset in `Data/PowerUps/`, named `PowerUp_Something`.
3. Add it to the `PowerUpSpawner`'s list. The pickup prefab needs no changes.

**Show a new number on screen (e.g. a dash cooldown)**
1. Add an event to the system that owns the number.
2. Write a small UI script that subscribes in `OnEnable` and unsubscribes in `OnDisable`.
3. Never let the UI script compute the number itself.

**Add a new player ability (e.g. dash)**
1. New system: `Scripts/Player/PlayerDash.cs`, with a public method such as `TryDash(Vector2 direction)`.
2. It moves the Rigidbody2D and calls `health.SetInvulnerable(true/false)` around the dash.
3. `PlayerController` reads the input and calls `TryDash`. The controller holds no dash rules: no distance, no duration, no cooldown.

---

## Part 9 — Mistakes this project has already hit

- **Sorting layer vs physics layer.** Two unrelated settings with the same names. Sorting Layer (on the SpriteRenderer) is draw order; Layer (top of the Inspector) is collisions. New sprites default to the `Default` sorting layer, which draws *behind* the floor, so they appear to vanish.
- **A 2D light that does not target your sorting layer** makes sprites render pure black. The Global Light 2D must target every sorting layer.
- **Art offset from its parent.** Keep the art child at local position (0, 0, 0). Physics, cameras and code all use the *root's* position, so an offset sprite puts the visible ant somewhere the game does not think it is.
- **Moving with `transform.position +=`** skips physics: the ant walks through walls. Move a Rigidbody2D by setting `linearVelocity` in `FixedUpdate`.
- **Rigidbody2D without Interpolate** looks jittery, because physics runs 50 times a second while the screen draws 60 or more.
- **`Health` must sit on the same GameObject as the collider**, because `Projectile` looks it up from whatever it touched.

---

## Part 10 — Glossary

| Term | Meaning |
|---|---|
| **Component** | a part attached to a GameObject; our scripts are components |
| **Prefab** | a saved GameObject, reusable, edited in one place |
| **Prefab instance** | a copy of a prefab placed in a scene |
| **ScriptableObject** | a script saved as an asset file; used for data |
| **Event** | an announcement other scripts can subscribe to |
| **Coroutine** | a method that can pause and resume, used for attack timing |
| **Object pool** | a set of pre-made objects reused instead of created and destroyed |
| **Collider / trigger** | a shape used for collisions; a trigger detects overlap without blocking |
| **Rigidbody2D** | the component that lets physics move an object |
| **Sorting layer** | decides draw order for sprites |
| **Physics layer** | decides what collides with what, via the collision matrix |
| **Serialized field** | a private variable exposed in the Inspector with `[SerializeField]` |
