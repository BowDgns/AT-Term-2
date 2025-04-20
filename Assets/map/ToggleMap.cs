using UnityEngine;
using System.Collections;

public class MapToggle : MonoBehaviour
{
    public RectTransform mapRect;
    public Vector2 onScreenPosition;
    public Vector2 offScreenPosition;
    public float transitionDuration = 0.5f;

    private bool isVisible = false;
    private Coroutine currentTransition;

    public void ToggleMap()
    {
        // if its moving onto the screen stop it
        if (currentTransition != null)
            StopCoroutine(currentTransition);

        Vector2 targetPosition = isVisible ? offScreenPosition : onScreenPosition;
        currentTransition = StartCoroutine(MoveMap(targetPosition));

        // toggle map visibility
        isVisible = !isVisible;
    }

    private IEnumerator MoveMap(Vector2 targetPosition)
    {
        Vector2 startPosition = mapRect.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            // small lerp affect for moving map on screen
            mapRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mapRect.anchoredPosition = targetPosition;
    }
}
