using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TaskUI : MonoBehaviour
{
    public static TaskUI instance;

    public TextMeshProUGUI taskText;

    private List<string> tasks = new List<string>();

    void Awake()
    {
        instance = this;
    }

    public void AddTask(string task)
    {
        if (!tasks.Contains(task))
        {
            tasks.Add(task);
            UpdateUI();
        }
    }

    public void RemoveTask(string task)
    {
        if (tasks.Contains(task))
        {
            tasks.Remove(task);
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (tasks.Count == 0)
        {
            taskText.text = "ALL SYSTEMS NORMAL";
            return;
        }

        string text = "TASKS:\n\n";

        foreach (string t in tasks)
        {
            text += "- " + t + "\n";
        }

        taskText.text = text;
    }
}