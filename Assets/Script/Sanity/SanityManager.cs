using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class SanityManager : MonoBehaviour
{
    public Slider sanitySlider;
    public PostProcessProfile profile;
    private Vignette vignette;

    public int fullSanity = 100;
    public int difficulty = 1;              // Multiplier for sanity drain rate

    [Header("Lose Condition")]
    public WinLoose winLooseScript;
    public TimerScript timerScript;

    private bool isDead = false;

    void Start()
    {
        // Auto-find WinLoose and Timer if not assigned
        if (winLooseScript == null)
            winLooseScript = FindObjectOfType<WinLoose>();
        if (timerScript == null)
            timerScript = FindObjectOfType<TimerScript>();

        // Setup slider
        if (sanitySlider == null)
            sanitySlider = GetComponent<Slider>();

        if (sanitySlider != null)
        {
            sanitySlider.maxValue = fullSanity;
            sanitySlider.value = fullSanity;
        }

        // Setup post-processing vignette
        if (profile != null)
        {
            profile.TryGetSettings(out vignette);
            if (vignette != null)
            {
                vignette.intensity.value = 0f; // start with no vignette
            }
        }

        // Start draining sanity
        StartCoroutine(LoseSanity());
    }

    IEnumerator LoseSanity()
    {
        while (sanitySlider != null && sanitySlider.value > 0)
        {
            // Drain sanity over time
            sanitySlider.value -= 2f * difficulty * Time.deltaTime;

            // Update vignette intensity based on sanity percent lost
            float percentLost = 1f - (sanitySlider.value / sanitySlider.maxValue);
            if (vignette != null)
                vignette.intensity.value = percentLost;

            yield return null;
        }

        // Sanity reached zero – trigger lose condition
        if (!isDead)
        {
            isDead = true;

            // Stop the timer
            if (timerScript != null)
                timerScript.StopTimer();

            // Trigger lose screen with custom message
            if (winLooseScript != null)
                winLooseScript.LoseLevel("You went insane...");
            else
                Debug.LogError("WinLoose script missing! Player went insane but no lose screen.");
        }
    }

    // Public methods to modify sanity externally
    public void AddSanity(float amount)
    {
        if (sanitySlider != null)
            sanitySlider.value = Mathf.Clamp(sanitySlider.value + amount, 0, fullSanity);
    }

    public void DrainSanity(float amount)
    {
        if (sanitySlider != null)
            sanitySlider.value = Mathf.Clamp(sanitySlider.value - amount, 0, fullSanity);
    }

    public void AffectSanity(float value)
    {
        sanitySlider.value += value;
    }
}