using UnityEngine;
using TMPro;

public class TrackManager : MonoBehaviour
{
    [System.Serializable]
    public class RacerData
    {
        public string name;
        public Transform racer;
        public int currentLap = 0;
        public int lastPassedWaypoint = -1;
        public float progress = 0f;
        public TextMeshProUGUI lapText;
        public TextMeshProUGUI positionText;

        [HideInInspector] public bool canTriggerLap = true;
    }

    public Transform[] trackWaypoints;
    public RacerData player1;
    public RacerData player2;

    public int totalLaps = 3;
    public int startWaypointIndex = 0;
    public float lapTriggerDistance = 10f;

    void Update()
    {
        UpdateRacerProgress(player1);
        UpdateRacerProgress(player2);
        UpdatePositions();
    }

    void UpdateRacerProgress(RacerData player)
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < trackWaypoints.Length; i++)
        {
            float dist = Vector3.Distance(player.racer.position, trackWaypoints[i].position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;
            }
        }

        Vector3 current = trackWaypoints[closestIndex].position;
        Vector3 next = trackWaypoints[(closestIndex + 1) % trackWaypoints.Length].position;
        float segmentDistance = Vector3.Distance(current, next);
        float subProgress = 1f - (closestDistance / segmentDistance);
        player.progress = closestIndex + subProgress;

        bool isAtStart = (closestIndex == startWaypointIndex && closestDistance <= lapTriggerDistance);

        if (isAtStart && player.canTriggerLap)
        {
            player.currentLap++;
            player.lapText.text = $"Lap: {player.currentLap}/{totalLaps}";
            Debug.Log($"{player.name} started Lap {player.currentLap}");
            player.canTriggerLap = false;
        }
        else if (!isAtStart)
        {
            player.canTriggerLap = true;
        }

        player.lastPassedWaypoint = closestIndex;
    }

    void UpdatePositions()
    {
        // Player 1 ahead if they are on a higher lap
        if (player1.currentLap > player2.currentLap)
        {
            player1.positionText.text = "1st";
            player2.positionText.text = "2nd";
        }
        // Player 2 ahead if they are on a higher lap
        else if (player2.currentLap > player1.currentLap)
        {
            player1.positionText.text = "2nd";
            player2.positionText.text = "1st";
        }
        // Same lap, compare track progress
        else
        {
            if (player1.progress > player2.progress)
            {
                player1.positionText.text = "1st";
                player2.positionText.text = "2nd";
            }
            else
            {
                player1.positionText.text = "2nd";
                player2.positionText.text = "1st";
            }
        }
    }

}
