# WATER FEEL — Đua Vỏ Lãi Vertical Slice

## Goals
- Nước phải cho người chơi "cảm giác trôi + nặng + nhanh".
- Hiệu ứng rõ ở tốc độ cao/boost/drift nhưng không spam hạt.

## Implemented
- `BoatWaterInteraction`:
  - wake intensity scales by speed
  - speed spray scales by speed/boost
  - drift side spray for lateral motion readability
  - collision splash + ripple burst hooks

## Performance constraints
- Particle modulation only (rate + enable/disable).
- No heavy realtime simulation.
- Compatible with URP mobile.

## Next pass
- Optional pooled shared emitter set for multi-boat scenes.
- Add quality-tier multipliers to reduce particle rates on low-end devices.
