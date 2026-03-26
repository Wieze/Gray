using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneAsset gameplayScene;   // Drag the scene here in the Inspector

    private Button _settingButton;
    private Action _openSettingsAction;
    public Action OpenSettings { set => _openSettingsAction = value; }

    void Awake()
    {
        UIDocument doc = GetComponent<UIDocument>();
        if (doc == null)
        {
            Debug.LogError("MainMenu: No UIDocument found on this GameObject.");
            return;
        }

        VisualElement root = doc.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("MainMenu: rootVisualElement is null. Check UIDocument source.");
            return;
        }

        _settingButton = root.Q<Button>("Setting");
        if (_settingButton != null)
        {
            _settingButton.clicked += () => _openSettingsAction?.Invoke();
            Debug.Log("Setting button wired.");
        }
        else
        {
            Debug.LogError("MainMenu: Button named 'Setting' not found.");
        }

        // Start Shift button
        Button startBtn = root.Q<Button>("Start_Shift");
        if (startBtn != null)
        {
            startBtn.clicked += LoadGameScene;
        }
        else
            Debug.LogWarning("Button 'Start_Shift' not found.");

        // Other buttons (optional)
        Button continueBtn = root.Q<Button>("Continue");
        if (continueBtn != null) continueBtn.clicked += () => Debug.Log("Continue clicked");
        else Debug.LogWarning("Button 'Continue' not found.");

        Button exitBtn = root.Q<Button>("Exit");
        if (exitBtn != null) exitBtn.clicked += () => Debug.Log("Exit clicked");
        else Debug.LogWarning("Button 'Exit' not found.");
    }

    private void LoadGameScene()
    {
        // Get scene name from the assigned SceneAsset, or fallback to a default
        string sceneName = gameplayScene != null ? gameplayScene.name : "GamePlay";

        // Check if the scene is available in build settings
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.Log($"Loading gameplay scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' could not be loaded. Make sure it is added to Build Settings (File → Build Settings).");
        }
    }
}