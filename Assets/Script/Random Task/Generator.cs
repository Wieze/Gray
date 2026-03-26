using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour
{
    public static bool powerOn = true;

    public Transform player;
    public float interactDistance = 3f;

    private bool isRestarting = false;

    private SystemBreak systemBreak;

    void Start()
    {
        systemBreak = GetComponent<SystemBreak>();
    }

    void Update()
    {
        // 🔴 Broken → no power
        if (systemBreak != null && systemBreak.isBroken)
        {
            powerOn = false;
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E) && !isRestarting)
        {
            StartCoroutine(RestartGenerator());
        }
    }

    IEnumerator RestartGenerator()
    {
        isRestarting = true;

        Debug.Log("GENERATOR RESTARTING...");

        powerOn = false;

        yield return new WaitForSeconds(3f); // 😈 tension delay

        powerOn = true;

        Debug.Log("GENERATOR ONLINE");

        isRestarting = false;
    }
}