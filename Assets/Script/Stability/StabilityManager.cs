using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StabilityManager : MonoBehaviour
{
    [Header("Stability Settings")]
    public Slider stabilitySlider;
    public float maxStability = 100f;          // Maximum stability value
    public float fillRatePerSecond = 5f;       // How much stability is added per second
    public bool resetOnFull = true;             // Whether to reset to 0 when reaching max

    [Header("Game Integration (Optional)")]
    public WinLoose winLooseScript;              // For potential lose conditions
    public TimerScript timerScript;               // If stability affects the timer

    // Event triggered when the stability bar completes a cycle (reaches max and resets)
    public System.Action OnStabilityCycleCompleted;

    private bool isCycleActive = true;
    private Coroutine fillCoroutine;

    void Start()
    {
        // Auto-find WinLoose and Timer if not assigned
        if (winLooseScript == null)
            winLooseScript = FindObjectOfType<WinLoose>();
        if (timerScript == null)
            timerScript = FindObjectOfType<TimerScript>();

        // Setup slider
        if (stabilitySlider == null)
            stabilitySlider = GetComponent<Slider>();

        if (stabilitySlider != null)
        {
            stabilitySlider.maxValue = maxStability;
            stabilitySlider.value = 0f; // Start at 0
        }

        // Start the fill coroutine
        if (fillCoroutine == null && isCycleActive)
        {
            fillCoroutine = StartCoroutine(FillStabilityOverTime());
        }
    }

    private IEnumerator FillStabilityOverTime()
    {
        // Continue filling as long as the cycle is active
        while (isCycleActive)
        {
            if (stabilitySlider != null)
            {
                // Add stability over time
                if (stabilitySlider.value < stabilitySlider.maxValue)
                {
                    stabilitySlider.value += fillRatePerSecond * Time.deltaTime;
                }
                else
                {
                    // Reached max – trigger event and reset
                    OnStabilityCycleCompleted?.Invoke();

                    if (resetOnFull)
                    {
                        stabilitySlider.value = 0f;
                    }
                    else
                    {
                        // If not resetting, stop filling (or you could leave it at max)
                        isCycleActive = false;
                        break;
                    }
                }
            }
            yield return null;
        }

        fillCoroutine = null;
    }

    // Public method to manually add stability
    public void AddStability(float amount)
    {
        if (stabilitySlider != null)
            stabilitySlider.value = Mathf.Clamp(stabilitySlider.value + amount, 0, maxStability);
    }

    // Public method to manually reduce stability (if needed)
    public void ReduceStability(float amount)
    {
        if (stabilitySlider != null)
            stabilitySlider.value = Mathf.Clamp(stabilitySlider.value - amount, 0, maxStability);
    }

    // Public method to reset the bar to zero
    public void ResetStability()
    {
        if (stabilitySlider != null)
            stabilitySlider.value = 0f;
    }

    // Public method to pause/resume the fill cycle
    public void SetCycleActive(bool active)
    {
        isCycleActive = active;
        if (active && fillCoroutine == null)
        {
            fillCoroutine = StartCoroutine(FillStabilityOverTime());
        }
        else if (!active && fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }
    }
}