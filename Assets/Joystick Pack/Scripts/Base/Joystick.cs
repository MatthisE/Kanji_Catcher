using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static Joystick instance; //constant across all projects, makes player functions and vars usable in other scripts

    public float Horizontal => SnapX ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x;
    public float Vertical => SnapY ? SnapFloat(input.y, AxisOptions.Vertical) : input.y;
    public Vector2 Direction => new Vector2(Horizontal, Vertical);

    public float HandleRange
    { get => handleRange; set => handleRange = Mathf.Abs(value);
    }

    public float DeadZone
    { get => deadZone; set => deadZone = Mathf.Abs(value);
    }

    public AxisOptions AxisOptions { get => AxisOptions; set => axisOptions = value; }
    [field: SerializeField]
    public bool SnapX { get; set; } = false;
    [field: SerializeField]
    public bool SnapY { get; set; } = false;

    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;
    [SerializeField] protected RectTransform background = null;
    [SerializeField] private RectTransform handle = null;
    private RectTransform baseRect = null;

    private Canvas canvas;
    private Camera cam;

    private Vector2 input = Vector2.zero;

    protected virtual void Start()
    {
        // Singleton pattern: Ensure only one Joystick instance exists
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroy duplicate joysticks
        }

        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("The Joystick is not placed inside a canvas");
        }

        Vector2 center = new(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            cam = canvas.worldCamera;
        }

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * canvas.scaleFactor);
        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius, cam);
        handle.anchoredPosition = input * radius * handleRange;
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
            {
                input = normalised;
            }
        }
        else
        {
            input = Vector2.zero;
        }
    }

    private void FormatInput()
    {
        if (axisOptions == AxisOptions.Horizontal)
        {
            input = new Vector2(input.x, 0f);
        }
        else if (axisOptions == AxisOptions.Vertical)
        {
            input = new Vector2(0f, input.y);
        }
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
        {
            return value;
        }

        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                return angle is < 22.5f or > 157.5f ? 0 : (value > 0) ? 1 : -1;
            }
            else if (snapAxis == AxisOptions.Vertical)
            {
                return angle is > 67.5f and < 112.5f ? 0 : (value > 0) ? 1 : -1;
            }
            return value;
        }
        else
        {
            if (value > 0)
            {
                return 1;
            }

            if (value < 0)
            {
                return -1;
            }
        }
        return 0;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
        {
            Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }

    public static void ActivateJoystick(bool activate)
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(activate);
        }
        else
        {
            Debug.LogError("Joystick instance is not initialized. Ensure the Joystick prefab exists in the scene.");
        }
    }

    public void ResetJoystick()
    {
        input = Vector2.zero; // reset the input values
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero; // move handle back to the center
        }
    }
}

public enum AxisOptions { Both, Horizontal, Vertical }