using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NitroBoost : MonoBehaviour
{
    [Header("Nitro Settings")]
    public float boostMultiplier = 2f;
    public float boostDuration = 2f;
    public float boostCooldown = 5f;

    [Header("Player Input")]
    public bool isPlayer1 = true;

    [Header("VFX & SFX")]
    public GameObject[] nitroVFXObjects; // Supports multiple exhausts
    public AudioSource nitroAudioSource;

    [Header("UI")]
    public Slider nitroSlider;

    private CarController carController;
    private bool isBoosting = false;
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;

    void Start()
    {
        carController = GetComponent<CarController>();
        if (carController == null)
            Debug.LogError("NitroBoost: Missing CarController!");

        // Ensure all VFX are off at start
        foreach (GameObject vfx in nitroVFXObjects)
        {
            if (vfx != null) vfx.SetActive(false);
        }

        if (nitroSlider != null)
            nitroSlider.value = 1f;
    }

    void Update()
    {
        if (InputTriggered() && !isBoosting && !isOnCooldown)
        {
            StartCoroutine(ActivateNitro());
        }

        // Refill slider only while on cooldown
        if (isOnCooldown && nitroSlider != null)
        {
            cooldownTimer += Time.deltaTime;
            nitroSlider.value = Mathf.Clamp01(cooldownTimer / boostCooldown);

            // Stop cooldown logic once filled
            if (cooldownTimer >= boostCooldown)
            {
                isOnCooldown = false;
                cooldownTimer = 0f;
                nitroSlider.value = 1f; // Ensure it's full
            }
        }
    }

    private bool InputTriggered()
    {
        return (isPlayer1 && Input.GetKeyDown(KeyCode.LeftControl)) ||
               (!isPlayer1 && Input.GetKeyDown(KeyCode.RightControl));
    }

    private IEnumerator ActivateNitro()
    {
        isBoosting = true;
        isOnCooldown = false;
        cooldownTimer = 0f;

        if (nitroSlider != null)
            nitroSlider.value = 0f;

        // Activate VFX
        foreach (GameObject vfx in nitroVFXObjects)
            if (vfx != null) vfx.SetActive(true);

        // Play SFX
        if (nitroAudioSource != null)
            nitroAudioSource.Play();

        // Apply boost
        float originalAcceleration = carController.maxAcceleration;
        carController.maxAcceleration = originalAcceleration * boostMultiplier;

        float timer = 0f;
        while (timer < boostDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Revert boost
        carController.maxAcceleration = originalAcceleration;

        // Deactivate VFX
        foreach (GameObject vfx in nitroVFXObjects)
            if (vfx != null) vfx.SetActive(false);

        isBoosting = false;
        isOnCooldown = true;
        cooldownTimer = 0f;
    }




}
