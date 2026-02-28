using UnityEngine;
using UnityEngine.UI;   // For legacy Text, or TMPro for TextMeshPro

public class WinLoose : MonoBehaviour
{
    public TimerScript timerScript;      // Reference to the timer
    public GameObject winScreen;
    public GameObject loseScreen;
    public Text loseReasonText;           // Optional UI text to display reason

    public void WinLevel()
    {
        if (timerScript != null) timerScript.StopTimer();
        if (winScreen != null) winScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LoseLevel()
    {
        LoseLevel("Killed by Stalker Anomaly."); // default message
    }

    public void LoseLevel(string reason)
    {
        if (timerScript != null) timerScript.StopTimer();
        if (loseScreen != null)
        {
            loseScreen.SetActive(true);
            if (loseReasonText != null)
                loseReasonText.text = reason;
        }
        Debug.Log(reason);
        Time.timeScale = 0f;

        
    }

    
}