# Performance Notes — Vertical Slice Baseline

## Current optimization applied
- Removed `BoatController` hot-path debug logs.
- Throttled repeated local-player lookups in HUD/minimap/debug scripts.
- Added camera collision avoidance with single sphere-cast per frame in camera only.

## Checklist to validate on device
- CPU spikes: monitor race start countdown and boost-heavy moments.
- GC alloc: verify 0B/frame target during steady race.
- Update/FixedUpdate cost: BoatController + HUD + camera scripts.
- Draw calls: focus on VFX spikes during boost/drift.
- Target: 60 FPS on mid devices.

## Next optimizations
- Pool splash/ripple/destruction FX objects.
- Cache frequently used camera and player refs through central local-player provider.
- Optional: reduce camera collision checks frequency on low-end tiers (e.g., every 2 frames).

## Additional optimizations applied in this phase
- Removed `GameObject.Find` fallback path from frequent effect flow by injecting references through `BoatPresentationRefs` (with one-time scene fallback only).
- Throttled `BoatAudio` runtime updates to ~30Hz to reduce per-frame cost on mobile while preserving audible responsiveness.
- Added lightweight `BoatWaterInteraction` particle toggling by speed thresholds.
