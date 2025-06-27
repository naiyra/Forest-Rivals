using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Animator animator;
    public float detectionRange = 10f; // Default detection range
    private bool isPlayerDetected = false;
    private bool isDead = false;
    public Collider enemyCollider; // Reference to the enemy's collider
    void Update()
    {
        // Check for player detection
        DetectPlayer();

        // Update animator parameters based on game logic
        UpdateAnimator();

        // Rotate towards the player if detected and alive
        if (isPlayerDetected && !isDead)
        {
            RotateTowardsPlayer();
        }

 
    }

    void DetectPlayer()
    {
        // Implement your logic for detecting the player here
        // For demonstration purposes, let's assume a simple distance-based detection
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < detectionRange)
            {
                isPlayerDetected = true;
            }
            else
            {
                isPlayerDetected = false;
            }
        }
    }

 public void OnHitHead()
    {
        if (!isDead)
        {
            isDead = true;
             if (enemyCollider != null)
            {
                enemyCollider.enabled = false;
            }
            
        }
    }
    void UpdateAnimator()
    {
        // Update animator parameters based on game logic
        animator.SetBool("isPlayerDetected", isPlayerDetected);
        animator.SetBool("isDead", isDead);
    }

 
   

    void RotateTowardsPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Calculate the direction from the enemy to the player
            Vector3 direction = player.transform.position - transform.position;

            // Ensure rotation is only in the horizontal plane (Y and Z axes)
            direction.y = 0f;

            // Rotate towards the player's position
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

   
}
