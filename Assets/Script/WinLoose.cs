using UnityEngine;

public class WinLoose : MonoBehaviour
{
    [Header("References")]
    public TimerScript timerScript;
    public View view;   // Drag the GameObject that has the View script here

    private bool ended = false;

    public void WinLevel()
    {
        if (ended) return;
        ended = true;

        if (timerScript != null) timerScript.StopTimer();
        if (view != null) view.ShowWin();
        else Debug.LogError("WinLoose: View not assigned!");
    }

    public void LoseLevel()
    {
        LoseLevel("Killed by Stalker Anomaly.");
    }

    public void LoseLevel(string reason)
    {
        if (ended) return;
        ended = true;

        if (timerScript != null) timerScript.StopTimer();
        if (view != null) view.ShowLose(reason);
        else Debug.LogError("WinLoose: View not assigned!");

        Debug.Log(reason);
    }
}