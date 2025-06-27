using UnityEngine;
using TMPro;

public class CollectibleUIManager : MonoBehaviour
{
    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;

    public CarController player1;
    public CarController player2;

    void Update()
    {
        if (player1 != null)
            player1Text.text = "P1: " + player1.collectibleCount;

        if (player2 != null)
            player2Text.text = "P2: " + player2.collectibleCount;
    }
}
