# Miền Tây Đại Chiến — Prototype Gameplay “Đua Vỏ Lãi”

Prototype Unity cho gameplay đua thuyền vỏ lãi theo phong cách arcade, có nền tảng sẵn cho mở rộng multiplayer bằng Netcode for GameObjects (NGO).

## Mục tiêu prototype
- Validate cảm giác lái: tăng tốc, drift, boost.
- Validate race loop: countdown -> racing -> finish.
- Validate systems nền: checkpoint/lap, respawn, minimap, HUD.
- Chuẩn bị nền kiến trúc cho scale production.

## Tech stack
- Unity + URP
- Unity Input System
- Unity Splines
- Unity Netcode for GameObjects (NGO)

## Cấu trúc chính
- `Assets/_Project/Scripts/Core`: quality/profile runtime.
- `Assets/_Project/Scripts/Gameplay`: thuyền, race, checkpoint, respawn, AI.
- `Assets/_Project/Scripts/UI`: HUD, minimap, debug, mobile controls.
- `Assets/_Project/Prefabs`: Vehicle/Environment/UI prefabs.
- `Assets/Settings`: URP assets PC/Mobile.

## Scene chạy hiện tại
- `Assets/Scenes/SampleScene.unity`

## Gameplay flow hiện tại
1. RaceManager quản lý state đua (`Waiting`, `Countdown`, `Racing`, `Finished`).
2. Boat nhận input và xử lý movement/drift/boost.
3. RaceProgress cập nhật checkpoint/lap/spline progress (server-authoritative).
4. RespawnHandler xử lý respawn auto/manual khi kẹt hoặc lệch track.
5. UI hiển thị timer/rank/lap/minimap/navigation.

## Tài liệu dự án
- Phân tích chi tiết: `Docs/PROJECT_ANALYSIS.md`
- Kế hoạch mở rộng: `Docs/ROADMAP.md`
- Hướng dẫn workflow AI: `AGENTS.md`

## Nguyên tắc phát triển tiếp theo
- Không phá gameplay prototype baseline.
- Refactor theo từng vertical slice nhỏ, luôn giữ playable.
- Ưu tiên mobile performance và multiplayer-authoritative correctness.

## Vertical Slice Stabilization (May 2026)
- Gameplay stabilization: giảm lookup spam, giảm log hot path, giảm coupling theo hướng incremental.
- Data-driven configs: Boat/Race/Mobile quality qua ScriptableObject.
- Scene architecture cleanup: thêm map additive scenes (Bootstrap/Gameplay/UI/Audio/Environment), giữ nguyên SampleScene đang chạy.
- Triển khai chi tiết: `Docs/Implementation/IMPLEMENTATION_NOTES.md`
- Technical debt: `Docs/Implementation/TECHNICAL_DEBT.md`
