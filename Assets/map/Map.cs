using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public RectTransform mapImage;       // map

    public float mapLatTop;              // latitude top left
    public float mapLonLeft;             // longitude top left
    public float mapLatBottom;           // latitude bottom right
    public float mapLonRight;            // latitude bottom right

    public Button markerButtonPrefab;    // marker button

    // place button on map at coordinates
    public FishMarker PlaceNewMarker(float deviceLat, float deviceLon)
    {
        // normalize logitude 
        float normalizedX = (deviceLon - mapLonLeft) / (mapLonRight - mapLonLeft);
        // normalize latitude
        float normalizedY = (deviceLat - mapLatBottom) / (mapLatTop - mapLatBottom);

        // convert to map width #


        float posX = normalizedX * mapImage.rect.width;
        float posY = normalizedY * mapImage.rect.height;

        // ensure its at the correct place despite map pivot
        posX = posX - (mapImage.rect.width * mapImage.pivot.x);
        posY = posY - (mapImage.rect.height * mapImage.pivot.y);

        // add marker as child of the map
        Button newMarker = Instantiate(markerButtonPrefab, mapImage);
        RectTransform markerRect = newMarker.GetComponent<RectTransform>();
        markerRect.anchoredPosition = new Vector2(posX, posY);

        return newMarker.GetComponent<FishMarker>();
    }
}
