using UnityEngine;
using System.Collections;

public class Collectibles : MonoBehaviour
{
    [SerializeField] private GameObject collectibleVFX;
    [SerializeField] private float respawnDelay = 10f;

    private AudioSource audioSource;
    private Collider col;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (audioSource == null)
            Debug.LogError("Collectible missing AudioSource!");
        if (col == null)
            Debug.LogError("Collectible missing Collider!");
        if (meshRenderer == null)
            Debug.LogWarning("Collectible missing MeshRenderer!");
    }

    private void OnTriggerEnter(Collider other)
    {
        CarController player = other.GetComponent<CarController>();
        if (player == null) return;

        if (audioSource != null)
            audioSource.Play();

        player.AddCollectible();

        if (collectibleVFX != null)
            Instantiate(collectibleVFX, transform.position, Quaternion.identity);

        // Hide visuals and collider only, not the GameObject itself
        StartCoroutine(HandleRespawn());
    }

    private IEnumerator HandleRespawn()
    {
        col.enabled = false;
        if (meshRenderer != null)
            meshRenderer.enabled = false;

        float delay = audioSource != null && audioSource.clip != null ? audioSource.clip.length : 0f;
        yield return new WaitForSeconds(delay);

        // Optional: wait a bit more before respawning
        yield return new WaitForSeconds(respawnDelay);

        // Reactivate
        if (meshRenderer != null)
            meshRenderer.enabled = true;
        col.enabled = true;
    }
}
