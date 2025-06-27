using UnityEngine;

public class EngineSoundController : MonoBehaviour
{
    public Rigidbody carRb;

    [Header("Audio Sources")]
    public AudioSource idleSource;
    public AudioSource forwardSource;
    public AudioSource reverseSource;
    public AudioSource startSource;

    [Header("Pitch Settings")]
    public float topSpeed = 50f;
    public float pitchMin = 1f;
    public float pitchMax = 2f;

    [Header("Blending Speeds")]
    public float blendInSpeed = 2f;
    public float blendOutSpeed = 1f;

    [Header("Start Engine Settings")]
    public float startSoundDelay = 1.5f;
    private bool hasPlayedStartSound = false;
    private bool hasStartedEngine = false;
    private float startTimer = 0f;

    [Header("Reverse Logic")]
    public float reverseVelocityThreshold = 1f;

    [Header("Volume Multipliers")]
    [Range(0f, 1f)] public float idleVolume = 1f;
    [Range(0f, 1f)] public float forwardVolume = 1f;
    [Range(0f, 1f)] public float reverseVolume = 1f;

    void Start()
    {
        // Ensure all sources are looping (except start)
        idleSource.loop = true;
        forwardSource.loop = true;
        reverseSource.loop = true;
        startSource.loop = false;

        // Start all looped sources silent
        idleSource.volume = 0f;
        forwardSource.volume = 0f;
        reverseSource.volume = 0f;

        idleSource.Play();
        forwardSource.Play();
        reverseSource.Play();
    }

    void Update()
    {
        float input = Input.GetAxis("Vertical");
        float speed = carRb.velocity.magnitude;

        // Get local velocity to check reverse movement
        Vector3 localVelocity = transform.InverseTransformDirection(carRb.velocity);
        bool isReversing = localVelocity.z < -reverseVelocityThreshold;

        // Update pitch
        float pitch = Mathf.Lerp(pitchMin, pitchMax, speed / topSpeed);
        forwardSource.pitch = pitch;
        reverseSource.pitch = pitch;
        idleSource.pitch = 1f;

        // Engine Start Trigger
        if (!hasPlayedStartSound && input > 0.1f)
        {
            startSource.PlayOneShot(startSource.clip);
            hasPlayedStartSound = true;
            startTimer = 0f;
            return;
        }

        if (hasPlayedStartSound && !hasStartedEngine)
        {
            startTimer += Time.deltaTime;
            if (startTimer >= startSoundDelay)
            {
                hasStartedEngine = true;
                idleSource.volume = idleVolume; // Start with idle audible
            }
            return;
        }

        if (!hasStartedEngine) return;

        // Audio State Logic
        if (localVelocity.z > 0.5f) // Moving forward
        {
            BlendTo(forwardSource, 1f, blendInSpeed, forwardVolume);
            BlendTo(reverseSource, 0f, blendOutSpeed, reverseVolume);
            BlendTo(idleSource, 0f, blendOutSpeed, idleVolume);
        }
        else if (isReversing) // Moving backward
        {
            BlendTo(reverseSource, 1f, blendInSpeed, reverseVolume);
            BlendTo(forwardSource, 0f, blendOutSpeed, forwardVolume);
            BlendTo(idleSource, 0f, blendOutSpeed, idleVolume);
        }
        else // Idle
        {
            BlendTo(idleSource, 1f, blendInSpeed * 0.5f, idleVolume);
            BlendTo(forwardSource, 0f, blendOutSpeed, forwardVolume);
            BlendTo(reverseSource, 0f, blendOutSpeed, reverseVolume);
        }
    }

    void BlendTo(AudioSource source, float targetBlend, float speed, float baseVolume)
    {
        float target = targetBlend * baseVolume;
        source.volume = Mathf.MoveTowards(source.volume, target, Time.deltaTime * speed);
    }
}
