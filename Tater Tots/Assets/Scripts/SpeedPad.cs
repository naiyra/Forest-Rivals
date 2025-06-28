using UnityEngine;

public class SpeedPad : MonoBehaviour
{
    public float boostMultiplier = 2f;
    public float boostDuration = 2f;
    public AudioClip boostSFX;

    private void OnTriggerEnter(Collider other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            car.ApplySpeedBoost(boostMultiplier, boostDuration);

            // Optional: Play boost sound
            if (boostSFX != null)
            {
                AudioSource.PlayClipAtPoint(boostSFX, transform.position);
            }
        }
    }
}
