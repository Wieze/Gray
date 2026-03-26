using UnityEngine;
using System.Collections.Generic;

public class MalfunctionManager : MonoBehaviour
{
    public List<SystemBreak> systems = new List<SystemBreak>();

    private float timer = 0f;
    private float checkInterval = 60f;

    private float gameTime = 0f;

    private int currentBroken = 0;
    private int maxBroken = 3;

    void Update()
    {
        timer += Time.deltaTime;
        gameTime += Time.deltaTime;

        UpdateDifficulty();

        if (timer >= checkInterval)
        {
            timer = 0f;
            TryBreakSystem();
        }
    }

    void UpdateDifficulty()
    {
        float minutes = gameTime / 60f;

        if (minutes >= 7)
        {
            maxBroken = 7;
            checkInterval = 25f;
        }
        else if (minutes >= 4)
        {
            maxBroken = 5;
            checkInterval = 40f;
        }
        else
        {
            maxBroken = 3;
            checkInterval = 60f;
        }

        // 🔥 Chain reaction (more broken = faster chaos)
        if (currentBroken >= 3)
        {
            checkInterval *= 0.7f; // faster
        }
    }

    void TryBreakSystem()
    {
        if (systems.Count == 0) return;
        if (currentBroken >= maxBroken) return;

        int index = Random.Range(0, systems.Count);

        if (!systems[index].isBroken)
        {
            systems[index].BreakSystem();
            currentBroken++;

            Debug.Log("BROKEN COUNT: " + currentBroken);
        }
    }

    public void SystemFixed()
    {
        if (currentBroken > 0)
            currentBroken--;

        Debug.Log("FIXED → CURRENT: " + currentBroken);
    }
}