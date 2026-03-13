using UnityEngine;
using UnityEngine.Pool;

public class AChase : MonoBehaviour
{
    [Header("Chase Settings")]
    public Transform target;
    public float moveSpeed = 2f;
    public bool isActive = true;
    public bool autoFindPlayer = true;

    [Header("Detection & Attack")]
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float sanityDrainRate = 5f;          // Sanity points per second

    [Header("Wall Detection")]
    public bool enableWallDetection = true;
    public LayerMask wallLayer = 1 << 0;
    public float wallCheckDistance = 1f;

    [Header("Spawn Protection (Optional)")]
    public float initialGracePeriod = 0f;        // Seconds before it can drain sanity (0 = no grace)

    [Header("References")]
    public SanityManager sanityManager;

    private float spawnTimer;

    private IObjectPool<AChase> aChasePool;
    

    void Start()
    {
        spawnTimer = initialGracePeriod;

        if (autoFindPlayer && target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        if (sanityManager == null)
            sanityManager = FindObjectOfType<SanityManager>();
    }

    void Update()
    {
        if (!isActive || target == null) return;

        // Decrease spawn protection timer
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
            return; // Don't move or attack during grace period
        }

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // If within attack range, drain sanity and stop moving
        if (distanceToPlayer <= attackRange)
        {
            DrainPlayerSanity();
            return;
        }

        // Only move if within detection range
        if (distanceToPlayer <= detectionRange)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;

            if (enableWallDetection && IsWallInFront(direction))
                return;

            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void DrainPlayerSanity()
    {
        if (sanityManager != null)
        {
            sanityManager.DrainSanity(sanityDrainRate * Time.deltaTime);
        }
    }

    bool IsWallInFront(Vector3 moveDirection)
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(origin, moveDirection, out hit, wallCheckDistance, wallLayer))
        {
            Debug.DrawRay(origin, moveDirection * hit.distance, Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(origin, moveDirection * wallCheckDistance, Color.green);
            return false;
        }
    }
}