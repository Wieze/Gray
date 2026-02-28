using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AnomalyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;               // Speed when not chasing
    public float chaseSpeed = 8f;               // Speed when chasing player
    public float gravity = 10f;                  // Gravity applied to the anomaly

    [Header("Chase Settings")]
    public Transform playerTarget;                // The player's transform
    public float detectionRange = 15f;            // Distance at which anomaly starts chasing
    public float stoppingDistance = 2f;            // How close it gets before stopping
    public bool isActive = true;                   // Master toggle

    [Header("Optional Patrol")]
    public bool enablePatrol = false;              // If true, anomaly wanders when not chasing
    public float patrolRadius = 10f;                // Radius for random patrol points
    public float waitTime = 2f;                      // Pause between patrol points

    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector3 velocity;                        // For gravity
    private bool isChasing = false;

    // Patrol variables
    private Vector3 patrolTarget;
    private float waitTimer;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Auto‑find player if not assigned
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTarget = player.transform;
        }

        if (enablePatrol)
        {
            SetNewPatrolTarget();
        }
    }

    void Update()
    {
        if (!isActive || playerTarget == null) return;

        // Distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        isChasing = distanceToPlayer <= detectionRange;

        Vector3 targetDirection = Vector3.zero;

        if (isChasing)
        {
            // Chase the player
            if (distanceToPlayer > stoppingDistance)
            {
                targetDirection = (playerTarget.position - transform.position).normalized;
                moveDirection.x = targetDirection.x * chaseSpeed;
                moveDirection.z = targetDirection.z * chaseSpeed;
            }
            else
            {
                // Close enough – stop horizontal movement
                moveDirection.x = 0;
                moveDirection.z = 0;
            }
        }
        else if (enablePatrol)
        {
            // Patrol behaviour
            Patrol();
        }
        else
        {
            // Idle – no horizontal movement
            moveDirection.x = 0;
            moveDirection.z = 0;
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -0.5f; // small downward force to keep grounded
        }

        // Combine horizontal movement with gravity
        Vector3 finalMovement = new Vector3(moveDirection.x, velocity.y, moveDirection.z);
        characterController.Move(finalMovement * Time.deltaTime);
    }

    void Patrol()
    {
        // If we have reached the patrol target or waited enough, pick a new one
        if (Vector3.Distance(transform.position, patrolTarget) < 1f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                SetNewPatrolTarget();
                waitTimer = 0f;
            }

            // Stop moving while waiting
            moveDirection.x = 0;
            moveDirection.z = 0;
        }
        else
        {
            // Move towards patrol target
            Vector3 dir = (patrolTarget - transform.position).normalized;
            moveDirection.x = dir.x * walkSpeed;
            moveDirection.z = dir.z * walkSpeed;
        }
    }

    void SetNewPatrolTarget()
    {
        // Random point within a circle on XZ plane
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        patrolTarget = new Vector3(transform.position.x + randomCircle.x, transform.position.y, transform.position.z + randomCircle.y);
    }

    // Optional: Visualise detection range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}