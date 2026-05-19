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
