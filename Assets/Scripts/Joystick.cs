using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // Reference to the joystick background and handle (set these in the Inspector)
    public RectTransform background;
    public RectTransform handle;

    // The normalized input vector (values between -1 and 1)
    private Vector2 inputVector = Vector2.zero;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Get the joystick center position in screen coordinates.
        Vector2 bgScreenPos = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, background.position);

        // Calculate the difference between the pointer's current position and the joystick center.
        Vector2 delta = eventData.position - bgScreenPos;

        // The radius of the joystick is half the width of the background.
        float radius = background.sizeDelta.x / 2f;

        // Normalize the delta by the radius.
        inputVector = delta / radius;

        // Clamp the vector so that it doesn't exceed a magnitude of 1.
        if (inputVector.magnitude > 1f)
            inputVector = inputVector.normalized;

        // Move the handle accordingly.
        handle.anchoredPosition = inputVector * radius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset the handle and input vector when the touch is released.
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    // Public accessors for horizontal and vertical input values.
    public float Horizontal()
    {
        return inputVector.x;
    }

    public float Vertical()
    {
        return inputVector.y;
    }
}
