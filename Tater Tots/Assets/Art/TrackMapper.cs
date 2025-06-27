using UnityEngine;
using UnityEngine.UI;

public class TrackMapper : MonoBehaviour
{
    public Transform[] trackPoints; // The same waypoints from the race
    public RectTransform mapRect; // The RawImage rect of your TrackMap
    public Transform player1;
    public Transform player2;
    public RectTransform player1Icon;
    public RectTransform player2Icon;

    public Vector2 mapSize = new Vector2(300, 300); // must match the image size in UI
    public Vector2 worldMin; // bottom-left of the track in world space
    public Vector2 worldMax; // top-right of the track in world space

    void Start()
    {
        mapSize = mapRect.sizeDelta;

        worldMin = new Vector2(float.MaxValue, float.MaxValue);
        worldMax = new Vector2(float.MinValue, float.MinValue);

        foreach (var point in trackPoints)
        {
            Vector3 pos = point.position;
            worldMin.x = Mathf.Min(worldMin.x, pos.x);
            worldMin.y = Mathf.Min(worldMin.y, pos.z);
            worldMax.x = Mathf.Max(worldMax.x, pos.x);
            worldMax.y = Mathf.Max(worldMax.y, pos.z);
        }
    }


    void Update()
    {
        UpdatePlayerIcon(player1, player1Icon);
        UpdatePlayerIcon(player2, player2Icon);
    }

    void UpdatePlayerIcon(Transform player, RectTransform icon)
    {
        Vector3 pos = player.position;

        float xNorm = Mathf.InverseLerp(worldMin.x, worldMax.x, pos.x);
        float zNorm = Mathf.InverseLerp(worldMin.y, worldMax.y, pos.z);

        // Get corners of the map image in local space
        Vector3[] corners = new Vector3[4];
        mapRect.GetLocalCorners(corners);

        // corners[0] = bottom-left, corners[2] = top-right
        float mapWidth = corners[2].x - corners[0].x;
        float mapHeight = corners[2].y - corners[0].y;

        float xLocal = corners[0].x + xNorm * mapWidth;
        float yLocal = corners[0].y + zNorm * mapHeight;

        icon.localPosition = new Vector2(xLocal, yLocal);
    }




}
