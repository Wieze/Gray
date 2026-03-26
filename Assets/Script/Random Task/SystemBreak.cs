using UnityEngine;

public class SystemBreak : MonoBehaviour
{
    [Header("System Info")]
    public string systemName = "SYSTEM";

    [Header("State")]
    public bool isBroken = false;

    [Header("Interaction")]
    public Transform player;
    public float interactDistance = 3f;

    [Header("Manager")]
    public MalfunctionManager manager;

    void Update()
    {
        // Only allow fixing if broken
        if (!isBroken) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                FixSystem();
            }
        }
    }

    // 🔴 BREAK SYSTEM
    public void BreakSystem()
    {
        if (isBroken) return;

        isBroken = true;

        // DO NOT disable object anymore (fixed issue)
        // systemObject.SetActive(false); ❌ removed

        if (TaskUI.instance != null)
        {
            TaskUI.instance.AddTask("Fix " + systemName);
        }

        Debug.Log(systemName + " BROKEN");
    }

    // 🟢 FIX SYSTEM
    public void FixSystem()
    {
        if (!isBroken) return;

        isBroken = false;

        if (TaskUI.instance != null)
        {
            TaskUI.instance.RemoveTask("Fix " + systemName);
        }

        if (manager != null)
        {
            manager.SystemFixed();
        }

        Debug.Log(systemName + " FIXED");
    }
}