using UnityEngine;
using System.Collections;

public class ExplosionTrigger : MonoBehaviour
{
    public GameObject explosionPrefab;             // VFX prefab
    public AudioSource explosionAudioSourcePrefab; // Prefab with AudioSource + Explosion SFX
    [SerializeField] private float respawnDelay = 10f;

    private Collider col;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        col = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (col == null)
            Debug.LogError("Missing Collider on TNT box!");
        if (meshRenderer == null)
            Debug.LogWarning("Missing MeshRenderer on TNT box!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CarController player = other.GetComponent<CarController>();
        if (player == null) return;

        // Play VFX
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Play SFX via temporary clone
        if (explosionAudioSourcePrefab != null)
        {
            AudioSource audioClone = Instantiate(explosionAudioSourcePrefab, transform.position, Quaternion.identity);
            audioClone.Play();
            Destroy(audioClone.gameObject, audioClone.clip.length);
        }

        // Apply TNT effect
        player.ApplyTNTStun();

        // Hide and start respawn
        StartCoroutine(HandleRespawn());
    }

    private IEnumerator HandleRespawn()
    {
        col.enabled = false;
        if (meshRenderer != null)
            meshRenderer.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        if (meshRenderer != null)
            meshRenderer.enabled = true;
        col.enabled = true;
    }
}
