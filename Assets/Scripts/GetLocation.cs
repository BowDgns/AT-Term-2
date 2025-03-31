using UnityEngine;
using System.Collections;
using System.IO;
using TMPro;

[System.Serializable]
public class SpawnMapping // to make the locations in the editor
{
    public string waterType;
    public string areaType;
    public Transform spawnPoint;
}

public class GetLocation : MonoBehaviour
{
    public Transform player;                // Assign your player Transform in the Inspector
    public SpawnMapping[] spawnMappings;    // Assign mappings in the Inspector
    public FishCatcher fishCatcher;         // Reference to the FishCatcher script
    float radius_threshold = 0.001f;         // Threshold for being "near" a water point

    public TMP_Text area_name_text;
    public TMP_Text location_warning_text;

    IEnumerator Start()
    {
        int maxWait = 20;
        bool isUnityRemote = true; // allowing time for Unity Remote to connect to the device

        if (isUnityRemote)
        {
            yield return new WaitForSeconds(5);
        }

        // Check if location is enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location not enabled");
            location_warning_text.text = "Location not enabled.";
            yield break;
        }

        Input.location.Start();

        if (isUnityRemote)
        {
            yield return new WaitForSeconds(5);
        }
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Check if location service is working
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to get location");
            location_warning_text.text = "Can't get location.";
            yield break;
        }

        Debug.Log("Latitude: " + Input.location.lastData.latitude +
                  ", Longitude: " + Input.location.lastData.longitude);

        nearWater(Input.location.lastData.latitude, Input.location.lastData.longitude);
    }

    void nearWater(float latitude, float longitude)
    {
        string path = Application.streamingAssetsPath + "/CityWater.csv";

        // Read through coordinates in a CSV file and compare them to device coordinates
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            // Assuming the first line is a header
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] parts = line.Split(',');
                // Expected order: latitude, longitude, water body, water type, area type
                double water_lat = double.Parse(parts[0]);
                double water_lon = double.Parse(parts[1]);
                string water_body = parts[2];
                string area_type = parts[3];

                if (checkRadius(latitude, longitude, water_lat, water_lon))
                {
                    Debug.Log("water type: " + water_body + ", area: " + area_type);
                    PlacePlayerAtSpawn(water_body, area_type);
                    return;
                }
            }
        }
        Debug.Log("Not near water");
        location_warning_text.text = "Not near water, to play go near any water body!";
    }

    // Simple check using Euclidean distance (for small distances)
    bool checkRadius(double lat1, double lon1, double lat2, double lon2)
    {
        double distance = Mathf.Sqrt(Mathf.Pow((float)(lat1 - lat2), 2) + Mathf.Pow((float)(lon1 - lon2), 2));
        return distance < radius_threshold;
    }

    void PlacePlayerAtSpawn(string waterType, string areaType)
    {
        foreach (SpawnMapping mapping in spawnMappings)
        {
            if (mapping.waterType == waterType && mapping.areaType == areaType)
            {
                Debug.Log("going to: " + mapping.spawnPoint.name);
                if (player != null)
                {
                    player.transform.position = mapping.spawnPoint.position;
                    area_name_text.text = "Area: " + mapping.areaType + ", " + mapping.waterType;
                }
                // Update the fish catcher with the current water type
                if (fishCatcher != null)
                {
                    fishCatcher.currentWaterType = waterType;
                }
                return;
            }
        }
        Debug.Log("No spawn mapping found for Water Type: " + waterType + " and Area Type: " + areaType);
    }
}
