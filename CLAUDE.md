# Ant Lion — Unity Project

A 2D top-down boss fight built for IAT 312. The player is an ant in an arena with an antlion boss: dodge the boss's projectiles, shoot back, bring its HP to zero. Player HP reaching zero is a loss.

This is a redesign of an existing Flash game. The design goals are **Challenge, Sensation, and Fantasy**, aimed at casual-to-hardcore players aged 13–25. When a design tradeoff comes up, favour readable, learnable boss patterns over raw difficulty.

**Unity version:** 6000.6.0f1 — must match across the team
**Team size:** 2

## Repo layout

```
Assets/
  _Project/          our work — everything we author goes here
    Art/
      Sprites/       Player/ Boss/ Projectiles/ PowerUps/ Environment/ UI/
      Animations/
      Materials/
    Audio/           SFX/ Music/
    Data/            ScriptableObject assets: AttackPatterns/ PowerUps/ Stats/
    Prefabs/         Player/ Boss/ Projectiles/ PowerUps/ UI/
    Scenes/
    Scripts/         Core/ Player/ Boss/ Boss/Patterns/ Projectiles/ PowerUps/ UI/
    Settings/
  Plugins/           third-party — do not edit
```

Anything imported (asset store, packages) stays outside `_Project/`. New scripts go in the matching `Scripts/` subfolder — do not create new top-level folders without asking.

## Architecture

Four layers. Keep them separate.

1. **Data** — ScriptableObjects. Attack patterns, power-up effects, stat configs.
2. **Systems** — MonoBehaviours. Health, movement, shooting, pooling, spawning.
3. **Controllers** — `PlayerController`, `BossController`. Thin; they wire systems together and hold no gameplay rules of their own.
4. **Presentation** — HP bars, VFX, audio, screens. Subscribes to events. Never drives logic.

## Decisions that are settled

- **`Health` is one shared component.** Player and boss both use it. It exposes `OnChanged(current, max)` and `OnDeath`. Nothing that listens should need to know which entity it belongs to. It must sit on the same GameObject as the entity's collider, because `Projectile` looks it up from whatever it hits. Damage is ignored while `IsInvulnerable` is set; the caller (dash, shield) owns the timing.
- **Attack patterns are ScriptableObjects** subclassing `AttackPattern`, one class per pattern, tunable fields exposed in the Inspector. The boss holds a list of them plus a selector. Difficulty tuning must not require code changes.
- **Projectiles are pooled.** `ProjectilePool` with `Get()`/`Return()`. Never `Instantiate`/`Destroy` a bullet at runtime — the patterns get dense enough that it will stutter.
- **One `Projectile` prefab**, carrying speed, damage, and a movement strategy. Player and boss bullets are two prefab variants of it, `Projectile_Player` and `Projectile_Boss`, each with its own `ProjectilePool`. Each variant carries its own layer and tuning; the collision matrix decides what can hit what. Do not write separate player/boss bullet classes.
- **Power-ups are `PowerUpEffect` ScriptableObjects** applied by a single `PowerUpPickup` prefab. Adding a power-up means authoring an asset, not editing a spawner.
- **`GameStateManager`** tracks Playing / Win / Lose, listens to both `OnDeath` events, and drives transitions. Keep it small.

## Not yet built — leave the seam, not the feature

- **Boss phase 2.** Planned for after the prototype. The pattern list must stay swappable so a phase swap at an HP threshold is a small change. Do not build the phase system now.
- **Player constraints** (ammo limits, shot cooldown). Deliberately absent from the first playable. Playtesting decides whether unlimited fire kills the tension. Do not add one unprompted.
- **Character select / unlocks.** Backlog. Out of scope for the first playable.

## Layers

Two unrelated systems — don't mix them up.

- **Sorting Layers** (SpriteRenderer → Sorting Layer) decide draw order, back to front: `Default`, `Background`, `Ground`, `Characters`, `Projectiles`. New sprites start on `Default`, which is *behind* the floor — always set one. Floor → `Background`, shadows → `Ground`, player/boss/pickups → `Characters`, bullets → `Projectiles`.
- **Physics Layers** (the Layer dropdown at the top of the Inspector) decide collisions: `Player`, `Boss`, `PlayerProjectile`, `BossProjectile`, `ArenaBounds`, `CameraBounds`, `PowerUp`. Only objects with a collider need one; sprite-only children stay on `Default`.
- Collision matrix: Player ↔ ArenaBounds, Boss, BossProjectile, PowerUp. Boss ↔ PlayerProjectile, ArenaBounds. Everything else is off. A projectile's layer is set on its prefab variant, never in code.
- The Global Light 2D must target every sorting layer. A sorting layer it doesn't target renders black.

## Conventions

- Classes and files: `PascalCase`, file name matches the class name.
- Private serialized fields: `[SerializeField] private int maxHealth;`
- Events: `OnSomethingHappened`.
- ScriptableObject assets: `Pattern_RadialBurst`, `PowerUp_Shield`.
- Prefabs: `Player`, `Boss`, `Projectile`, `PowerUpPickup` — no `_Prefab` suffix.
- Namespace scripts under `AntLion.<Area>` (e.g. `AntLion.Boss`).
- No game logic in UI scripts, ever.

## Working with this repo

- **Never move, rename, or delete files outside the Unity editor.** It orphans `.meta` files and breaks every reference to them.
- **Version control is Unity Version Control (Plastic)**, repo `AntLion BulletHell`, branch `/main`. `ignore.conf` excludes `Library/`, `Temp/`, `Logs/`, `UserSettings/` and `obj/`. Check in small and often, and pull before you start.
- `.meta` files and `ProjectSettings/` are checked in. A missing `.meta` breaks every reference to that file for the other person.
- Asset Serialization is **Force Text** and Version Control Mode is **Visible Meta Files** (Project Settings > Editor). Do not change these — text scenes are the only reason merges are survivable.
- **Scene merges are the main collision risk.** One person owns `Arena.unity`; the other works in prefabs. Prefer a prefab change over a scene change whenever both would work.
- **Test in your own sandbox scene** (`Scenes/Sandbox_A.unity` / `Sandbox_B.unity`), not in `Arena.unity`. They are deliberately out of Build Settings.
- **One person at a time edits Project Settings** (layers, collision matrix, sorting layers). `TagManager.asset` and `Physics2DSettings.asset` do not merge.
- Do not reorganize folders mid-project. Mass file moves wreck review and merges for the other person.

## Build order

The prototype is being built in this sequence. Prefer work that advances the current step over work that jumps ahead.

1. Player movement + shooting + a pooled projectile.
2. `Health` on both sides, HP bars wired to it.
3. Boss with one hardcoded pattern. **Stop and playtest here.**
4. Convert that pattern to a ScriptableObject, add two more.
5. Power-ups.
6. Win/lose screens, instructions, polish.

Step 3 is a real gate. If dodging and shooting isn't fun with a single pattern, the fix is the core loop, not more content.
