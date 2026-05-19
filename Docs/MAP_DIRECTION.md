# MAP DIRECTION — Mekong Delta Racing Slice

## Environment philosophy
- Map phải đọc được ở tốc độ cao trước, đẹp sau.
- Mỗi đoạn sông cần có "câu chuyện nhỏ": nhà sàn, ghe chợ, bờ cây, cọc dẫn hướng.
- Tránh clutter ở line đua chính; chi tiết dày hơn ở vùng ngoại vi.

## Recommended hierarchy (scene root)
- `River`
- `Foliage`
- `Villages`
- `FloatingMarket`
- `Props`
- `Obstacles`
- `FX`
- `AudioZones`

## Naming/readability rules
- Checkpoint/obstacle phải có prefix chức năng: `CP_`, `OBS_`, `NAV_`, `FX_`.
- Landmark dẫn hướng tại cua gắt: cờ, cổng tre, cột đèn lồng phải nằm trong cone nhìn 30-45 độ.
- Drift corner cần tối thiểu 2 lớp tín hiệu: landmark xa + obstacle gần.

## River flow guidance
- Mỗi khúc cua nên có chuỗi vật thể dẫn flow theo chiều cong (lantern poles, flags, bamboo fences).
- Tránh đặt props cao che checkpoint ở khoảng cách 25–40m trước checkpoint.

## Mobile constraints
- Giữ mật độ foliage vừa phải tại vùng gameplay chính.
- Ưu tiên mesh/prefab lặp lại cùng material để giảm draw calls.
- FX readability > FX số lượng.

## Future expansion (safe)
- Mở rộng theo segment, không mở rộng toàn map đồng loạt.
- Mỗi segment mới phải pass readability review ở tốc độ boost.
