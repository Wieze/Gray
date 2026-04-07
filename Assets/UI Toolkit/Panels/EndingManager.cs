using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [Header("UI Documents")]
    public UIDocument happyEndingUI;
    public UIDocument failedEndingUI;

    [Header("Jumpscare Settings")]
    public float jumpscareDuration = 0.5f;
    public float textFadeInDuration = 0.3f;
    public AudioClip jumpscareSound;

    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";
    public string gameplayScene = "GamePlay";

    private bool gameEnded = false;

    public void CheckEnding()
    {
        if (gameEnded) return;
        gameEnded = true;

        MalfunctionManager malfunctionManager = FindObjectOfType<MalfunctionManager>();
        bool hasPendingTasks = malfunctionManager != null && malfunctionManager.GetBrokenCount() > 0;

        if (hasPendingTasks)
        {
            StartCoroutine(ShowFailedEndingWithJumpscare());
        }
        else
        {
            ShowHappyEnding();
        }
    }

    private IEnumerator ShowFailedEndingWithJumpscare()
    {
        failedEndingUI.enabled = true;
        
        var root = failedEndingUI.rootVisualElement;
        VisualElement textContainer = root.Q<VisualElement>("TextContainer");
        Image jumpscareImage = root.Q<Image>("JumpscareImage");
        
        if (jumpscareImage != null) jumpscareImage.style.opacity = 1;
        if (textContainer != null) textContainer.style.opacity = 0;
        
        if (jumpscareSound != null)
            AudioSource.PlayClipAtPoint(jumpscareSound, Camera.main.transform.position);
        
        yield return new WaitForSeconds(jumpscareDuration);
        
        // Fade out jumpscare
        float fadeTime = 0.2f;
        float elapsed = 0;
        if (jumpscareImage != null)
        {
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                jumpscareImage.style.opacity = Mathf.Lerp(1, 0, elapsed / fadeTime);
                yield return null;
            }
            jumpscareImage.style.opacity = 0;
        }
        
        // Fade in text
        elapsed = 0;
        if (textContainer != null)
        {
            while (elapsed < textFadeInDuration)
            {
                elapsed += Time.deltaTime;
                textContainer.style.opacity = Mathf.Lerp(0, 1, elapsed / textFadeInDuration);
                yield return null;
            }
            textContainer.style.opacity = 1;
        }
        
        WireFailedEndingButtons();
    }

    private void ShowHappyEnding()
    {
        if (happyEndingUI != null)
        {
            happyEndingUI.enabled = true;
            WireHappyEndingButtons();
        }
    }

    private void WireHappyEndingButtons()
    {
        var root = happyEndingUI.rootVisualElement;
        Button mainMenuBtn = root.Q<Button>("MainMenu");
        if (mainMenuBtn != null)
            mainMenuBtn.clicked += ReturnToMainMenu;
    }

    private void WireFailedEndingButtons()
    {
        var root = failedEndingUI.rootVisualElement;
        Button retryBtn = root.Q<Button>("RetryShift");
        if (retryBtn != null)
            retryBtn.clicked += RetryShift;
        
        Button mainMenuBtn = root.Q<Button>("MainMenu");
        if (mainMenuBtn != null)
            mainMenuBtn.clicked += ReturnToMainMenu;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void RetryShift()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplayScene);
    }
}