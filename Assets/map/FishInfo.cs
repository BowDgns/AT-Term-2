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
    /// It displays the fish information on the info panel and forces the panel to stay within the screen bounds.
    /// </summary>
    public void OnMarkerClicked()
    {
        if (infoPanel != null)
        {
            // Activate the panel.
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

            // Force the info panel to be fully within the screen bounds.
            ForcePanelInBounds();
        }
    }

    /// <summary>
    /// Adjusts the info panel's position so that it remains fully on screen.
    /// </summary>
    private void ForcePanelInBounds()
    {
        RectTransform panelRect = infoPanel.GetComponent<RectTransform>();
        if (panelRect == null)
            return;

        // Get the world corners of the panel.
        Vector3[] corners = new Vector3[4];
        panelRect.GetWorldCorners(corners);
        // corners[0] = bottom-left, [1] = top-left, [2] = top-right, [3] = bottom-right

        float offsetX = 0f;
        float offsetY = 0f;

        // Check if panel is off the left side of the screen.
        if (corners[0].x < 0)
            offsetX = -corners[0].x;
        // Check if panel is off the bottom of the screen.
        if (corners[0].y < 0)
            offsetY = -corners[0].y;
        // Check if panel is off the right side of the screen.
        if (corners[2].x > Screen.width)
            offsetX = Screen.width - corners[2].x;
        // Check if panel is off the top of the screen.
        if (corners[2].y > Screen.height)
            offsetY = Screen.height - corners[2].y;

        // Apply the adjustment to the panel's position.
        panelRect.position += new Vector3(offsetX, offsetY, 0f);
    }
}
