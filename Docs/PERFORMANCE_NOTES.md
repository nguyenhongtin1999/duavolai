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

## Profiler snapshot recommendations
- Capture 3 snapshots: normal race, sustained drift, repeated boost sections.
- Record CPU (main thread), GC alloc/frame, draw calls, particle count.
- Compare with/without camera collision checks on low-end profile.

## Scalability recommendations
- Add particle quality multiplier (1.0 / 0.7 / 0.45) bound to mobile quality presets.
- Reduce ambience update rate if CPU > budget on low-end.
- Disable side spray first before disabling core wake/splash readability FX.


## Environment perf pass notes
- Add `EnvironmentVisibilityScaler` to distant environment clusters to cut renderer/collider overhead outside racing bubble.
- Keep `AudioZones` transitions light and avoid many overlapping ambient emitters.
- Use readability beacons sparingly at high-speed decision points only.
