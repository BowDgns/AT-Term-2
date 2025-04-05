using UnityEngine;
using System.Collections;

public class MapToggle : MonoBehaviour
{
    [Header("Map UI Settings")]
    // Reference to the map's RectTransform (the UI Image holding your map).
    public RectTransform mapRect;
    // The position where the map is fully visible.
    public Vector2 onScreenPosition;
    // The position where the map is hidden off-screen.
    public Vector2 offScreenPosition;
    // Duration for the transition animation (in seconds).
    public float transitionDuration = 0.5f;

    // Tracks the current state of the map.
    private bool isVisible = false;
    // Reference to any ongoing transition.
    private Coroutine currentTransition;

    /// <summary>
    /// Call this method when the toggle button is pressed.
    /// </summary>
    public void ToggleMap()
    {
        // If a transition is already in progress, stop it.
        if (currentTransition != null)
            StopCoroutine(currentTransition);

        // Determine the target position based on current state.
        Vector2 targetPosition = isVisible ? offScreenPosition : onScreenPosition;
        // Start animating the map's movement.
        currentTransition = StartCoroutine(MoveMap(targetPosition));

        // Toggle the state.
        isVisible = !isVisible;
    }

    /// <summary>
    /// Smoothly moves the map to the target position.
    /// </summary>
    /// <param name="targetPosition">The anchored position to move the map to.</param>
    /// <returns></returns>
    private IEnumerator MoveMap(Vector2 targetPosition)
    {
        Vector2 startPosition = mapRect.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            // Lerp between the start and target positions.
            mapRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is set.
        mapRect.anchoredPosition = targetPosition;
    }
}
