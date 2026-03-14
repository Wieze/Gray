using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StabilityManager : MonoBehaviour
{
    [Header("Stability Settings")]
    public Slider stabilitySlider;
    public float maxStability = 100f;          // Maximum stability value
    public float baseFillRate = 5f;            // Base stability added per second

    [Header("Ignored Task Penalties")]
    public float lightOffPenalty = 2f;          // Extra fill rate per light off
    public float alarmOnPenalty = 3f;            // Extra fill rate per alarm on

    [Header("Sanity Multiplier")]
    public SanityManager sanityManager;          // Reference to SanityManager
    public AnimationCurve sanityMultiplierCurve = AnimationCurve.EaseInOut(0f, 3f, 1f, 1f);
    // Curve maps sanity percent (0 = empty, 1 = full) to multiplier.
    // Default: at 0% sanity → multiplier 3, at 100% sanity → multiplier 1.

    [Header("Game Integration (Optional)")]
    public WinLoose winLooseScript;
    public TimerScript timerScript;

    // Event triggered when the stability bar completes a cycle (reaches max and resets)
    public System.Action OnStabilityCycleCompleted;

    private bool isCycleActive = true;
    private Coroutine fillCoroutine;
    private int ignoredLights = 0;
    private int ignoredAlarms = 0;

    void Start()
    {
        if (winLooseScript == null)
            winLooseScript = FindObjectOfType<WinLoose>();
        if (timerScript == null)
            timerScript = FindObjectOfType<TimerScript>();

        // Auto-find SanityManager if not assigned
        if (sanityManager == null)
            sanityManager = FindObjectOfType<SanityManager>();

        // Setup slider
        if (stabilitySlider == null)
            stabilitySlider = GetComponent<Slider>();

        if (stabilitySlider != null)
        {
            stabilitySlider.maxValue = maxStability;
            stabilitySlider.value = 0f;
        }

        // Start the fill coroutine
        if (fillCoroutine == null && isCycleActive)
        {
            fillCoroutine = StartCoroutine(FillStabilityOverTime());
        }
    }

    private IEnumerator FillStabilityOverTime()
    {
        while (isCycleActive)
        {
            if (stabilitySlider != null)
            {
                // --- Calculate sanity multiplier ---
                float sanityMultiplier = 1f;
                if (sanityManager != null && sanityManager.sanitySlider != null)
                {
                    float sanityPercent = sanityManager.sanitySlider.value / sanityManager.fullSanity;
                    sanityMultiplier = sanityMultiplierCurve.Evaluate(sanityPercent);
                }

                // Base fill rate with penalties (additive)
                float rawFillRate = baseFillRate 
                                  + (ignoredLights * lightOffPenalty) 
                                  + (ignoredAlarms * alarmOnPenalty);

                // Apply sanity multiplier
                float currentFillRate = rawFillRate * sanityMultiplier;

                // Fill the bar
                if (stabilitySlider.value < stabilitySlider.maxValue)
                {
                    stabilitySlider.value += currentFillRate * Time.deltaTime;
                }
                else
                {
                    OnStabilityCycleCompleted?.Invoke();
                    stabilitySlider.value = 0f; // always reset for this game design
                }
            }
            yield return null;
        }
        fillCoroutine = null;
    }

    // Called by LightSwitch when a light becomes ignored (off due to player choice)
    public void AddIgnoredLight()
    {
        ignoredLights++;
    }

    // Called by LightSwitch when a light is no longer ignored
    public void RemoveIgnoredLight()
    {
        if (ignoredLights > 0) ignoredLights--;
    }

    // Called by AlarmSwitch when an alarm becomes ignored (on due to player choice)
    public void AddIgnoredAlarm()
    {
        ignoredAlarms++;
    }

    // Called by AlarmSwitch when an alarm is no longer ignored
    public void RemoveIgnoredAlarm()
    {
        if (ignoredAlarms > 0) ignoredAlarms--;
    }

    // Public methods to manually adjust stability
    public void AddStability(float amount)
    {
        if (stabilitySlider != null)
            stabilitySlider.value = Mathf.Clamp(stabilitySlider.value + amount, 0, maxStability);
    }

    public void ReduceStability(float amount)
    {
        if (stabilitySlider != null)
            stabilitySlider.value = Mathf.Clamp(stabilitySlider.value - amount, 0, maxStability);
    }

    public void ResetStability()
    {
        if (stabilitySlider != null)
            stabilitySlider.value = 0f;
    }

    public void SetCycleActive(bool active)
    {
        isCycleActive = active;
        if (active && fillCoroutine == null)
            fillCoroutine = StartCoroutine(FillStabilityOverTime());
        else if (!active && fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
            fillCoroutine = null;
        }
    }
}