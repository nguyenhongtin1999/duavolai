# Technical Debt

1. HUDManager and RaceHUD still overlap in responsibilities and should be unified.
2. RaceManager singleton dependency remains broad across systems.
3. Water collision/ripple effects still need pooling pass.
4. No automated feel-regression tests for boat handling and camera behavior.
5. Scene split is planned but additive runtime loading is not yet implemented.
