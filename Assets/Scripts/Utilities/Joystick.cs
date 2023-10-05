using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public static Joystick Instance;
    [SerializeField] private RectTransform handle; // Reference to the joystick handle
    private RectTransform baseRect; // Reference to the joystick background/base
    private Vector2 inputDirection = Vector2.zero; // The direction of the joystick input

    private void Awake()
    {
        Instance = this;
        baseRect = transform as RectTransform; // Get the RectTransform of the base/background
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, eventData.position, eventData.pressEventCamera, out pos))
        {
            // Calculate the position relative to the center of the baseRect
            pos.x = (pos.x - baseRect.sizeDelta.x * 0.5f) / (baseRect.sizeDelta.x * 0.5f);
            pos.y = (pos.y - baseRect.sizeDelta.y * 0.5f) / (baseRect.sizeDelta.y * 0.5f);

            inputDirection = new Vector2(pos.x, pos.y);
            inputDirection = (inputDirection.magnitude > 1) ? inputDirection.normalized : inputDirection;

            // Move the handle while keeping it inside the baseRect
            handle.anchoredPosition = new Vector2(inputDirection.x * (baseRect.sizeDelta.x * 0.5f), inputDirection.y * (baseRect.sizeDelta.y * 0.5f));
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        inputDirection = Vector2.zero;
        handle.anchoredPosition = Vector2.zero; // Reset the handle position
    }

    // You can access the inputDirection variable to control your character or other elements in your game.
    public Vector2 GetMovementInput()
    {
        return inputDirection;
    }
}
