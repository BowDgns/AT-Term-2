using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections; // Required for IEnumerator and WaitForSeconds

public class FishCatcher : MonoBehaviour
{
    public float catchRange = 0.5f;
    public TMP_Text fishNameText;
    public Image fishImage; // UI Image to display the fish sprite

    // Current water type that gets updated from GetLocation
    [HideInInspector]
    public string currentWaterType = "other";

    // Fish lists for different water types
    public string[] pondFish = new string[] { "Goldfish", "Koi" };
    public string[] riverFish = new string[] { "Bass", "Trout" };
    public string[] seaFish = new string[] { "Mackerel", "Sea Bass" };
    public string[] otherFish = new string[] { "Catfish", "Carp" };

    // Mapping of fish names to their sprites.
    [System.Serializable]
    public class FishSpriteMapping
    {
        public string fishName;
        public Sprite fishSprite;
    }
    public FishSpriteMapping[] fishSprites;

    // Initialize UI elements to be empty/invisible.
    void Start()
    {
        if (fishNameText != null)
        {
            fishNameText.text = "";
        }
        if (fishImage != null)
        {
            fishImage.enabled = false;
        }
    }

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

        // Set the fish image based on the caught fish.
        if (fishImage != null)
        {
            Sprite spriteToDisplay = GetSpriteForFish(caughtFish);
            if (spriteToDisplay != null)
            {
                fishImage.sprite = spriteToDisplay;
                fishImage.enabled = true; // Ensure the image is visible.
            }
            else
            {
                Debug.LogWarning("No sprite found for fish: " + caughtFish);
                fishImage.enabled = false; // Optionally hide the image if not found.
            }
        }
        else
        {
            Debug.LogWarning("Fish Image reference is not set.");
        }

        // Remove the fish from the scene.
        Destroy(fish);

        // Start coroutine to clear the UI after 4 seconds.
        StartCoroutine(ClearUI());
    }

    // Selects a random fish name based on the current water type.
    string GetRandomFishByWaterType()
    {
        string[] selectedFishList;

        // Choose the fish list based on the water type.
        if (currentWaterType.ToLower().Contains("pond"))
        {
            selectedFishList = pondFish;
        }
        else if (currentWaterType.ToLower().Contains("river"))
        {
            selectedFishList = riverFish;
        }
        else if (currentWaterType.ToLower().Contains("sea"))
        {
            selectedFishList = seaFish;
        }
        else
        {
            selectedFishList = otherFish;
        }

        int randomIndex = Random.Range(0, selectedFishList.Length);
        return selectedFishList[randomIndex];
    }

    // Retrieves the sprite associated with the given fish name.
    Sprite GetSpriteForFish(string fishName)
    {
        foreach (FishSpriteMapping mapping in fishSprites)
        {
            if (mapping.fishName.Equals(fishName))
            {
                return mapping.fishSprite;
            }
        }
        return null;
    }

    // Coroutine to clear the UI elements after 4 seconds.
    IEnumerator ClearUI()
    {
        yield return new WaitForSeconds(4f);
        if (fishNameText != null)
        {
            fishNameText.text = "";
        }
        if (fishImage != null)
        {
            fishImage.enabled = false;
        }
    }
}
