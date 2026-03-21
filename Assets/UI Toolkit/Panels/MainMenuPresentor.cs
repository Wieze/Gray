using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private void Awake()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("MainMenu: UIDocument not found.");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;
        if (root == null) return;

        // Use the exact names from the UI Builder (no # symbol)
        Button startBtn = root.Q<Button>("Start_Shift");
        Button continueBtn = root.Q<Button>("Continue");
        Button settingBtn = root.Q<Button>("Setting");
        Button exitBtn = root.Q<Button>("Exit");

        if (startBtn != null)
            startBtn.clicked += () => Debug.Log("Start button Click");
        else
            Debug.LogWarning("Button named 'Start_Shift' not found.");

        if (continueBtn != null)
            continueBtn.clicked += () => Debug.Log("Continue button Click");
        else
            Debug.LogWarning("Button named 'Continue' not found.");

        if (settingBtn != null)
            settingBtn.clicked += () => Debug.Log("Setting button Click");
        else
            Debug.LogWarning("Button named 'Setting' not found.");

        if (exitBtn != null)
            exitBtn.clicked += () => Debug.Log("Exit button Click");
        else
            Debug.LogWarning("Button named 'Exit' not found.");
    }
}