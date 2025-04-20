using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class FishCatcher : MonoBehaviour
{
    public float catchRange = 0.5f;
    public TMP_Text fishNameText;
    public Image fishImage;

    // location based on "getlocation"
    public string currentWaterType = "other";

    // fish lists for different water types
    public string[] pondFish = new string[] { "Goldfish", "Koi" };
    public string[] riverFish = new string[] { "Bass", "Trout" };
    public string[] seaFish = new string[] { "Mackerel", "Sea Bass" };
    public string[] otherFish = new string[] { "Catfish", "Carp" };

    // mapping of fish names to their sprites.
    [System.Serializable]
    public class FishSpriteMapping
    {
        public string fishName;
        public Sprite fishSprite;
    }
    public FishSpriteMapping[] fishSprites;


    public Map mapManager;

    // get location coordinates
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

    public void TryCatchFishAtBobber(Transform bobberTransform)
    {
        if (bobberTransform == null)
        {
            Debug.LogWarning("Bobber transform is null!");
            return;
        }

        // find fish in scene to ensure its a catchable object
        GameObject[] fishObjects = GameObject.FindGameObjectsWithTag("Fish");
        foreach (GameObject fish in fishObjects)
        {
            float distance = Vector3.Distance(bobberTransform.position, fish.transform.position);
            if (distance <= catchRange)
            {
                CatchFish(fish);
                break; // only catch one fish per catch
            }
        }
    }

    void CatchFish(GameObject fish)
    {
        string caughtFish = GetRandomFishByWaterType();

        if (fishNameText != null)
        {
            fishNameText.text = "Caught a " + caughtFish + "!";
        }
        else
        {
            Debug.LogWarning("Fish Name TMP_Text reference is not set.");
        }

        // get sprite for fish
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

        // remove fish shadow
        Destroy(fish);

        // get lat long for mapping
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

        if (mapManager != null)
        {
            FishMarker marker = mapManager.PlaceNewMarker(lat, lon);
            if (marker != null)
            {
                marker.SetFishData(caughtFish, spriteToDisplay, DateTime.Now);
            }
        }
        else
        {
            Debug.LogWarning("MapManager reference is not set.");
        }

        StartCoroutine(ClearUI());
    }

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

    // get sprite from name
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

    // clearing ui crountine
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
