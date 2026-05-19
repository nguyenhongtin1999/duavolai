# Technical Debt

1. HUDManager and RaceHUD still overlap in responsibilities and should be unified.
2. RaceManager singleton dependency remains broad across systems.
3. Water collision/ripple effects still need pooling pass.
4. No automated feel-regression tests for boat handling and camera behavior.
5. Scene split is planned but additive runtime loading is not yet implemented.
6. BoatController still bundles movement + drift + boost logic in one class; next slice should extract BoatPhysics core while preserving current API.
7. Camera collision uses per-frame sphere-cast; low-end mobile tier may need reduced check frequency.
