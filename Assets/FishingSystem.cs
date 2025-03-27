using UnityEngine;
using TMPro;

public class FishCatcher : MonoBehaviour
{
    public float catchRange = 0.5f;      // Range within which a fish can be caught relative to the bobber
    public TMP_Text fishNameText;        // TextMeshPro text component to display the fish name
    public string[] fishNames = new string[] { "Goldfish", "Bass", "Trout" };

    // Called by PlayerController when a bobber exists and the player taps.
    public void TryCatchFishAtBobber(Transform bobberTransform)
    {
        if (bobberTransform == null)
        {
            Debug.LogWarning("Bobber transform is null!");
            return;
        }

        // Find all fish in the scene with the "Fish" tag.
        GameObject[] fishObjects = GameObject.FindGameObjectsWithTag("Fish");
        foreach (GameObject fish in fishObjects)
        {
            float distance = Vector3.Distance(bobberTransform.position, fish.transform.position);
            if (distance <= catchRange)
            {
                CatchFish(fish);
                break; // Only catch one fish per tap.
            }
        }
    }

    void CatchFish(GameObject fish)
    {
        // Select a random fish name from the array.
        int randomIndex = Random.Range(0, fishNames.Length);
        string randomFishName = fishNames[randomIndex];

        // Display the fish name using TextMeshPro.
        if (fishNameText != null)
        {
            fishNameText.text = "Caught a " + randomFishName + "!";
        }
        else
        {
            Debug.LogWarning("Fish Name TMP_Text reference is not set.");
        }

        // Remove the fish from the scene.
        Destroy(fish);
    }
}
