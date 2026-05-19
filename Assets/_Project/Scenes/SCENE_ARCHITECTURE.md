# Scene Architecture Plan (Incremental, Non-breaking)

Current playable scene remains: `Assets/Scenes/SampleScene.unity`.

Target additive split for vertical slice foundation:
- `Assets/_Project/Scenes/Bootstrap` — boot sequence, service wiring, quality selection.
- `Assets/_Project/Scenes/Gameplay` — race logic, player/networked entities.
- `Assets/_Project/Scenes/UI` — HUD, menus, overlays.
- `Assets/_Project/Scenes/Audio` — mixers, music/sfx emitters.
- `Assets/_Project/Scenes/Environment` — static level geometry and ambience.

Migration rule:
1. Keep SampleScene as integration scene.
2. Move one system at a time to additive scenes.
3. Validate race loop after each extraction.
