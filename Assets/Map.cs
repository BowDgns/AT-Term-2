using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public RectTransform mapImage;       // The UI Image's RectTransform
    public float mapLatTop;              // Latitude at the top left corner
    public float mapLonLeft;             // Longitude at the top left corner
    public float mapLatBottom;           // Latitude at the bottom right corner
    public float mapLonRight;            // Longitude at the bottom right corner
    public Button markerButtonPrefab;    // Your marker (UI Button) prefab; should have a FishMarker component

    // Places a new marker on the map based on device coordinates and returns its FishMarker component.
    public FishMarker PlaceNewMarker(float deviceLat, float deviceLon)
    {
        // Convert device longitude into a normalized value (0 = left, 1 = right).
        float normalizedX = (deviceLon - mapLonLeft) / (mapLonRight - mapLonLeft);
        // Convert device latitude into a normalized value (0 = bottom, 1 = top).
        float normalizedY = (deviceLat - mapLatBottom) / (mapLatTop - mapLatBottom);

        // Convert normalized values into pixel positions relative to the map's size.
        float posX = normalizedX * mapImage.rect.width;
        float posY = normalizedY * mapImage.rect.height;

        // Adjust for the RectTransform pivot.
        posX = posX - (mapImage.rect.width * mapImage.pivot.x);
        posY = posY - (mapImage.rect.height * mapImage.pivot.y);

        // Instantiate the marker as a child of the map image.
        Button newMarker = Instantiate(markerButtonPrefab, mapImage);
        RectTransform markerRect = newMarker.GetComponent<RectTransform>();
        markerRect.anchoredPosition = new Vector2(posX, posY);

        // Return the FishMarker component on the new marker (if it exists).
        return newMarker.GetComponent<FishMarker>();
    }
}
