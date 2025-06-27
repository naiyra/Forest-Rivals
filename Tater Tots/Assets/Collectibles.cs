using UnityEngine;

public class Collectibles : MonoBehaviour
{
    public GameObject collectiblePrefab; // Optional VFX (like a sparkle)

    private void OnTriggerEnter(Collider other)
    {
        CarController player = other.GetComponent<CarController>();

        if (player != null)
        {
            player.AddCollectible(); // Add 1 collectible to this player

            if (collectiblePrefab != null)
            {
                Instantiate(collectiblePrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject); // Remove the collectible
        }
    }
}
