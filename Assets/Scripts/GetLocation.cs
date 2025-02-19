using UnityEngine;
using System.Collections;

public class GetLocation : MonoBehaviour
{
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
    }
}
