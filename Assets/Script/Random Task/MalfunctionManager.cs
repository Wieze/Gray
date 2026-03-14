using UnityEngine;
using System.Collections.Generic;

public class MalfunctionManager : MonoBehaviour
{
    public List<SystemBreak> systems = new List<SystemBreak>();

    public float checkInterval = 180f; // 3 minutes
    private float timer = 0f;
    private float gameTime = 0f;

    private int currentMalfunctions = 0;
    private int maxMalfunctions = 3;

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
            maxMalfunctions = 5;
        else if (minutes >= 4)
            maxMalfunctions = 4;
        else
            maxMalfunctions = 3;
    }

    void TryBreakSystem()
    {
        if (systems.Count == 0) return;

        if (currentMalfunctions >= maxMalfunctions)
            return;

        int randomIndex = Random.Range(0, systems.Count);

        if (!systems[randomIndex].isBroken)
        {
            systems[randomIndex].BreakSystem();
            currentMalfunctions++;
        }
    }

    public void SystemFixed()
    {
        if (currentMalfunctions > 0)
            currentMalfunctions--;
    }
}