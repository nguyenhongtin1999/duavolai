# ATMOSPHERE DIRECTION — Mekong Delta Feel

## Emotional target
- Cảm giác sông nước miền Tây: ẩm, thoáng, có sức sống, nhưng vẫn rõ đường đua.

## Implemented hooks
- `RiverAtmosphereHooks` + `RiverAtmosphereProfile`:
  - river bed ambience
  - wind modulation by speed
  - birds layer that reduces at high speed
  - distant boat ambience growth with speed
- `AtmosphereZone` for local ambience overrides along route.

## Lighting/Fog recommendations
- Sunset-biased warm key light; avoid over-saturation.
- Use layered fog nhẹ: gần mỏng, xa dày vừa đủ để tách depth.
- Keep race-critical landmarks high contrast.

## Humidity impression (lightweight)
- Audio-first: river + birds + wind transitions.
- Visual secondary: haze and warm sky grading (low cost).


## Audio zone recommendations
- `AudioZones` nên đặt theo khu vực: chợ nổi, đoạn rừng dừa nước, đoạn dân cư nhà sàn.
- Mỗi zone chỉ override 1-2 tham số chính để tránh hỗn tạp âm thanh.

## Sunset haze / fog layering
- Dùng fog tầng xa để tách depth, giữ foreground rõ cho obstacle/checkpoint.
- Tránh fog quá dày ở tầm nhìn ngắn vì ảnh hưởng phản xạ khi boost.

## Performance guardrails
- Ambience updates theo nhịp (interval) thay vì full-frame polling.
- Birds/distant-boat layers giảm volume ở tốc độ cao để ưu tiên âm thanh điều khiển.
