using UnityEngine;

public class ExplosionTrigger : MonoBehaviour
{
    public GameObject explosionPrefab;             // VFX prefab
    public AudioSource explosionAudioSourcePrefab; // Prefab with AudioSource + Explosion SFX

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

        // Stun effect
        player.ApplyTNTStun();

        // Destroy TNT immediately
        Destroy(gameObject);
    }
}
