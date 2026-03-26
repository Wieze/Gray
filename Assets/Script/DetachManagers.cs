using UnityEngine;

public class DetachManagers : MonoBehaviour
{
    void Start()
    {
        // Create a persistent GameObject to host all managers
        GameObject managersGO = GameObject.Find("ManagersContainer");
        if (managersGO == null)
        {
            managersGO = new GameObject("ManagersContainer");
            DontDestroyOnLoad(managersGO);
        }

        // List of manager types to detach
        System.Type[] managerTypes = {
            typeof(StabilityManager),
            typeof(SanityManager),
            typeof(TimerScript),
            typeof(WinLoose),
            // add any other manager scripts here
        };

        foreach (var type in managerTypes)
        {
            var component = GetComponent(type);
            if (component != null)
            {
                // Move the component to the new container
                component.transform.SetParent(managersGO.transform);
                Debug.Log($"Moved {type.Name} to {managersGO.name}");
            }
        }

        // Optionally, also detach any children that contain managers
        // (if you have them as separate children)
        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child == transform) continue;
            foreach (var type in managerTypes)
            {
                var comp = child.GetComponent(type);
                if (comp != null)
                {
                    comp.transform.SetParent(managersGO.transform);
                    Debug.Log($"Moved {type.Name} (child) to {managersGO.name}");
                }
            }
        }

        // After moving, the HUD GameObject can be safely disabled without affecting the managers
        Debug.Log("All managers detached. HUD can now be safely disabled.");
    }
}