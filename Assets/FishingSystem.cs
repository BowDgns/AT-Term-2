using UnityEngine;
using TMPro;

public class FishCatcher : MonoBehaviour
{
    public float catchRange = 0.5f;
    public TMP_Text fishNameText;

    // Current water type that gets updated from GetLocation
    [HideInInspector]
    public string currentWaterType = "Default";

    // Fish lists for different water types
    public string[] pondFish = new string[] { "Goldfish", "Koi" };
    public string[] riverFish = new string[] { "Bass", "Trout" };
    public string[] defaultFish = new string[] { "Catfish", "Carp" };

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
        string caughtFish = GetRandomFishByWaterType();

        // Display the fish name using TextMeshPro.
        if (fishNameText != null)
        {
            fishNameText.text = "Caught a " + caughtFish + "!";
        }
        else
        {
            Debug.LogWarning("Fish Name TMP_Text reference is not set.");
        }

        // Remove the fish from the scene.
        Destroy(fish);
    }

    // Selects a random fish name based on the current water type
    string GetRandomFishByWaterType()
    {
        string[] selectedFishList;

        // Choose the fish list based on the water type
        if (currentWaterType.ToLower().Contains("pond"))
        {
            selectedFishList = pondFish;
        }
        else if (currentWaterType.ToLower().Contains("river"))
        {
            selectedFishList = riverFish;
        }
        else
        {
            selectedFishList = defaultFish;
        }

        int randomIndex = Random.Range(0, selectedFishList.Length);
        return selectedFishList[randomIndex];
    }
}
