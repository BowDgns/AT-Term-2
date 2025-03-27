using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro

public class FishingSystem : MonoBehaviour
{
    public Transform fishingBobber; // The bobber that moves with the cast
    public TextMeshProUGUI catchText; // UI to display the caught fish
    public GameObject[] fishShadows; // The fish shadows in the water

    private bool isFishing = false;
    private bool fishBiting = false;
    private string[] fishTypes = { "Bluegill", "Bass", "Carp" };
    private Vector3 originalBobberPosition;

    void Start()
    {
        originalBobberPosition = fishingBobber.position;
        catchText.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isFishing)
        {
            StartCoroutine(CastLine());
        }

        if (fishBiting && Input.GetKeyDown(KeyCode.Space))
        {
            CatchFish();
        }
    }

    IEnumerator CastLine()
    {
        isFishing = true;
        fishingBobber.position += new Vector3(0, -1, 0); // Simulate casting
        yield return new WaitForSeconds(Random.Range(2f, 5f)); // Random wait time
        fishBiting = true;
        catchText.text = "A fish is biting! Press SPACE!";
    }

    void CatchFish()
    {
        if (!fishBiting) return;

        string caughtFish = fishTypes[Random.Range(0, fishTypes.Length)];
        catchText.text = "You caught a " + caughtFish + "!";

        // Reset fishing state
        fishingBobber.position = originalBobberPosition;
        isFishing = false;
        fishBiting = false;
    }
}