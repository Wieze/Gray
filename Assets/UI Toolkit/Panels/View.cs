using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class View : MonoBehaviour
{
    [Header("UI Documents")]
    public UIDocument playerUI;      // PlayerUi.uxml (main HUD)
    public UIDocument pauseUI;       // Pause.uxml
    public UIDocument winUI;         // ShiftComplete.uxml
    public UIDocument loseUI;        // GameOverMenu.uxml

    [Header("Additional GameObjects")]
    public GameObject playerHUDObject;  // The visual HUD GameObject (e.g., "Player HUD (1)")

    [Header("Game Managers")]
    public StabilityManager stabilityManager; // Reference to the stability manager

    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";
    public string gameplayScene = "GamePlay";

    private bool isPaused = false;
    private bool gameEnded = false;

    void Start()
    {
        ShowMainUI();
        WirePauseButtons();
        WireWinButtons();
        WireLoseButtons();

        // Auto‑find stability manager if not assigned
        if (stabilityManager == null)
            stabilityManager = FindObjectOfType<StabilityManager>();
    }

    void Update()
    {
        if (!gameEnded && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (gameEnded) return;
        isPaused = true;
        Time.timeScale = 0f;
        stabilityManager?.PauseStability(); // Notify stability manager
        ShowPauseUI();
    }

    public void ResumeGame()
    {
        if (gameEnded) return;
        isPaused = false;
        Time.timeScale = 1f;
        stabilityManager?.ResumeStability(); // Notify stability manager
        ShowMainUI();
    }

    public void ShowWin()
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 0f;
        ShowWinUI();
    }

    public void ShowLose(string reason = null)
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 0f;
        ShowLoseUI(reason);
    }

    // --- UI visibility helpers ---
    private void ShowMainUI()
    {
        SetActive(playerUI, true);
        if (playerHUDObject != null) playerHUDObject.SetActive(true);
        SetActive(pauseUI, false);
        SetActive(winUI, false);
        SetActive(loseUI, false);
    }

    private void ShowPauseUI()
    {
        SetActive(playerUI, false);
        if (playerHUDObject != null) playerHUDObject.SetActive(false);
        SetActive(pauseUI, true);
        SetActive(winUI, false);
        SetActive(loseUI, false);
    }

    private void ShowWinUI()
    {
        SetActive(playerUI, false);
        if (playerHUDObject != null) playerHUDObject.SetActive(false);
        SetActive(pauseUI, false);
        SetActive(winUI, true);
        SetActive(loseUI, false);
    }

    private void ShowLoseUI(string reason)
    {
        SetActive(playerUI, false);
        if (playerHUDObject != null) playerHUDObject.SetActive(false);
        SetActive(pauseUI, false);
        SetActive(winUI, false);
        SetActive(loseUI, true);

        if (loseUI != null && loseUI.rootVisualElement != null)
        {
            Label reasonLabel = loseUI.rootVisualElement.Q<Label>("LoseReason");
            if (reasonLabel != null && !string.IsNullOrEmpty(reason))
                reasonLabel.text = reason;
        }
    }

    private void SetActive(UIDocument doc, bool active)
    {
        if (doc != null) doc.enabled = active;
    }

    // --- Button wiring (one‑time setup) ---
    private bool pauseWired = false;
    private void WirePauseButtons()
    {
        if (pauseWired) return;
        if (pauseUI == null || pauseUI.rootVisualElement == null) return;

        var root = pauseUI.rootVisualElement;
        Button resume = root.Q<Button>("Resume");
        if (resume != null) resume.clicked += ResumeGame;

        Button settings = root.Q<Button>("Settings");
        if (settings != null) settings.clicked += () => Debug.Log("Settings clicked (implement later)");

        Button mainMenu = root.Q<Button>("MainMenu");
        if (mainMenu != null) mainMenu.clicked += () => LoadScene(mainMenuScene);

        Button exit = root.Q<Button>("Exit");
        if (exit != null) exit.clicked += Application.Quit;

        pauseWired = true;
    }

    private bool winWired = false;
    private void WireWinButtons()
    {
        if (winWired) return;
        if (winUI == null || winUI.rootVisualElement == null) return;

        var root = winUI.rootVisualElement;
        Button continueBtn = root.Q<Button>("Continue");
        if (continueBtn != null) continueBtn.clicked += () => LoadScene(gameplayScene);

        Button mainMenu = root.Q<Button>("MainMenu");
        if (mainMenu != null) mainMenu.clicked += () => LoadScene(mainMenuScene);

        winWired = true;
    }

    private bool loseWired = false;
    private void WireLoseButtons()
    {
        if (loseWired) return;
        if (loseUI == null || loseUI.rootVisualElement == null) return;

        var root = loseUI.rootVisualElement;
        Button retry = root.Q<Button>("RetryShift");
        if (retry != null) retry.clicked += () => LoadScene(gameplayScene);

        Button mainMenu = root.Q<Button>("MainMenu");
        if (mainMenu != null) mainMenu.clicked += () => LoadScene(mainMenuScene);

        loseWired = true;
    }

    private void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}