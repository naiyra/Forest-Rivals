using UnityEngine;
using TMPro;

public class TrackPositionManager : MonoBehaviour
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


    public GameObject raceResultsPanel;
    public TextMeshProUGUI player1ResultsText;
    public TextMeshProUGUI player2ResultsText;

    private bool raceEnded = false;

    void Update()
    {
        UpdateRacerProgress(player1);
        UpdateRacerProgress(player2);
        UpdatePositions();

        if (!raceEnded && (player1.currentLap >= totalLaps || player2.currentLap >= totalLaps))
        {
            EndRace();
        }



    }
    void EndRace()
    {
        raceEnded = true;
        raceResultsPanel.SetActive(true);

        var car1 = player1.racer.GetComponent<CarController>();
        var car2 = player2.racer.GetComponent<CarController>();

        int p1Collectibles = car1.collectibleCount;
        int p2Collectibles = car2.collectibleCount;

        string firstPlace, secondPlace;

        // Determine 1st place based on currentLap and progress
        if (player1.currentLap > player2.currentLap ||
            (player1.currentLap == player2.currentLap && player1.progress > player2.progress))
        {
            firstPlace = "Player 1";
            secondPlace = "Player 2";
        }
        else
        {
            firstPlace = "Player 2";
            secondPlace = "Player 1";
        }

        string collectorWinner = (p1Collectibles > p2Collectibles)
            ? "Player 1 collected more"
            : (p2Collectibles > p1Collectibles)
                ? "Player 2 collected more"
                : "Both collected equally";

        player1ResultsText.text = $"Player 1\nLap: {player1.currentLap}/{totalLaps}\nCollectibles: {p1Collectibles}";
        player2ResultsText.text = $"Player 2\nLap: {player2.currentLap}/{totalLaps}\nCollectibles: {p2Collectibles}";

        Debug.Log("Race Ended! Showing results.");

        // Optionally freeze both cars
        car1.enabled = false;
        car2.enabled = false;

        // Optionally show a summary line
        Debug.Log($"🏁 First Place: {firstPlace}\n💰 {collectorWinner}");

        // You can add this info to a UI text too
        // summaryText.text = $"🏁 Winner: {firstPlace}\n💰 {collectorWinner}";
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
            if (player.currentLap < totalLaps)
            {
                player.currentLap++;
                if (player.currentLap >= totalLaps)
                    player.lapText.text = "Finished!";
                else
                    player.lapText.text = $" {player.currentLap}/{totalLaps}";

                Debug.Log($"{player.name} started Lap {player.currentLap}");
            }

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
