# ENVIRONMENT GUIDE — Production-Useful Rules

## 1) Gameplay readability first
- Đường đua nhìn rõ khi boost.
- Obstacle silhouette rõ nền trời/nước.
- Checkpoint không bị foliage che ở khoảng cách trung bình.

## 2) Atmosphere without clutter
- Ambience layers nên tăng cảm giác (sông/gió/chim/thuyền xa) nhưng không lấn cue gameplay.
- Dùng `AtmosphereZone` cho local mood transitions, tránh thay đổi toàn cục đột ngột.

## 3) Performance-safe environment tools
- `EnvironmentVisibilityScaler`: dùng cho cụm props xa để giảm render/collider cost.
- `MapReadabilityBeacon`: dùng cho điểm định hướng quan trọng (turn-in/checkpoint gate).
- `RiverAtmosphereHooks` update theo interval để giảm polling cost.

## 4) Obstacle placement rules
- Tránh obstacle ngẫu nhiên ở apex mà không có tín hiệu dẫn hướng trước.
- Tại tốc độ cao, khoảng cách phản ứng cần đủ lớn (landmark báo trước).

## 5) Mobile optimization checklist
- Audit collider thừa trên props không tương tác.
- Gộp material trùng lặp.
- Hạn chế transparency stack dày trên cùng một góc camera.
- Kiểm tra draw calls khi bật đầy đủ wake/spray/splash.

## 6) Multiplayer-aware concerns
- Tránh environment FX phụ thuộc tuyệt đối vào local-only state nếu ảnh hưởng gameplay readability.
- Landmark/navigation cues cần đồng nhất giữa client.
