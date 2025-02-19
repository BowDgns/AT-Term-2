using UnityEngine;
using System.Collections;

public class GetLocation : MonoBehaviour
{
    IEnumerator Start()
    {
        // Check if the user has location services enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services not enabled");
            yield break;
        }

        // Start location services
        Input.location.Start();

        // Wait until the service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Check if location service is working
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }

        // Retrieve and log coordinates
        Debug.Log("Latitude: " + Input.location.lastData.latitude +
                  ", Longitude: " + Input.location.lastData.longitude);
    }
}
