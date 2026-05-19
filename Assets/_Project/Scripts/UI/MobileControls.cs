using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using TMPro;

namespace MienTayDaiChien.UI
{
    /// <summary>
    /// Floating joystick that centers on touch start.
    /// </summary>
    public class MobileJoystick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [InputControl(layout = "Vector2")]
        [SerializeField] private string m_ControlPath;

        public RectTransform container;
        public RectTransform handle;
        public float movementRange = 100f;

        private Vector2 _startPos;
        private CanvasGroup _canvasGroup;

        protected override string controlPathInternal { get => m_ControlPath; set => m_ControlPath = value; }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _startPos = container.anchoredPosition;
            // Start hidden or semi-transparent for "floating"
            _canvasGroup.alpha = 0.5f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPos))
            {
                container.anchoredPosition = localPos;
                _canvasGroup.alpha = 1f;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(container, eventData.position, eventData.pressEventCamera, out Vector2 localPos))
            {
                Vector2 delta = localPos;
                delta = Vector2.ClampMagnitude(delta, movementRange);
                handle.anchoredPosition = delta;

                SendValueToControl(delta / movementRange);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            handle.anchoredPosition = Vector2.zero;
            container.anchoredPosition = _startPos;
            _canvasGroup.alpha = 0.5f;
            SendValueToControl(Vector2.zero);
        }
    }

    /// <summary>
    /// Sends button state to Input System for mobile buttons.
    /// </summary>
    public class MobileButtonBridge : OnScreenControl, IPointerDownHandler, IPointerUpHandler
    {
        [InputControl(layout = "Button")]
        [SerializeField] private string m_ControlPath;
        protected override string controlPathInternal { get => m_ControlPath; set => m_ControlPath = value; }

        public void OnPointerDown(PointerEventData eventData) => SendValueToControl(1.0f);
        public void OnPointerUp(PointerEventData eventData) => SendValueToControl(0.0f);
    }

    /// <summary>
    /// Sends float axis state to Input System for mobile pedals (Accel/Brake).
    /// </summary>
    public class MobileAxisBridge : OnScreenControl, IPointerDownHandler, IPointerUpHandler
    {
        [InputControl(layout = "Axis")]
        [SerializeField] private string m_ControlPath;
        protected override string controlPathInternal { get => m_ControlPath; set => m_ControlPath = value; }

        public void OnPointerDown(PointerEventData eventData) => SendValueToControl(1.0f);
        public void OnPointerUp(PointerEventData eventData) => SendValueToControl(0.0f);
    }
}
