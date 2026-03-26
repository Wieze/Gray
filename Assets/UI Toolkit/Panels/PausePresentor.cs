using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PausePresenter : MonoBehaviour
{
    [Header("References")]
    public View view;                         // Drag the GameUI (with View) here, or leave null to auto‑find
    public string mainMenuScene = "MainMenu";  // Name of the main menu scene

    void Awake()
    {
        // Get the UIDocument on this GameObject
        UIDocument doc = GetComponent<UIDocument>();
        if (doc == null)
        {
            Debug.LogError("PausePresenter: No UIDocument found on this GameObject.");
            return;
        }

        VisualElement root = doc.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("PausePresenter: rootVisualElement is null. Check UIDocument source.");
            return;
        }

        // Auto‑find the View script if not assigned
        if (view == null)
            view = FindObjectOfType<View>();

        if (view == null)
            Debug.LogWarning("PausePresenter: No View script found in the scene. Resume may not work.");

        // Wire up buttons
        Button resumeBtn = root.Q<Button>("Resume");
        if (resumeBtn != null)
            resumeBtn.clicked += OnResumeClicked;
        else
            Debug.LogWarning("Button named 'Resume' not found.");

        Button settingsBtn = root.Q<Button>("Settings");
        if (settingsBtn != null)
            settingsBtn.clicked += OnSettingsClicked;
        else
            Debug.LogWarning("Button named 'Settings' not found.");

        Button mainMenuBtn = root.Q<Button>("MainMenu");
        if (mainMenuBtn != null)
            mainMenuBtn.clicked += OnMainMenuClicked;
        else
            Debug.LogWarning("Button named 'MainMenu' not found.");

        Button exitBtn = root.Q<Button>("Exit");
        if (exitBtn != null)
            exitBtn.clicked += OnExitClicked;
        else
            Debug.LogWarning("Button named 'Exit' not found.");
    }

    private void OnResumeClicked()
    {
        Debug.Log("Resume button clicked");
        if (view != null)
            view.ResumeGame();
        else
            Debug.LogError("Cannot resume: View script not found.");
    }

    private void OnSettingsClicked()
    {
        Debug.Log("Settings clicked – you can extend this to show a settings panel.");
        // Example: call a method on the View to show settings if you have one
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("Loading main menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    private void OnExitClicked()
    {
        Debug.Log("Exit clicked");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}