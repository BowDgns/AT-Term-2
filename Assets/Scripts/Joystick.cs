using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform background;
    public RectTransform handle;

    // normalised input
    private Vector2 inputVector = Vector2.zero;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // center joystick to background against other aspect ratios 
        Vector2 bgScreenPos = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, background.position);
        Vector2 delta = eventData.position - bgScreenPos;

        float radius = background.sizeDelta.x / 2f;
        inputVector = delta / radius;

        if (inputVector.magnitude > 1f)
            inputVector = inputVector.normalized;

        // move the handle based on input
        handle.anchoredPosition = inputVector * radius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // reset when released
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    public float Horizontal()
    {
        return inputVector.x;
    }

    public float Vertical()
    {
        return inputVector.y;
    }
}
