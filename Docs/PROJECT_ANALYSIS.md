# PROJECT ANALYSIS — Đua Vỏ Lãi Prototype (Unity)

## 1) Executive Summary
Prototype hiện tại đã có loop gameplay cốt lõi: điều khiển vỏ lãi, drift/boost, checkpoint/lap, respawn, HUD và một số thành phần multiplayer với Netcode for GameObjects (NGO). Kiến trúc đang ở trạng thái **feature-first prototype**: logic tập trung trong MonoBehaviour, nhiều tham chiếu singleton/find động, phù hợp cho tốc độ làm prototype nhưng chưa sẵn sàng scale production.

## 2) Current Architecture Snapshot

### Runtime domains
- **Gameplay core**: `BoatController`, `BoatPlayerInput`, `RaceProgress`, `RaceManager`, `RespawnHandler`.
- **World systems**: spline river, water FX, checkpoint triggers.
- **UI layer**: HUD, minimap, debug, respawn fade, mobile controls.
- **Device/quality**: mobile quality preset + URP mobile/PC assets.

### Networking status
- Đã dùng `NetworkBehaviour`, `NetworkVariable`, `ServerRpc`, `ClientRpc` cho các phần race state/progress/respawn.
- Tuy nhiên chưa có phân tầng đầy đủ giữa simulation server-authoritative và presentation client-local (nhiều script vẫn dùng `FindObjectsByType`, `Camera.main`, singleton tại runtime).

## 3) Detailed Evaluation

## 3.1 Folder structure

**Hiện tại tốt**
- Có tách `_Project` khỏi package/sample assets.
- Scripts chia theo `Core/Gameplay/UI`.
- Có tách `Prefabs`, `Scenes`, `Materials`, `Settings`, `Audio`, `Art`.

**Vấn đề**
- Vẫn còn nhiều asset sample/temporary trong repo (`Assets/TextMesh Pro/Examples & Extras`, `GeneratedAssets`, `AI Toolkit/Temp`) làm phình project và khó maintain.
- `Assets/_Project/Scenes/Levels` và `System` chưa có scene thực tế đi kèm (chỉ `.meta`), trong khi scene chạy ở `Assets/Scenes/SampleScene.unity`.
- Chưa có kiến trúc “assembly definition + bounded contexts” cho scale team.

## 3.2 Scene organization
- Build hiện chỉ có `Assets/Scenes/SampleScene.unity`.
- Chưa có bootstrap scene, loading scene, gameplay scene additive, UI overlay scene, network bootstrap scene tách biệt.
- Rủi ro khi mở rộng: khó stream level, khó quản lý lifecycle systems, khó test automation theo scene.

## 3.3 Scripts

### Ưu điểm
- Có namespace thống nhất `MienTayDaiChien.*`.
- Nhiều thành phần gameplay đã tách class riêng (camera/audio/feedback/input/progress/respawn).
- Có groundwork multiplayer (NGO).

### Rủi ro/hardcode/duplication
- **Hardcoded lookup/runtime coupling**:
  - `GameObject.Find("SpeedLines_VFX")`.
  - `Camera.main` ở nhiều nơi.
  - `Object.FindObjectsByType` liên tục trong `Update` (HUD/minimap/debug).
- **Duplication UI logic**: `HUDManager` và `RaceHUD` chồng chéo chức năng (lap/ranking/timer/navigation).
- **Race state coupling**: nhiều class đọc trực tiếp `RaceManager.Instance` và network variables, gây phụ thuộc mạnh singleton.
- **Excessive logs in hot path**: `BoatController.FixedUpdate` log mỗi physics tick khi có vận tốc > 0.1f gây CPU/Garbage tăng mạnh trên mobile.
- **Physics + visual trộn nhau**: `BoatController` gánh cả movement, drift charge, boost meter; khó test và khó thay thế model vật lý.

## 3.4 Prefabs
- Prefab hệ môi trường và vehicle đã khá đầy đủ cho prototype.
- Thiếu prefab variants theo chất lượng (Mobile/PC), thiếu chuẩn “authoring prefab vs runtime spawned prefab”.
- Chưa thấy pooling strategy cho FX/impact object (Instantiate trong runtime collision/destruction).

## 3.5 Materials
- Có shader riêng cho river water (`StylizedRiverWater.shader`) và material final.
- Chưa thấy material profile theo quality tier (LOD shader variant, keyword management, texture compression policy tài liệu hóa).

## 3.6 URP setup

**Điểm tốt**
- Có tách `PC_RPAsset` và `Mobile_RPAsset`.
- Mobile đã giảm render scale và tắt một số tính năng nặng (không additional light shadows).

**Rủi ro**
- `MobileQualityManager` chỉnh trực tiếp `UniversalRenderPipeline.asset` runtime → thay đổi global pipeline state, có thể side-effect ngoài ý muốn.
- PC renderer bật SSAO feature; nếu dùng chung scene/material không kiểm soát volume/profile có thể tốn GPU.
- Thiếu ma trận chất lượng cụ thể theo device tier + telemetry FPS.

## 3.7 Gameplay flow
Flow hiện tại:
1. RaceManager chuyển Waiting -> Countdown -> Racing.
2. Boat nhận input, xử lý drift/boost, tiến qua checkpoint.
3. RaceProgress cập nhật spline distance/wrong-way trên server.
4. RespawnHandler tự động/manual respawn khi stuck/off-track.
5. HUD/minimap hiển thị thông tin đua.

**Nhận xét**
- Loop hoạt động cho prototype arcade.
- Chưa có state machine gameplay cấp cao (pre-race, intro cam, results, rematch, lobby reconnect).
- Chưa có event bus/command pipeline, nên feature mới sẽ dễ tạo “spaghetti dependencies”.

## 4) Key Issues

## 4.1 Hardcoded systems
- Magic numbers rải rác trong boat physics, camera, UI arrows.
- String-based object lookup, singleton direct access.
- Scene assumptions (single scene, static references).

## 4.2 Mobile performance risks
- `FindObjectsByType` trong `Update` nhiều script.
- Debug logs trong `FixedUpdate`.
- Runtime instantiate hiệu ứng va chạm/huỷ object không pooling.
- Potential overdraw: VFX + UI + post-process nếu không tiering rõ.

## 4.3 Scalability risks
- MonoBehaviour-first không có service abstraction/ports.
- Logic gameplay + net sync + presentation còn lẫn.
- Thiếu module boundaries bằng asmdef/feature packages.

## 4.4 Duplicated logic
- HUDManager vs RaceHUD.
- Nhiều nơi tự tìm local player tương tự nhau.
- Arrow/navigation logic lặp lại.

## 5) Production-Scale Proposal

## 5.1 Scalable architecture (Unity)
- **Layered feature architecture**:
  - `Domain`: pure C# race rules, lap/checkpoint validation, ranking.
  - `Application`: use-cases (StartRace, ApplyCheckpoint, RequestRespawn).
  - `Infrastructure`: NGO adapters, spline adapters, persistence.
  - `Presentation`: MonoBehaviours + UI binding.
- Dùng `ScriptableObject` cho config data (BoatStats, RaceRules, QualityProfiles).
- Dùng event bus (C# events/MessagePipe/UnityEvent bridge) để giảm singleton coupling.

## 5.2 Modular gameplay systems
- Tách Boat thành module:
  - `BoatMovementMotor`
  - `BoatDriftSystem`
  - `BoatBoostSystem`
  - `BoatStateReplicator`
- Tách UI theo view-model đồng nhất, chỉ 1 nguồn race HUD state.
- Standardize interfaces: `ILocalPlayerProvider`, `IRaceStateReader`, `ICheckpointNavigator`.

## 5.3 Multiplayer-ready structure
- Server-authoritative simulation rõ ràng:
  - Input command từ client -> server simulation -> snapshot replication.
- Tách “predicted local feel” và “authoritative correction”.
- Chuẩn hóa network prefab registry + spawn lifecycle + reconnect flow.

## 5.4 Optimization roadmap (high impact)
1. Xóa log trong hot path và thêm compile symbol `ENABLE_PROTOTYPE_LOGS`.
2. Cache references thay cho find trong Update.
3. Gộp HUD scripts trùng lặp.
4. Pooling cho splash/collision/destruction VFX.
5. Thiết lập device tier matrix (Low/Mid/High) với mục tiêu FPS + GPU frame time.
6. Audit texture import/compression và mesh LOD cho environment assets.

## 6) Suggested Target Folder Architecture

```text
Assets/_Project
  /Runtime
    /Domain
    /Application
    /Infrastructure
      /Networking
      /Spline
      /Persistence
    /Presentation
      /Gameplay
      /UI
  /Content
    /Prefabs
    /Materials
    /Scenes
    /Audio
  /Config
    /ScriptableObjects
    /Quality
  /Tests
    /EditMode
    /PlayMode
```

## 7) Migration Strategy (No Gameplay Break)
- Giữ nguyên prototype scene/prefabs hiện tại.
- Bọc logic cũ qua adapter trước khi refactor core.
- Refactor theo vertical slice nhỏ (HUD -> Player provider -> Boat modules) và kiểm tra playtest mỗi bước.

## 8) Implementation Update (Vertical Slice Foundation)
- Đã áp dụng throttled lookup cho local player discovery trong các script HUD/minimap/debug nhằm giảm CPU cost per-frame.
- Đã loại bỏ debug logs trong hot path của `BoatController` để giảm GC pressure và main-thread overhead.
- Đã đưa tuning chính sang ScriptableObject configs cho boat/race/mobile quality, giữ fallback để không phá behavior hiện có.
- Đã bổ sung bản đồ scene architecture additive theo từng nhóm hệ thống mà không thay scene gameplay hiện tại.
