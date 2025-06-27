using UnityEngine;

public class Collectibles : MonoBehaviour
{
    [SerializeField] private GameObject collectibleVFX; // Optional visual effect
    private AudioSource audioSource;
    private bool hasBeenCollected = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Collectible missing AudioSource!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenCollected) return; // Prevent double trigger
        CarController player = other.GetComponent<CarController>();
        if (player == null) return;

        hasBeenCollected = true;

        // Play audio from this collectible's AudioSource
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Call player method
        player.AddCollectible();

        // Spawn visual effect
        if (collectibleVFX != null)
        {
            Instantiate(collectibleVFX, transform.position, Quaternion.identity);
        }

        // Disable the collider and visuals immediately
        GetComponent<Collider>().enabled = false;
        if (GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().enabled = false;

        // Destroy after sound finishes
        float destroyDelay = audioSource != null ? audioSource.clip.length : 0f;
        Destroy(gameObject, destroyDelay);
    }
}
