using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

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

    [Header("Map Integration")]
    // Reference to your map script that has the PlaceNewMarker function.
    public Map mapManager;
    // Reference to GetLocation script to access current latitude/longitude.
    public GetLocation getLocationScript;

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

        // Retrieve the sprite for the caught fish.
        Sprite spriteToDisplay = GetSpriteForFish(caughtFish);
        if (fishImage != null)
        {
            if (spriteToDisplay != null)
            {
                fishImage.sprite = spriteToDisplay;
                fishImage.enabled = true;
            }
            else
            {
                Debug.LogWarning("No sprite found for fish: " + caughtFish);
                fishImage.enabled = false;
            }
        }
        else
        {
            Debug.LogWarning("Fish Image reference is not set.");
        }

        // Remove the fish from the scene.
        Destroy(fish);

        // Retrieve current latitude and longitude.
        float lat, lon;
        if (getLocationScript != null)
        {
            lat = getLocationScript.currentLatitude;
            lon = getLocationScript.currentLongitude;
        }
        else
        {
            lat = Input.location.lastData.latitude;
            lon = Input.location.lastData.longitude;
        }

        // Call the map script function to place a marker using the coordinates.
        // Assumes that PlaceNewMarker returns a FishMarker component.
        if (mapManager != null)
        {
            FishMarker marker = mapManager.PlaceNewMarker(lat, lon);
            if (marker != null)
            {
                // Set marker data using the fish's name, sprite, and the current time.
                marker.SetFishData(caughtFish, spriteToDisplay, DateTime.Now);
            }
        }
        else
        {
            Debug.LogWarning("MapManager reference is not set.");
        }

        // Start coroutine to clear the UI after 4 seconds.
        StartCoroutine(ClearUI());
    }

    // Selects a random fish name based on the current water type.
    string GetRandomFishByWaterType()
    {
        string[] selectedFishList;

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

        int randomIndex = UnityEngine.Random.Range(0, selectedFishList.Length);
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
