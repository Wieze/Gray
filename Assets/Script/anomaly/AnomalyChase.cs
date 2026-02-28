using UnityEngine;

public class AChase : MonoBehaviour
{
    [Header("Chase Settings")]
    public Transform target;
    public float moveSpeed = 2f;
    public bool isActive = true;
    public bool autoFindPlayer = true;

    [Header("Detection & Catch")]
    public float detectionRange = 10f;      // Only chase if player within this range
    public float catchDistance = 1.5f;       // How close to trigger lose

    [Header("Wall Detection (Optional)")]
    public bool enableWallDetection = true;
    public LayerMask wallLayer = 1 << 0;
    public float wallCheckDistance = 1f;

    [Header("Lose Condition")]
    public WinLoose winLooseScript;
    public TimerScript timerScript;

    private bool hasKilled = false;

    void Start()
    {
        if (autoFindPlayer && target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }

        if (winLooseScript == null)
            winLooseScript = FindObjectOfType<WinLoose>();
        if (timerScript == null)
            timerScript = FindObjectOfType<TimerScript>();
    }

    void Update()
    {
        if (!isActive || target == null || hasKilled) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // If within catch distance, kill
        if (distanceToPlayer <= catchDistance)
        {
            TriggerLose();
            return;
        }

        // Only move if within detection range
        if (distanceToPlayer <= detectionRange)
        {
            // Direction toward player (ignore vertical difference)
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;

            // Wall detection (optional)
            if (enableWallDetection && IsWallInFront(direction))
                return; // Don't move if wall ahead

            // Move toward player
            Vector3 move = direction * moveSpeed * Time.deltaTime;
            transform.Translate(move, Space.World);
        }
        // else: do nothing (idle)
    }

    bool IsWallInFront(Vector3 moveDirection)
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f; // slightly above ground
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

    void TriggerLose()
    {
        if (hasKilled) return;
        hasKilled = true;

        if (timerScript != null)
            timerScript.StopTimer();

        if (winLooseScript != null)
            winLooseScript.LoseLevel("Killed by Stalker Anomaly.");
        else
            Debug.LogError("WinLoose script missing!");

        isActive = false; // Stop chasing
    }
}