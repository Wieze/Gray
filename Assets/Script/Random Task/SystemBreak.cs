using UnityEngine;

public class SystemBreak : MonoBehaviour
{
    public bool isBroken = false;
    public MalfunctionManager manager;

    public GameObject systemObject;

    public void BreakSystem()
    {
        isBroken = true;
        systemObject.SetActive(false);

        Debug.Log(gameObject.name + " BROKEN");
    }

    public void FixSystem()
    {
        isBroken = false;
        systemObject.SetActive(true);

        manager.SystemFixed();

        Debug.Log(gameObject.name + " FIXED");
    }

    void Update()
    {
        if (isBroken && Input.GetKeyDown(KeyCode.E))
        {
            FixSystem();
        }
    }
}