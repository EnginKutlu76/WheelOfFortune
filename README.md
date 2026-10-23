# Wheel of Fortune

A mobile wheel-of-fortune game demo built with **Unity 6000.3.23f1** for the Vertigo Games Game Developer case. Spin the wheel, collect rewards, avoid the bomb, and decide when to cash out.

[![Gameplay video](https://img.youtube.com/vi/VIDEO_ID/maxresdefault.jpg)](https://www.youtube.com/watch?v=VIDEO_ID)

**[▶ Watch the gameplay video]([https://www.youtube.com/watch?v=VIDEO_ID](https://www.youtube.com/watch?v=Yl71GhKHQrE))** · **[⬇ Download the APK](https://github.com/EnginKutlu76/WheelOfFortune/releases/latest)**

## Screenshots

| 20:9 | 16:9 | 4:3 |
|---|---|---|
| <img src="Docs/Screenshots/screenshot_20x9.png" width="300"> | <img src="Docs/Screenshots/screenshot_16x9.png" width="300"> | <img src="Docs/Screenshots/screenshot_4x3.png" width="300"> |

## How it works

- Every zone has a wheel with **8 slices**. The player spins instead of picking a card.
- **Normal zones** contain exactly one bomb. Rewards get better as the zone number grows.
- **Safe zone** (every 5th): silver wheel, no bomb, risk-free spin.
- **Super zone** (every 30th): golden wheel, special rewards, no bomb, risk-free spin.
- **Collect** (cash out) is available only on safe and super zones, and only while the wheel is idle.
- **Bomb:** the player can revive once to keep the rewards. Otherwise everything is lost and the run restarts.
- The result is decided before the spin animation starts, so the wheel always lands on the pre-selected slice.

## Features

- Slice contents, reward pools, multipliers and bomb counts are editable in the Editor through ScriptableObjects.
- Responsive UI for **20:9, 16:9 and 4:3** (Canvas Scaler *Expand*, anchor/pivot-based layout, TextMeshPro).
- DOTween animations, Sprite Atlases, prefab-based UI, object pooling for the reward inventory.
- Event-driven UI (no `Update` polling), no Inspector `OnClick` references (buttons are bound in code, references are auto-assigned in `OnValidate`).
- Editor tooling: **Tools > Vertigo > UI Audit** checks Raycast Target, Maskable, Sliced images, naming and Canvas Scaler rules.
- 34 automated EditMode tests.

## Architecture

| Layer | Contents |
|---|---|
| **Domain** (pure C#, assembly `VertigoGames.Domain`) | `RunState` (game rules), `ZoneRules`, `WheelSelector`, `WheelMath`, `IRandom`, ScriptableObject data (`ZoneConfigSet`, `ZoneConfig`, `RewardPoolData`, `RewardData`) |
| **Game** | `GameController` (composition root), `WheelSpinController`, `WheelSliceVisualController`, view classes (`HudView`, `CashOutView`, `ResultView`, `RewardInventoryView`, `ZoneProgressView`), `AudioManager` |

Design decisions:

- **Game rules live in `RunState`**, a plain C# class with no Unity dependency, so they can be unit tested.
- **UI reacts to `RunState.Changed`** instead of polling every frame, which is cheaper on mobile.
- **Randomness goes through `IRandom`**, so spin selection is deterministic in tests.
- **Data-driven config:** `ZoneConfigSet` → `ZoneConfig` → `RewardPoolData` → `RewardData`.
- `GameController` is the only place that wires views, wheel, audio and state together.

## Project structure

```
Assets/_Game/
  Art/            sprites and Sprite Atlases
  Audio/          SFX
  Editor/         UI audit tool
  Prefabs/        UI prefabs
  ScriptableObjects/  reward pools, zone configs
  Scenes/Game.unity
  Scripts/
    Domain/       pure game logic and data (own assembly)
    Game/         controllers, views, audio
  Tests/          EditMode tests (own assembly)
```

## Getting started

1. Open the project with **Unity 6000.3.23f1**.
2. Open `Assets/_Game/Scenes/Game.unity` and press Play.
3. Run tests: *Window > General > Test Runner > EditMode > Run All*.

## Build

Android, IL2CPP, ARM64. The signed release APK is available on the [Releases page](https://github.com/EnginKutlu76/WheelOfFortune/releases/latest).
