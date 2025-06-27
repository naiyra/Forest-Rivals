using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public Transform player1; // First player
    public Transform player2; // Second player

    public GameObject bulletPrefab;
    public GameObject shootingVFXPrefab;
    public float bulletSpeed = 10f;
    public float shootInterval = 1f;
    public float detectionRange = 10f;
    public GameObject shootingPoint;
    public Animator animator;
    public float liftAmount = 2f;
    public Collider stopShootingCollider;
    public int maxBullets = 10;

    private int bulletsFired = 0;
    private float lastShootTime;
    private bool isShooting = false;
    private Vector3 originalPosition;

    private Transform currentTarget; // Closest player at each frame

    void Start()
    {
        lastShootTime = -shootInterval;
        originalPosition = transform.position;
    }

    void Update()
    {
        // Choose the closest player
        float distToP1 = Vector3.Distance(transform.position, player1.position);
        float distToP2 = Vector3.Distance(transform.position, player2.position);
        currentTarget = distToP1 < distToP2 ? player1 : player2;

        // Proceed only if closest player is within range
        if (Vector3.Distance(transform.position, currentTarget.position) <= detectionRange)
        {
            if (!isShooting && bulletsFired < maxBullets)
            {
                animator.SetBool("IsShooting", true);
                isShooting = true;
                bulletsFired++;

                transform.position += Vector3.up * liftAmount;
            }

            // Face the current target on XZ plane only
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction);

            if (Time.time - lastShootTime >= shootInterval && bulletsFired < maxBullets)
            {
                Shoot(currentTarget);
                lastShootTime = Time.time;
            }
        }
        else
        {
            if (isShooting)
            {
                animator.SetBool("IsShooting", false);
                isShooting = false;
                bulletsFired = 0;
                transform.position = originalPosition;
            }
        }
    }

    void Shoot(Transform target)
    {
        if (shootingVFXPrefab != null)
        {
            Instantiate(shootingVFXPrefab, shootingPoint.transform.position, shootingPoint.transform.rotation);
        }

        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.transform.position, Quaternion.identity);

        Vector3 shootDirection = (target.position - bullet.transform.position).normalized;
        shootDirection.y = 0f;

        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.velocity = shootDirection * bulletSpeed;

        bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == stopShootingCollider)
        {
            animator.SetBool("IsShooting", false);
            isShooting = false;
            bulletsFired = 0;
            transform.position = originalPosition;
        }
    }
}
