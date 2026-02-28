using UnityEngine;
using TMPro;

public class InGameClock : MonoBehaviour
{
    [Header("Time Settings")]
    public int startHour = 6;               // 6:00 AM
    public int endHour = 18;                 // 6:00 PM
    public float totalRealSeconds = 600f;    // 10 minutes real time for full shift

    [Header("UI Reference")]
    public TextMeshProUGUI clockText;        // Assign in Inspector

    [Header("Optional: 12/24 Hour Format")]
    public bool use12HourFormat = true;

    // Current in‑game time values (read‑only)
    public int CurrentHour { get; private set; }
    public int CurrentMinute { get; private set; }
    public string Period => CurrentHour < 12 ? "AM" : "PM";

    /// <summary>
    /// Update the clock based on real elapsed time (in seconds).
    /// Call this every frame with the elapsed real time.
    /// </summary>
    /// <param name="realElapsedSeconds">How many real seconds have passed since shift start.</param>
    public void UpdateClock(float realElapsedSeconds)
    {
        // Clamp elapsed to total shift duration
        realElapsedSeconds = Mathf.Clamp(realElapsedSeconds, 0f, totalRealSeconds);

        // Total in‑game minutes from start to end (e.g., 12 hours = 720 minutes)
        float totalInGameMinutes = (endHour - startHour) * 60f;

        // Map real elapsed to in‑game minutes
        float inGameMinutesElapsed = (realElapsedSeconds / totalRealSeconds) * totalInGameMinutes;

        // Calculate hour and minute
        CurrentHour = startHour + Mathf.FloorToInt(inGameMinutesElapsed / 60f);
        CurrentMinute = Mathf.FloorToInt(inGameMinutesElapsed % 60f);

        // At the very end, ensure we don't overshoot
        if (CurrentHour >= endHour)
        {
            CurrentHour = endHour;
            CurrentMinute = 0;
        }

        UpdateDisplay();
    }

    /// <summary>
    /// Directly set the clock to a specific in‑game time (useful for testing).
    /// </summary>
    public void SetClock(int hour, int minute)
    {
        CurrentHour = Mathf.Clamp(hour, startHour, endHour);
        CurrentMinute = Mathf.Clamp(minute, 0, 59);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (clockText == null) return;

        if (use12HourFormat)
        {
            int displayHour = CurrentHour % 12;
            if (displayHour == 0) displayHour = 12;
            clockText.text = $"{displayHour:00}:{CurrentMinute:00} {Period}";
        }
        else
        {
            // 24‑hour format
            clockText.text = $"{CurrentHour:00}:{CurrentMinute:00}";
        }
    }
}