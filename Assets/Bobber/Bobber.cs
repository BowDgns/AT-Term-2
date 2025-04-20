using UnityEngine;
using System.Collections;

public class BobberController : MonoBehaviour
{
    public float throwDuration = 1f;   
    public float reelDuration = 1f;
    public float arcHeight = 2f;
    public bool isAnimating = false;

    // animate bobber along an arc (show throwing) 
    public IEnumerator ThrowBobber(Vector3 startPos, Vector3 targetPos)
    {
        isAnimating = true;
        startPos = Camera.main.transform.position + new Vector3(0, 5.56f, 0);
        float elapsed = 0f;
        while (elapsed < throwDuration)
        {
            float t = elapsed / throwDuration;
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += arcHeight * 4 * t * (1 - t); 
            transform.position = currentPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        // keep bobber in place once "in water"
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        isAnimating = false;
    }

    // animate reeling
    public IEnumerator ReelBobber(Vector3 reelTarget)
    {
        isAnimating = true;
        // leaving water so enable physics
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
            currentPos.y += (arcHeight * 0.5f) * 4 * t * (1 - t);
            transform.position = currentPos;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = reelTarget;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        isAnimating = false;
    }
}
