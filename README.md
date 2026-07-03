# Simple Magic Tiles 3

A minimal Magic Tiles 3 clone built with Unity — endless rhythm game with tap-based gameplay, event-driven architecture, and increasing difficulty.

> **Project Context:** This is the implementation for the Amanotes Senior Game Unity Developer assessment.  
> **Original Game:** [Magic Tiles 3 on Play Store](https://play.google.com/store/apps/details?id=com.youmusic.magictiles&hl=vi) · [Magic Tiles 3 on App Store](https://apps.apple.com/us/app/magic-tiles-3-piano-game/id1443446174)

---

## 📋 Table of Contents

- [Gameplay Overview](#gameplay-overview)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Main Systems](#main-systems)
  - [Input System](#input-system)
  - [Notes System](#notes-system)
  - [Judgment System](#judgment-system)
  - [Score System](#score-system)
  - [Spawn System](#spawn-system)
  - [Audio System](#audio-system)
  - [VFX System](#vfx-system)
  - [Background System](#background-system)
- [Game Flow](#game-flow)
- [Data Format](#data-format)
- [Technical Decisions](#technical-decisions)
- [Current Status](#current-status)
- [Future Improvements](#future-improvements)

---

## 🎮 Gameplay Overview

- **Tap-based** — tap notes falling in 4 lanes
- **Note types:** Short, Long, Zigzag, Mood
- **Endless loop** — replay the song on completion; speed increases each loop
- **Collision detection** — timing-based judgment (Perfect/Great/Good/Miss)
- **Score tracking** — score, judgment feedback, progress bar
- **Slowing power-up** — temporary slow-motion (planned)
- **Miss** → immediate game over

---

## 🏗 Architecture

**Event-Driven Architecture** — all systems communicate through a central static event bus (`GameEvents`) with zero coupling between systems.

```
Input → HitDetectSystem → NoteBase.OnTabBegan
                              ↓
                      GameEvents.RaiseOnNoteHit
                              ↓
                     JudgmentSystem.HandleNoteHit
                              ↓
                      GameEvents.RaiseOnJudged
                              ↓
                     ScoreSystem.JudgmentHandler
                              ├── OnScoreChanged → GamePlayScreen (UI)
                              └── OnJudgedChanged → GamePlayScreen (UI)
```

**Design Patterns Used:**
| Pattern | Where |
|---------|-------|
| Event Bus | `GameEvents` static class — all cross-system communication |
| Abstract Factory / Pool | `NotesFactory` — object pool for note recycling |
| Singleton | `UIManager` — centralized UI management |
| State Machine | `LongNote` — `WaitingTab` → `Hold` → `Released` |
| Strategy (implicit) | `JudgmentType` enum — scoring varies by judgment |
| ScriptableObject Config | `JudgmentScoreConfig`, `SongDBSO` — data-driven tuning |

---

## 📁 Project Structure

```
Assets/
└── _Game/
    ├── Scripts/
    │   ├── Camera/
    │   │   └── ScreenDetector.cs        — Multi-resolution lane positioning
    │   ├── Core/
    │   │   ├── ITickable.cs              — Tick interface
    │   │   └── Singleton.cs              — Generic singleton base
    │   ├── Data/
    │   │   ├── NoteData.cs               — JSON data models
    │   │   ├── SongDBSO.cs               — Song asset reference
    │   │   ├── JudgmentScoreConfig.cs    — Score config SO
    │   │   └── NotesPoolConfig.cs        — Pool configuration
    │   ├── Game/
    │   │   ├── GameController.cs         — Main game loop & flow
    │   │   └── GameEvents.cs             — Central event bus
    │   ├── Input/
    │   │   └── InputReader.cs            — Touch & mouse input
    │   ├── Notes/
    │   │   ├── NoteBase.cs               — Abstract note base class
    │   │   ├── ShortNote.cs              — Single-tap note
    │   │   ├── LongNote.cs               — Hold note (state machine)
    │   │   ├── ZigzagNote.cs             — Slide note (TODO)
    │   │   └── MoodNote.cs               — Background-changing note
    │   ├── System/
    │   │   ├── AudioSystem.cs            — dspTime-scheduled audio
    │   │   ├── NotesSpawnSystem.cs       — Spawn & lifecycle management
    │   │   ├── NotesFactory.cs           — Object pool factory
    │   │   ├── HitDetectSystem.cs        — Hit detection & finger tracking
    │   │   ├── JudgmentSystem.cs         — Timing window evaluation
    │   │   ├── ScoreSystem.cs            — Score & combo management
    │   │   ├── BackgroundSystem.cs       — Mood-driven background changes
    │   │   ├── VFXManager.cs             — Particle pool manager
    │   │   └── ReturnToPoolOnStop.cs     — Auto-return pooled particles
    │   └── UI/
    │       ├── UIBase.cs                 — Base UI with fade transitions
    │       ├── UIManager.cs              — UI state manager
    │       ├── GameStartScreen.cs        — Start screen
    │       ├── GamePlayScreen.cs         — In-game HUD (score, combo, progress)
    │       └── GameOverScreen.cs         — Game over with score display
    └── ... (Prefabs, Scenes, Art, Audio)
```

---

## ⚙️ Main Systems

### Input System (`InputReader.cs`)
- Supports both **touch** (Android) and **mouse** (Editor/PC)
- Converts screen position to lane index (1–4)
- Tracks active touches via `Dictionary<int, TouchInfo>` for multi-touch
- Fires `OnTabBegan`, `OnTabEnded`, `OnHeldTab` events

### Notes System (`NoteBase` → `ShortNote`, `LongNote`, `MoodNote`, `ZigzagNote`)
- **NoteBase** — abstract class with `OnTick`, `OnTabBegan`, `OnTabEnded`, `OnHeld`
- Handles scrolling, hit timeout → game over, fade-out on complete
- **ShortNote** — single tap → fire `NoteHitEvent` + spawn VFX
- **LongNote** — state machine (`WaitingTab` → `Hold` → `Released`):
  - Hold phase: visual fill shrinks as time passes
  - Release: fire `NoteHitEvent` for final judgment
  - Auto-complete on duration expired (planned)
- **MoodNote** — tap → background change via `OnMoodChanged` event

### Judgment System (`JudgmentSystem.cs`)
- Subscribes to `GameEvents.OnNoteHit`
- Evaluates `delta = |note.HitTime - tabTime|`
- Timing windows (configurable via Inspector):
  - **Perfect** ≤ 0.10s
  - **Great** ≤ 0.15s
  - **Good** ≤ 0.20s
  - **Miss** > 0.46s
- Fires `GameEvents.RaiseOnJudged(type)`

### Score System (`ScoreSystem.cs`)
- Subscribes to `GameEvents.OnJudged`
- Looks up score value from `JudgmentScoreConfig` ScriptableObject
- Fires `OnScoreChange` and `OnJudgedChange` events for UI
- Resets on `OnGameStarted` / `OnGameReset`
- **Combo tracking is planned** (currently missing)

### Spawn System (`NotesSpawnSystem.cs`)
- Reads note data from JSON (deserialized by `GameController`)
- Spawns notes at calculated Y positions based on timing + scroll speed
- Manages active note lifecycle (add/remove as notes scroll off-screen)
- Supports restart (pool all notes, reset index)

### Audio System (`AudioSystem.cs`)
- Uses `AudioSettings.dspTime` for precise scheduling
- `PlayScheduled` for sample-accurate sync
- Configurable pre-delay (0.2s default) for visual lead-in
- Reports `SongTime`, `Progress`, `IsSongFinished`

### VFX System (`VFXManager.cs`)
- ObjectPool-based particle management
- Supports multiple VFX types (`Hit`, `Tab`, `Mood`)
- Auto-returns particles to pool on stop via `ReturnToPoolOnStop`

### Background System (`BackgroundSystem.cs`)
- Subscribes to `OnMoodChanged` events from MoodNotes
- Swaps background sprite by key
- Bounce scale animation on change (DOTween)

---

## 🔄 Game Flow

```
GameStart → GameStartScreen (menu)
                ↓ (tap Start)
           GameEvents.RaiseOnGameStarted()
                ↓
           GameController.GameStarted()
                ├── ResetProgress()
                ├── SetUpSong() → AudioSystem.InitSound + StartSong
                └── UIManager.ShowUI<GamePlayScreen>()
                ↓
           Update Loop:
                ├── AudioSystem.SongTime → NotesSpawnSystem.OnTick
                ├── InputReader active
                ├── HitDetectSystem matches taps to notes
                └── Check complete → StartNewLoop()
                ↓
           On Miss → GameEvents.RaiseOnGameOver()
                ↓
           GameOverScreen (score display)
                ↓ (tap Restart)
           GameEvents.RaiseOnGameReset()
                ↓
           Back to GameStarted()
```

---

## 📊 Data Format

Notes are defined in JSON:

```json
{
  "notes": [
    {
      "lane": 1,
      "time": 1.5,
      "type": "Short",
      "metas": [],
      "controls": [],
      "duration": 0
    }
  ],
  "songMeta": {
    "bpm": 120,
    "nLanes": 4,
    "visualSpeed": 4.0,
    "audioDuration": 30.0
  },
  "format": "v1"
}
```

**Note Types:**
- `Short` — single tap
- `Long` — hold note (`duration` indicates hold length)
- `Zigzag` — slide between lanes (`controls` define path)
- `Mood` — background change (`metas` contain `bg_color` key)

---

## 🧠 Technical Decisions & Trade-offs

| Decision | Rationale |
|----------|-----------|
| **Static event bus** (`GameEvents`) over DI | Simpler setup, runtime coupling via subscribe/unsubscribe |
| **`dspTime` scheduling** for audio | Frame-independent timing for precise note-judgment sync |
| **Object pooling** for notes & VFX | Zero allocation during gameplay; essential for mobile |
| **JSON-driven notes** | Data and logic separation; non-developers can author levels |
| **Singleton UIManager** | Simple UI state machine; adequate for 3 screens |
| **Hardcoded timing windows** (Inspector) | Quick iteration; `JudgmentScoreConfig` SO for score values |
| **No external dependency injection** | Keeps project scope small; manual wiring in scene is manageable for this scale |

**Known Simplifications:**
- ZigzagNote is structurally defined but not fully implemented
- No combo multiplier in ScoreSystem yet
- Endless loop restarts without accelerating (speed increase pending)
- Power-up (slow-down) not implemented
- AudioSystem delay (0.2s) is hardcoded
- HitDetectSystem max detection range (0.5s) is hardcoded

---

## 📈 Current Status

| Feature | Status | Notes |
|---------|--------|-------|
| Short note tap + judgment | ✅ Done | Event-driven |
| Long note hold + release | ✅ Done | 2-phase state machine |
| Mood note + background | ✅ Done | Event-driven |
| Scoring system | ✅ Done | Config-driven |
| UI flow (start → play → over) | ✅ Done | 3 UIs with transitions |
| VFX pooling | ✅ Done | ObjectPool |
| Zigzag note | 🟡 Partial | Data model exists, gameplay not wired |
| Combo tracking | ❌ Missing | ScoreSystem has no combo logic |
| Endless speed increase | ❌ Missing | `StartNewLoop()` restarts at same speed |
| Slow-down power-up | ❌ Missing | Not started |
| Miss = game over (no HP) | ✅ Done | One miss = game over |
| Multi-resolution support | 🟡 Partial | ScreenDetector positions lanes by aspect ratio |

---

## 🚀 Future Improvements

1. **Combo System** — Track consecutive hits; bonus score per combo step; display combo counter with animation

2. **Endless Speed Scaling** — Increase scroll speed and/or note density each loop; exponential difficulty curve

3. **Slow-Down Power-Up** — Temporary speed reduction button; UI cooldown indicator; strategic use cases in dense sections

4. **HP / Life System** — Instead of instant game over, use HP bar; misses drain HP, perfect hits regenerate

5. **Zigzag Note Implementation** — Implement smooth lane-to-lane interpolation using `controls` array data

6. **Result Screen Enhancement** — Show judgment breakdown (Perfect/Great/Good/Miss counts), max combo, grade (S/A/B/C)

7. **Audio Calibration** — Make `AudioSystem._delay` configurable per device; add calibration screen

8. **Multiple Songs / Song Select** — Song selection menu with JSON-defined song list; each song has unique data and BGM

9. **Visual Effects** — Lane glow on hit, screen shake on miss, judgment text popups ("Perfect!", "Great!"), note trail effects

10. **Mobile Optimization** — Touch input smoothing, resolution-independent UI (Canvas Scaler), reduced draw calls via sprite atlas

---

## 🛠 Requirements

- **Unity** 2022.3 LTS or newer
- **Target Platform:** PC (Editor/Standalone) or Android/iOS
- **Dependencies:**
  - `Newtonsoft.Json` (JSON parsing)
  - `DG.Tweening` (DOTween — animations)

---

*Built for the Amanotes Senior Game Unity Developer assessment — July 2026*
