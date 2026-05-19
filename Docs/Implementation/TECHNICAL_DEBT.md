# Technical Debt (Actionable)

1. `RaceManager.Instance` singleton coupling still present across gameplay/UI scripts.
2. HUD responsibilities still split between `HUDManager` and `RaceHUD` (needs unification slice).
3. Several camera/UI references still rely on `Camera.main`.
4. VFX spawn paths still instantiate objects directly in collision/destruction hooks.
5. No asmdef boundaries yet for Gameplay/UI/Core domains.
6. No PlayMode/EditMode automated tests for race rules and respawn flows.
