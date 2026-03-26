using UnityEngine;
using UnityEngine.UIElements;

public class StartViewPresentor : MonoBehaviour
{
    private VisualElement _mainMenuPanel;
    private VisualElement _settingsPanel;

    void Start()
    {
        UIDocument doc = GetComponent<UIDocument>();
        if (doc == null) return;

        VisualElement root = doc.rootVisualElement;
        _mainMenuPanel = root.Q("MainMenu");
        _settingsPanel = root.Q("SettingView");

        ShowMainMenu();

        MainMenu menu = GetComponent<MainMenu>();
        if (menu != null) menu.OpenSettings = ShowSettings;

        SettingsSceneManager settings = GetComponent<SettingsSceneManager>();
        if (settings != null) settings.OnBack = ShowMainMenu;
    }

    void ShowMainMenu()
    {
        if (_mainMenuPanel != null) _mainMenuPanel.style.display = DisplayStyle.Flex;
        if (_settingsPanel != null) _settingsPanel.style.display = DisplayStyle.None;
    }

    void ShowSettings()
    {
        if (_mainMenuPanel != null) _mainMenuPanel.style.display = DisplayStyle.None;
        if (_settingsPanel != null) _settingsPanel.style.display = DisplayStyle.Flex;
    }
}