using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StabilityManager : MonoBehaviour
{
    [Header("Stability Settings")]
    public Slider stabilitySlider;
    public float maxStability = 100f;
    public float baseFillRate = 5f;

    [Header("Ignored Task Penalties")]
    public float lightOffPenalty = 2f;
    public float alarmOnPenalty = 3f;

    [Header("Sanity Multiplier")]
    public SanityManager sanityManager;
    public AnimationCurve sanityMultiplierCurve = AnimationCurve.EaseInOut(0f, 3f, 1f, 1f);

    [Header("Game Integration (Optional)")]
    public WinLoose winLooseScript;
    public TimerScript timerScript;

    [Header("Pause Handling")]
    private bool isPaused = false;          // Prevents updates while paused

    public System.Action OnStabilityCycleCompleted;

    private bool isCycleActive = true;
    private Coroutine fillCoroutine;
    private int ignoredLights = 0;
    private int ignoredAlarms = 0;

    void Start()
    {
        if (winLooseScript == null) winLooseScript = FindObjectOfType<WinLoose>();
        if (timerScript == null) timerScript = FindObjectOfType<TimerScript>();
        if (sanityManager == null) sanityManager = FindObjectOfType<SanityManager>();

        if (stabilitySlider == null) stabilitySlider = GetComponent<Slider>();
        if (stabilitySlider != null)
        {
            stabilitySlider.maxValue = maxStability;
            stabilitySlider.value = 0f;
        }

        if (fillCoroutine == null && isCycleActive)
            fillCoroutine = StartCoroutine(FillStabilityOverTime());
    }

    private IEnumerator FillStabilityOverTime()
    {
        while (isCycleActive)
        {
            if (!isPaused && stabilitySlider != null)
            {
                float sanityMultiplier = 1f;
                if (sanityManager != null && sanityManager.sanitySlider != null)
                {
                    float sanityPercent = sanityManager.sanitySlider.value / sanityManager.fullSanity;
                    sanityMultiplier = sanityMultiplierCurve.Evaluate(sanityPercent);
                }

                float rawFillRate = baseFillRate 
                                  + (ignoredLights * lightOffPenalty) 
                                  + (ignoredAlarms * alarmOnPenalty);

                float currentFillRate = rawFillRate * sanityMultiplier;

                if (stabilitySlider.value < stabilitySlider.maxValue)
                {
                    stabilitySlider.value += currentFillRate * Time.deltaTime;
                }
                else
                {
                    OnStabilityCycleCompleted?.Invoke();
                    stabilitySlider.value = 0f;
                }
            }
            yield return null;
        }
        fillCoroutine = null;
    }

    public void AddIgnoredLight() => ignoredLights++;
    public void RemoveIgnoredLight() { if (ignoredLights > 0) ignoredLights--; }
    public void AddIgnoredAlarm() => ignoredAlarms++;
    public void RemoveIgnoredAlarm() { if (ignoredAlarms > 0) ignoredAlarms--; }

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
        if (stabilitySlider != null) stabilitySlider.value = 0f;
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

    // --- Pause handling ---
    public void PauseStability()
    {
        isPaused = true;
    }

    public void ResumeStability()
    {
        isPaused = false;
    }
}