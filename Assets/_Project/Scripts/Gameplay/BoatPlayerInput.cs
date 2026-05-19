using UnityEngine;
using UnityEngine.InputSystem;

namespace MienTayDaiChien.Gameplay
{
    public class BoatPlayerInput : MonoBehaviour
    {
        public InputActionAsset inputActions;
        
        private BoatController _controller;
        private RespawnHandler _respawnHandler;
        private InputAction _steerAction;
        private InputAction _accelAction;
        private InputAction _brakeAction;
        private InputAction _boostAction;
        private InputAction _driftAction;
        private InputAction _respawnAction;

        private void Awake()
        {
            _controller = GetComponent<BoatController>();
            _respawnHandler = GetComponent<RespawnHandler>();
            if (inputActions != null)
            {
                Initialize(inputActions);
            }
        }

        public void Initialize(InputActionAsset actions)
        {
            inputActions = actions;
            _steerAction = inputActions.FindAction("Steer");
            _accelAction = inputActions.FindAction("Accelerate");
            _brakeAction = inputActions.FindAction("Brake");
            _boostAction = inputActions.FindAction("Boost");
            _driftAction = inputActions.FindAction("Drift");
            _respawnAction = inputActions.FindAction("Respawn");
            
            if (_steerAction == null) Debug.LogError("[BoatPlayerInput] Steer action not found!");
            if (_accelAction == null) Debug.LogError("[BoatPlayerInput] Accelerate action not found!");

            inputActions.Enable();
        }

        private void Update()
        {
            float steer = _steerAction != null ? _steerAction.ReadValue<Vector2>().x : 0;
            float accel = _accelAction != null ? _accelAction.ReadValue<float>() : 0;
            float brake = _brakeAction != null ? _brakeAction.ReadValue<float>() : 0;
            bool drift = _driftAction != null && _driftAction.IsPressed();

            _controller.SetInput(steer, accel, brake, drift);

            if (_boostAction != null && _boostAction.WasPressedThisFrame())
            {
                _controller.TryBoost();
            }

            if (_respawnAction != null && _respawnAction.WasPressedThisFrame())
            {
                if (_respawnHandler != null) _respawnHandler.RequestRespawn();
            }
        }

        private void OnDisable()
        {
            if (inputActions != null) inputActions.Disable();
        }
}
}
