using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FishMarker : MonoBehaviour
{
    // Data about the fish catch.
    public string fishName;
    public Sprite fishSprite;
    public DateTime catchDate;

    [Header("Child Info Panel (within the prefab)")]
    // These should be assigned in the prefab by dragging the appropriate child objects.
    public GameObject infoPanel;
    public TMP_Text fishNameText;
    public TMP_Text catchDateText;
    public Image fishImage;

    private Button markerButton;

    void Awake()
    {
        // Get the Button component and add a click listener.
        markerButton = GetComponent<Button>();
        if (markerButton != null)
        {
            markerButton.onClick.AddListener(OnMarkerClicked);
        }
        // Ensure the info panel is hidden by default.
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Call this method from your fishing system immediately after instantiating the marker.
    /// </summary>
    /// <param name="name">Name of the caught fish.</param>
    /// <param name="sprite">Sprite representing the fish.</param>
    /// <param name="date">Date and time of the catch.</param>
    public void SetFishData(string name, Sprite sprite, DateTime date)
    {
        fishName = name;
        fishSprite = sprite;
        catchDate = date;
    }

    /// <summary>
    /// Called when the marker (button) is clicked.
    /// It displays the fish information on the info panel.
    /// </summary>
    public void OnMarkerClicked()
    {
        if (infoPanel != null)
        {
            // Toggle the panel on or off as needed. Here we simply activate it.
            infoPanel.SetActive(true);

            // Update the panel UI elements with the stored data.
            if (fishNameText != null)
            {
                fishNameText.text = fishName;
            }
            if (catchDateText != null)
            {
                // Format the catch date as desired.
                catchDateText.text = catchDate.ToString("MM/dd/yyyy HH:mm");
            }
            if (fishImage != null)
            {
                fishImage.sprite = fishSprite;
            }
        }
    }
}
