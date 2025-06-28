using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    public Rigidbody[] spikeRigidbodies;

    public GameObject[] spikeObjects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var rb in spikeRigidbodies)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var obj in spikeObjects)
            {
                obj.SetActive(false);
            }
        }
    }
}
