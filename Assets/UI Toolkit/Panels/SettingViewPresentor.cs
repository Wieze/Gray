using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsSceneManager : MonoBehaviour
{
    public Action OnBack;

    void Awake()
    {
        UIDocument doc = GetComponent<UIDocument>();
        if (doc == null) return;

        VisualElement root = doc.rootVisualElement;
        Button back = root.Q<Button>("Back");
        if (back != null)
            back.clicked += () => OnBack?.Invoke();
    }
}