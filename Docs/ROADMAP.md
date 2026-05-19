# ROADMAP — Đua Vỏ Lãi (Prototype -> Production)

## M0 — Baseline Stabilization (1-2 tuần)
- Freeze gameplay baseline hiện tại (golden prototype scene).
- Gỡ log hot path, chuẩn hóa define symbols debug.
- Gom các reference runtime FindObjects/GameObject.Find về injected references.
- Gộp HUDManager + RaceHUD thành 1 pipeline UI.

## M1 — Architecture Foundation (2-3 tuần)
- Thiết kế Runtime layers: Domain/Application/Infrastructure/Presentation.
- Introduce asmdef cho từng module.
- Tách BoatController thành movement/drift/boost modules.
- Tạo `RaceRulesConfig`, `BoatConfig`, `QualityProfileConfig` bằng ScriptableObject.

## M2 — Multiplayer Hardening (3-5 tuần)
- Chuẩn hóa server-authoritative tick và input command flow.
- Tạo reconnection flow + race rejoin handling.
- Xây snapshot/interpolation strategy cho remote boats.
- Instrument network metrics (RTT, packet loss, correction frequency).

## M3 — Content & Scene Pipeline (2-4 tuần)
- Chuyển sang additive scenes: Bootstrap, Network, Level, UI.
- Thiết kế prefab variants theo quality tiers.
- Thiết lập addressables/bundling strategy cho live ops.

## M4 — Mobile Optimization (liên tục, checkpoint mỗi 2 tuần)
- Profiler pass: CPU main thread, render thread, GPU.
- Pooling VFX/destruction.
- Texture compression, mipmap strategy, LOD policy.
- URP per-tier tuning + thermal/battery stress tests.

## M5 — QA & Live Readiness
- Thiết lập PlayMode/EditMode test suite cho race rules.
- Smoke test automation cho scene boot, race complete, respawn, reconnect.
- Crash/analytics pipeline.
- Release checklist và rollback strategy.

## KPI mục tiêu
- Mid-tier Android: >= 60 FPS ổn định race 4-8 boats.
- 1% low frame spikes < 25ms (CPU) và < 20ms (GPU).
- Desync correction trung bình < 2 correction/10s/player.

## Immediate actionable milestones (next 2 sprints)
- Sprint A: Hợp nhất HUDManager + RaceHUD và chuẩn hóa local-player provider dùng chung.
- Sprint A: Tạo PlayMode smoke tests cho race loop (countdown, finish, respawn).
- Sprint B: Tách BoatController thành các module nhỏ (movement/boost/drift) nhưng giữ API cũ để tránh break prefab.
- Sprint B: Áp object pooling cho splash/collision/destruction VFX.
