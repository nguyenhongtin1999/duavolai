# AGENTS.md — Workflow cho AI Contributors

## 1) Mission
Dự án này là prototype gameplay cho game **Miền Tây Đại Chiến**. Mọi thay đổi phải ưu tiên:
1. Không làm vỡ gameplay baseline hiện tại.
2. Tăng tính mở rộng (architecture, modularity).
3. Tối ưu cho mobile và sẵn sàng multiplayer.

## 2) Working rules
- Luôn đọc `README.md`, `Docs/PROJECT_ANALYSIS.md`, `Docs/ROADMAP.md` trước khi sửa kiến trúc lớn.
- Khi sửa code gameplay, giữ backward-compatible behavior nếu không có yêu cầu thay đổi gameplay rõ ràng.
- Tránh thêm singleton/global state mới trừ khi có justification.
- Tránh `FindObject*` trong `Update` và tránh `GameObject.Find` trừ trường hợp tạm thời có TODO rõ.
- Không để `Debug.Log` spam trong `Update/FixedUpdate` ở bản production path.

## 3) Code organization conventions
- Namespace chuẩn: `MienTayDaiChien.<Domain>`.
- Ưu tiên tách logic:
  - Domain rules (pure C#) tách khỏi MonoBehaviour.
  - Networking adapter tách khỏi gameplay state machine.
  - UI binding tách khỏi data/state calculation.
- Thêm `asmdef` khi module đủ ổn định.

## 4) Performance checklist (mobile)
- Kiểm tra allocations/frame khi thêm UI/gameplay loop.
- Dùng object pooling cho VFX spawn thường xuyên.
- Cache component refs; không `GetComponent` lặp không cần thiết trong hot paths.
- Test ít nhất ở 2 tier: low-end Android profile và mid-tier profile.

## 5) Multiplayer checklist
- Xác định rõ logic nào authoritative server.
- RPC phải có mục tiêu rõ và hạn chế gửi mỗi frame.
- NetworkVariable chỉ sync state cần thiết.
- Có chiến lược xử lý late-join/reconnect khi thêm feature race state.

## 6) PR/Commit conventions
- Commit theo milestone nhỏ, message dạng:
  - `docs(analysis): ...`
  - `refactor(gameplay): ...`
  - `perf(mobile): ...`
  - `netcode(race): ...`
- Mỗi PR cần nêu:
  - Mục tiêu.
  - Phạm vi thay đổi.
  - Rủi ro/rollback.
  - Cách test thủ công.

## 7) Safe rollout strategy
- Nếu refactor lớn: tạo adapter layer, migrate dần, xóa code cũ sau khi verified.
- Với gameplay critical paths (boat movement, race progress, respawn): ưu tiên snapshot test/playtest script trước khi merge.

## 8) Implementation notes policy
- Khi thêm/refactor hệ thống gameplay, cập nhật `Docs/Implementation/IMPLEMENTATION_NOTES.md`.
- Khi trì hoãn cải tiến quan trọng, ghi rõ vào `Docs/Implementation/TECHNICAL_DEBT.md` với bước hành động kế tiếp.
