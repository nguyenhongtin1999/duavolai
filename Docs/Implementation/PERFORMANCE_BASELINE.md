# Performance Baseline Checklist (Vertical Slice)

## Profiling checklist
- CPU: measure `PlayerLoop`, `Physics.Simulate`, UI scripts.
- GPU: frame time by quality tier (Mobile Low/Mid, PC).
- Memory: GC alloc/frame during 3-lap race.
- Network: server tick stability and race-state replication overhead.

## Mobile optimization checklist
- Remove hot-path logs and redundant `FindObjectsByType` calls.
- Verify URP mobile renderer uses minimized feature set.
- Validate resolution scale + shadow distance by tier.
- Audit textures (compression + mipmaps) for environment/UI/VFX.

## Pooling recommendations
- Pool splash/collision/destruction VFX prefabs.
- Pool temporary UI indicators spawned at runtime.
- Reuse audio one-shot emitters where possible.

## URP recommendations
- Keep SSAO/extra post-processing disabled for low tier.
- Prefer single directional light + baked/static lighting where possible.
- Lock additional lights and shadow settings by quality profile.
- Track shader variant count and strip unused keywords regularly.
