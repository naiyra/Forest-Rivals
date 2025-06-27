using UnityEngine;

public class ExplosionTrigger : MonoBehaviour
{
    public GameObject explosionPrefab; // Assign your VFX prefab in the Inspector

    private void OnTriggerEnter(Collider other)

    {


        CarController player = other.GetComponent<CarController>();


        if (other.CompareTag("Player"))
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            player.ApplyTNTStun();
            Destroy(gameObject);
        }
    }

}
