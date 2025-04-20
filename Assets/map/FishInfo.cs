using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FishMarker : MonoBehaviour
{
    // fish data
    public string fishName;
    public Sprite fishSprite;
    public DateTime catchDate;

    // panel objects
    public GameObject infoPanel;
    public TMP_Text fishNameText;
    public TMP_Text catchDateText;
    public Image fishImage;

    // marker
    private Button markerButton;

    void Awake()
    {
        // click button to show info
        markerButton = GetComponent<Button>();
        if (markerButton != null)
        {
            markerButton.onClick.AddListener(OnMarkerClicked);
        }
        // hidden by default
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    public void SetFishData(string name, Sprite sprite, DateTime date)
    {
        fishName = name;
        fishSprite = sprite;
        catchDate = date;
    }
    public void OnMarkerClicked()
    {
        if (infoPanel != null)
        {
            // set active
            infoPanel.SetActive(true);

            if (fishNameText != null)
            {
                fishNameText.text = fishName;
            }
            if (catchDateText != null)
            {
                catchDateText.text = catchDate.ToString("MM/dd/yyyy HH:mm");
            }
            if (fishImage != null)
            {
                fishImage.sprite = fishSprite;
            }

            // make sure its on screen so player can read it
            ForcePanelInBounds();
        }
    }

    // make sure panels on screen
    private void ForcePanelInBounds()
    {
        RectTransform panelRect = infoPanel.GetComponent<RectTransform>();
        if (panelRect == null)
            return;

        
        Vector3[] corners = new Vector3[4];
        panelRect.GetWorldCorners(corners);
        // corners
        // 0 = bottom left, 1 = top-left, 2 = top-right, 3 = bottom-right

        float offsetX = 0f;
        float offsetY = 0f;

        // chck if off left
        if (corners[0].x < 0)
            offsetX = -corners[0].x;
        // check if off bottom
        if (corners[0].y < 0)
            offsetY = -corners[0].y;
        // check if off right
        if (corners[2].x > Screen.width)
            offsetX = Screen.width - corners[2].x;
        // check if off top
        if (corners[2].y > Screen.height)
            offsetY = Screen.height - corners[2].y;

        // hi adjusts
        panelRect.position += new Vector3(offsetX, offsetY, 0f);
    }
}
