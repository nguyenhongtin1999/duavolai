# Gameplay Feel — Đua Vỏ Lãi

## Boat handling polish (implemented)
- Added speed-based acceleration curve and steering curve via `BoatPhysicsModel` for more arcade feel at low speed while keeping control at high speed.
- Added soft cap interpolation (`weightResponse`) so top-speed feels heavier instead of hard-clamped.
- Added water resistance blend for coasting to improve "boat gliding then drag" sensation.
- Kept existing race loop and input contracts unchanged.

## Camera feel (implemented)
- New modular camera rig `BoatCameraRig`:
  - follow lag
  - speed/boost FOV
  - drift tilt
  - shake
  - collision avoidance via sphere-cast
- Mobile goal: stronger readability with minimal CPU overhead.

## Water feel (lightweight path)
- Existing splash/wake/speed-lines are retained.
- Recommended next incremental step: object pooling for splash/ripple spawn emitters.
- Keep shader complexity low (no heavy screen-space water simulation).

## Atmosphere hooks
- Added `RiverAtmosphereHooks` to modulate wind ambience by boat speed.
- Recommendations:
  - River ambience loop constant low-mid volume.
  - Wind layer scales with speed for motion sensation.
  - Keep assets mono/stereo lightweight for mobile memory.

## Lighting/fog/sunset recommendations
- Use warm directional light profile (late afternoon/sunset).
- Apply height fog lightly to compress far geometry and reduce visual noise.
- Prefer baked/static where possible for mobile draw-call stability.

## Incremental modular split applied (this phase)
- BoatInput: kept in `BoatPlayerInput` (no loop rewrite), removed non-essential runtime log noise.
- BoatPhysics: feel tuning centralized through `BoatPhysicsModel` used by `BoatController`.
- BoatEffects: `BoatFeedbackEffects` now supports reference injection via `BoatPresentationRefs` to avoid hardcoded scene lookups.
- BoatCamera: `BoatCameraRig` remains modular follow/FOV/shake/tilt/collision component.
- BoatAudio: added lightweight ambience modulation hook directly in `BoatAudio` with throttled update cadence.

## Water interaction update
- Added `BoatWaterInteraction` for lightweight wake/spray/splash feedback with speed thresholds.
- No heavy shader simulation introduced.

## Cinematic feel pass (current)
- Speed readability increased by camera profile blending (low/high speed) and dynamic follow distance.
- Boost impact reinforced by stronger but bounded FOV + shake response.
- Drift readability improved via lean + side spray hooks.
- Momentum readability improved through wake/spray scaling and audio motion cues.

## Remaining weak areas
- Boat core still mixes movement/drift/boost in one class; split should continue carefully.
- Multi-boat FX scalability needs per-tier particle caps.

## Next recommended polish pass
- Add quality-tier multipliers for camera shake/FX intensity.
- Add short boost impulse timeline (camera/audio/FX in sync) using current components.
- Add lightweight ghost trail only on boost for top-tier devices (optional).
