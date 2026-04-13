using UnityEngine;

public class RoamingAnomaly : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float idleTime = 2f;
    public float waypointRadius = 10f;
    public LayerMask obstacleLayer = 1 << 0;

    [Header("Detection Settings")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;
    public float healthDrainRate = 5f;

    [Header("References")]
    public Transform player;
    public HealthManager healthManager;

    private Vector3 currentWaypoint;
    private bool isWaiting = false;
    private bool isChasing = false;
    private float waitTimer = 0f;

    private enum State { Roaming, Chasing, Attacking, Waiting }
    private State currentState = State.Roaming;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (healthManager == null)
            healthManager = FindObjectOfType<HealthManager>();

        GetNewWaypoint();
    }

    void Update()
    {
        if (player == null || healthManager == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && !isChasing)
        {
            currentState = State.Chasing;
            isChasing = true;
        }
        else if (distanceToPlayer > detectionRange * 1.5f && isChasing)
        {
            currentState = State.Roaming;
            isChasing = false;
            GetNewWaypoint();
        }

        switch (currentState)
        {
            case State.Roaming:
                Roam();
                break;
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Attacking:
                AttackPlayer();
                break;
        }

        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attacking;
        }
    }

    void Roam()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= idleTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                GetNewWaypoint();
            }
            return;
        }

        Vector3 direction = (currentWaypoint - transform.position).normalized;
        direction.y = 0;

        if (IsPathBlocked(direction))
        {
            GetNewWaypoint();
            return;
        }

        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, currentWaypoint) < 1f)
        {
            isWaiting = true;
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (IsPathBlocked(direction))
        {
            currentState = State.Roaming;
            isChasing = false;
            GetNewWaypoint();
            return;
        }

        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }

    void AttackPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer > attackRange)
        {
            currentState = State.Chasing;
            return;
        }

        if (healthManager != null)
        {
            healthManager.DrainHealth(healthDrainRate * Time.deltaTime);
        }
    }

    void GetNewWaypoint()
    {
        Vector3 randomPoint = transform.position + (Random.insideUnitSphere * waypointRadius);
        randomPoint.y = transform.position.y;

        RaycastHit hit;
        if (Physics.Raycast(randomPoint + Vector3.up * 5f, Vector3.down, out hit, 10f))
        {
            randomPoint.y = hit.point.y;
        }

        currentWaypoint = randomPoint;
    }

    bool IsPathBlocked(Vector3 direction)
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        return Physics.Raycast(origin, direction, out hit, 1.5f, obstacleLayer);
    }
}