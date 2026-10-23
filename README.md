# Wheel of Fortune

A mobile wheel-spin game demo developed with **Unity 6000.3.23f1** for the Vertigo Games Game Developer case.

Spin the wheel, collect rewards, manage risk, and decide when to cash out. The game features regular, safe (silver), and super (gold) zones with configurable rewards and gameplay rules.

**[▶ Watch Gameplay Video](https://www.youtube.com/watch?v=Yl71GhKHQrE)** · **[⬇ Download Android APK](https://github.com/EnginKutlu76/WheelOfFortune/blob/main/Builds/Android/WheelOfFortune.apk)**

## Screenshots

| 20:9 | 16:9 | 4:3 |
|---|---|---|
| ![Gameplay 20:9](Docs/screenshot_20x9.png) | ![Gameplay 16:9](Docs/screenshot_16x9.png) | ![Gameplay 4:3](Docs/screenshot_4x3.png) |

## Gameplay

- Spin a wheel containing 8 slices to win rewards or encounter a bomb.
- Regular zones contain a bomb, putting accumulated rewards at risk.
- Every 5th zone is a **Safe Zone**, featuring a silver wheel without bombs.
- Every 30th zone is a **Super Zone**, featuring a gold wheel, special rewards, and no bombs.
- The Collect option becomes available in eligible zones when the wheel is idle.
- Players can revive once after hitting a bomb to preserve accumulated rewards. If a bomb is hit again after the revive has been used, accumulated rewards are lost.
- The spin result is selected before the animation starts, ensuring the wheel animation matches the selected outcome.

## Features

- **Data-driven gameplay:** Reward pools, zone configurations, slice weights, and reward values are configurable through ScriptableObjects.
- **Responsive UI:** TextMeshPro and anchor-based layouts designed for different aspect ratios.
- **Wheel animation:** DOTween-based rotation and animation sequences.
- **Sprite Atlases:** Organized sprite assets for UI rendering.
- **UI architecture:** Separate views for the HUD, cash-out, results, reward inventory, and zone progression.
- **Editor tooling:** UI Audit tool for checking naming conventions and UI configuration.
- **Automated tests:** EditMode tests covering game state, zone rules, reward configuration, and wheel selection.

## Architecture

The project separates gameplay state and rules from Unity-facing presentation and controllers.

| Layer | Responsibility |
|---|---|
| **Domain** | Game state, reward stacking, zone rules, wheel selection, and reward data |
| **Game** | Game controller, spin orchestration, audio, and UI view binding |
| **Presentation** | Reward animations and wheel slice visuals |
| **Editor** | UI audit and validation tools |
| **Tests** | EditMode tests for core gameplay behavior |

### Design Principles

- Gameplay state is managed by `RunState`, independently of `MonoBehaviour`.
- Zone behavior is centralized in `ZoneRules`.
- Wheel selection uses an `IRandom` abstraction to support deterministic testing.
- Reward and zone configuration is data-driven through ScriptableObjects.
- `GameController` connects gameplay systems and UI views.
- UI views respond to state changes instead of continuously polling the game state.

## Project Structure

```text
Assets/
  _Game/
    Scenes/
      Game.unity
    Scripts/
      Domain/
        Common/
        Core/
        Data/
      Game/
        App/
        Audio/
        Gameplay/
        UI/
      Rewards/
      Wheel/
    Tests/
Builds/
  Android/
    WheelOfFortune.apk
Docs/
  screenshot_20x9.png
  screenshot_16x9.png
  screenshot_4x3.png
Packages/
ProjectSettings/
```

## Getting Started

### Requirements

- Unity **6000.3.23f1**
- Android Build Support, including Android SDK & NDK Tools and OpenJDK, to create Android builds

### Run in Unity

1. Clone or download this repository.
2. Open the project using Unity Hub with Unity `6000.3.23f1`.
3. Open `Assets/_Game/Scenes/Game.unity`.
4. Press **Play** in the Unity Editor.

### Run Automated Tests

1. Open **Window → General → Test Runner**.
2. Select **EditMode**.
3. Click **Run All**.

## Android Build

A tested Android APK is included in the repository.

**[Download WheelOfFortune.apk](https://github.com/EnginKutlu76/WheelOfFortune/blob/main/Builds/Android/WheelOfFortune.apk)**

To create a new build:

1. Open **File → Build Profiles**.
2. Select Android and switch to the Android platform if necessary.
3. Ensure the required scenes are included in the build profile.
4. Disable **Build App Bundle** to generate an APK.
5. Click **Build** and choose an output location.

## Project Information

- **Project:** Wheel of Fortune
- **Engine:** Unity 6000.3.23f1
- **Platform:** Android
- **Purpose:** Vertigo Games Game Developer demo case
- **Status:** Functional demo, tested on an Android device

