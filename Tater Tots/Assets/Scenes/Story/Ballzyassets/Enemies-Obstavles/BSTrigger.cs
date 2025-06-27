using UnityEngine;
using UnityEngine.Playables;

public class BSTrigger : MonoBehaviour
{
    public PlayableDirector timeline;

    bool hit = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hit && other.CompareTag("Player"))
        {
            hit = true;
            Invoke("PlayAnimationWithDelay", 3f);
        }
    }

    void PlayAnimationWithDelay()
    {
        if (timeline != null) 
        {
            timeline.Play();
        }
        else 
        {
            Debug.LogError("No timeline assigned!");
        }
    }
}
