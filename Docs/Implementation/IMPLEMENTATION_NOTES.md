# Implementation Notes — Vertical Slice Stabilization

## Gameplay stabilization implemented
- Reduced per-frame lookup spam in UI scripts by throttling local-player discovery to 1-second intervals.
- Removed heavy runtime logs from `BoatController.FixedUpdate` and drift/boost hot paths.
- Kept runtime behavior intact while reducing avoidable CPU overhead.

## Data-driven configs implemented
- Added `BoatGameplayConfig` ScriptableObject for boat movement, boost, drift, and floating params.
- Added `RaceRulesConfig` ScriptableObject for lap/countdown/wrong-way threshold.
- Added `MobileQualityConfig` ScriptableObject for low/mid/high presets.
- Integrated configs into `BoatController`, `RaceManager`, and `MobileQualityManager` with safe fallback values.

## Scene architecture cleanup implemented
- Added explicit scene architecture map + folder structure for Bootstrap/Gameplay/UI/Audio/Environment.
- Kept current SampleScene untouched to avoid prototype breakage.
