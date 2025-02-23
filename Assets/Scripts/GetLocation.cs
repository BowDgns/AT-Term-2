using UnityEngine;
using System.Collections;
using System.IO;

public class GetLocation : MonoBehaviour
{
    float radius_threashold = 0.001f;
    IEnumerator Start()
    {
        int maxWait = 20;
        bool isUnityRemote = true;
       

        if (isUnityRemote)  // allowing time for unity remote to connect to the device
        {
            yield return new WaitForSeconds(5);
        }

        // check if location is enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("location not enabled");
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

        // check location is working
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("unable to get location");
            yield break;
        }

        // return lagitute and longitude
        Debug.Log("latitude: " + Input.location.lastData.latitude +
                  ", longitude: " + Input.location.lastData.longitude);

        nearWater(Input.location.lastData.latitude, Input.location.lastData.longitude);
    }

    void nearWater(float latitude, float longitude)
    {
        string path = Application.streamingAssetsPath + "/CityWater.csv";

        // read through coordinates in a csv file and compare them to device coordinates
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] parts = line.Split(',');
                double water_lat = double.Parse(parts[0]);
                double water_lon = double.Parse(parts[1]);
                string body = parts[2];

                if (checkRadius(latitude, longitude, water_lat, water_lon))
                {
                    Debug.Log("near a: " + body);
                    return;
                }
            }
        }
        Debug.Log("not near water");
    }

    // hi comment
    bool checkRadius(double lat1, double lon1, double lat2, double lon2)
    {
        double distance = Mathf.Sqrt(Mathf.Pow((float)(lat1 - lat2), 2) + Mathf.Pow((float)(lon1 - lon2), 2));
        return distance < radius_threashold;
    }
}
