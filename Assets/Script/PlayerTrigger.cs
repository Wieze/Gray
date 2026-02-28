using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public WinLoose winLooseScript;
    public TimerScript timerScript;   // Add reference to TimerScript

    void Update()
    {
        // If player falls out of world (example win condition)
        if (transform.position.y < -10.0f)
        {
            // Stop the timer first
            if (timerScript != null)
                timerScript.StopTimer();

            // Then trigger win
            winLooseScript.WinLevel();
        }
    }
}