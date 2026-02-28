using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    public float timeRemaining = 600f;        // 10 minutes = 600 seconds
    public bool timerIsRunning = false;

    [Header("Optional: Real Timer Display")]
    public TextMeshProUGUI realTimerText;

    [Header("In‑Game Clock Reference")]
    public InGameClock inGameClock;            // Drag the object with InGameClock here

    // Public method to stop the timer (e.g., when win/lose occurs)
    public void StopTimer()
    {
        timerIsRunning = false;
    }

    // Public method to start/resume the timer
    public void StartTimer()
    {
        timerIsRunning = true;
    }

    // Optional: Reset timer to full value
    public void ResetTimer()
    {
        timeRemaining = 600f;
        timerIsRunning = false;   // stopped after reset; call StartTimer() to begin
    }

    void Start()
    {
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayRealTime(timeRemaining);

                float elapsed = 600f - timeRemaining;
                if (inGameClock != null)
                    inGameClock.UpdateClock(elapsed);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                DisplayRealTime(timeRemaining);
                if (inGameClock != null)
                    inGameClock.UpdateClock(600f);
                OnTimerEnd();
            }
        }
    }

    void DisplayRealTime(float timeToDisplay)
    {
        if (realTimerText == null) return;
        timeToDisplay = Mathf.Max(0, timeToDisplay);
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        realTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void OnTimerEnd()
    {
        Debug.Log("Shift complete! (10 minutes real time)");
        // Trigger win condition here if not already triggered by player death
    }
}