# CAMERA DESIGN — Đua Vỏ Lãi Vertical Slice

## Tuning philosophy
- Camera phải giữ hướng đi rõ ràng trước hết, cinematic đứng sau.
- Mọi hiệu ứng (shake/tilt/FOV) đều có trần mềm để tránh motion sickness trên mobile.

## Implemented system
- `BoatCameraRig` now supports:
  - low/high speed profile blending via `BoatCameraProfile`
  - dynamic follow distance by speed
  - drift lean + terrain-aware tilt
  - boost-aware FOV response
  - collision smoothing and recovery

## Readability rules
- Horizon stable, lean only on Z and at low amplitude.
- Camera clip mitigation must prioritize forward visibility.
- FOV response smoothed (no hard jumps).

## Mobile limitations
- Only one sphere-cast collision test per frame.
- No post-process dependency for speed feel.

## Multiplayer note
- Camera remains local-only presentation layer; does not touch authoritative race state.
