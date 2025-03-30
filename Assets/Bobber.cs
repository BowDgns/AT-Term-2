using UnityEngine;
using System.Collections;

public class BobberController : MonoBehaviour
{
    [Header("Animation Settings")]
    public float throwDuration = 1f;    // Duration for throw animation.
    public float reelDuration = 1f;     // Duration for reel-in animation.
    public float arcHeight = 2f;        // Maximum extra height for the arc.

    [HideInInspector]
    public bool isAnimating = false;    // Tracks if the bobber is currently animating.

    // Animates from startPos to targetPos along a parabolic arc.
    public IEnumerator ThrowBobber(Vector3 startPos, Vector3 targetPos)
    {
        isAnimating = true;
        // Override the passed startPos: use the camera's position + 5.56 on the Y-axis.
        startPos = Camera.main.transform.position + new Vector3(0, 5.56f, 0);
        float elapsed = 0f;
        while (elapsed < throwDuration)
        {
            float t = elapsed / throwDuration;
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += arcHeight * 4 * t * (1 - t); // Parabolic vertical offset.
            transform.position = currentPos;
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Ensure the bobber is exactly at the target position.
        transform.position = targetPos;

        // Lock the bobber in place by disabling physics if a Rigidbody exists.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        isAnimating = false;
    }

    // Animates from the current position back to the fixed reelTarget.
    public IEnumerator ReelBobber(Vector3 reelTarget)
    {
        isAnimating = true;
        // Re-enable physics for a smooth reel.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Vector3 startPos = transform.position;
        float elapsed = 0f;
        while (elapsed < reelDuration)
        {
            float t = elapsed / reelDuration;
            Vector3 currentPos = Vector3.Lerp(startPos, reelTarget, t);
            // Optionally, add a smaller arc during reeling.
            currentPos.y += (arcHeight * 0.5f) * 4 * t * (1 - t);
            transform.position = currentPos;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = reelTarget;

        // Lock the bobber once it reaches the reel target.
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        isAnimating = false;
    }
}
