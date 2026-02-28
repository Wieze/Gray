using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    public WinLoose winLooseScript;
    public TimerScript timerScript;   // Add reference to the timer

    void OnTriggerEnter(Collider other)
    {
        // Stop the timer immediately
        if (timerScript != null)
            timerScript.StopTimer();

        // Then trigger lose condition
        winLooseScript.LoseLevel();
    }

    
}