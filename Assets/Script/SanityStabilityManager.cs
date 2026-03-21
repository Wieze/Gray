using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Events;

public class SanityStabilityManager : MonoBehaviour
{
    [Header("----- SANITY SETTINGS -----")]
    public Slider sanitySlider;
    public PostProcessProfile profile;
    private Vignette vignette;
    public int fullSanity = 100;
    public float baseSanityDrain = 5f;           // per second near anomaly
    public float alarmDrainRate = 2f;
    public float lightDrainRate = 1f;
    public int difficulty = 1;

    [Header("----- STABILITY SETTINGS -----")]
    public Slider stabilitySlider;
    public float maxStability = 100f;
    public float baseStabilityFill = 5f;          // per second
    public float lightOffPenalty = 2f;             // per ignored light
    public float alarmOnPenalty = 3f;               // per ignored alarm
    public AnimationCurve sanityMultiplier = AnimationCurve.EaseInOut(0f, 3f, 1f, 1f);

    [Header("----- GAME REFERENCES -----")]
    public WinLoose winLooseScript;
    public TimerScript timerScript;

    [Header("----- EVENTS -----")]
    public UnityEvent onInsane;                    // triggered when sanity reaches 0
    public System.Action OnStabilityCycleCompleted; // for spawner

    // Private state
    private bool isDead = false;
    private int anomaliesNear = 0;
    private int ignoredLights = 0;
    private int ignoredAlarms = 0;
    private Coroutine sanityAnomalyCoroutine;
    private Coroutine alarmCoroutine;
    private Coroutine lightCoroutine;
    private Coroutine stabilityCoroutine;
    private bool stabilityCycleActive = true;

    void Start()
    {
        // Auto-find game references
        if (winLooseScript == null) winLooseScript = FindObjectOfType<WinLoose>();
        if (timerScript == null) timerScript = FindObjectOfType<TimerScript>();

        // Setup sanity slider & vignette
        if (sanitySlider == null) sanitySlider = GetComponent<Slider>();
        if (sanitySlider != null)
        {
            sanitySlider.maxValue = fullSanity;
            sanitySlider.value = fullSanity;
        }
        if (profile != null && profile.TryGetSettings(out vignette))
            vignette.intensity.value = 0f;

        // Setup stability slider
        if (stabilitySlider == null) stabilitySlider = GetComponent<Slider>();
        if (stabilitySlider != null)
        {
            stabilitySlider.maxValue = maxStability;
            stabilitySlider.value = 0f;
        }

        // Start stability fill
        if (stabilityCycleActive && stabilityCoroutine == null)
            stabilityCoroutine = StartCoroutine(FillStability());
    }

    void Update()
    {
        // Update vignette based on sanity
        if (vignette != null && sanitySlider != null)
        {
            float percentLost = 1f - (sanitySlider.value / sanitySlider.maxValue);
            vignette.intensity.value = percentLost;
        }

        // Safety: if sanity hits zero from external drains
        if (!isDead && sanitySlider != null && sanitySlider.value <= 0)
            TriggerLose();
    }

    // ==================== SANITY METHODS ====================

    public void StartAnomalyDrain()
    {
        anomaliesNear++;
        if (sanityAnomalyCoroutine == null && sanitySlider.value > 0)
            sanityAnomalyCoroutine = StartCoroutine(AnomalyDrain());
    }

    public void StopAnomalyDrain()
    {
        if (anomaliesNear > 0) anomaliesNear--;
        if (anomaliesNear == 0 && sanityAnomalyCoroutine != null)
        {
            StopCoroutine(sanityAnomalyCoroutine);
            sanityAnomalyCoroutine = null;
        }
    }

    private IEnumerator AnomalyDrain()
    {
        while (anomaliesNear > 0 && sanitySlider.value > 0)
        {
            sanitySlider.value -= baseSanityDrain * difficulty * Time.deltaTime;
            yield return null;
        }
        sanityAnomalyCoroutine = null;
        if (sanitySlider.value <= 0 && !isDead) TriggerLose();
    }

    public void StartAlarmDrain()
    {
        if (alarmCoroutine != null) return;
        alarmCoroutine = StartCoroutine(AlarmDrain());
    }

    public void StopAlarmDrain()
    {
        if (alarmCoroutine != null)
        {
            StopCoroutine(alarmCoroutine);
            alarmCoroutine = null;
        }
    }

    private IEnumerator AlarmDrain()
    {
        while (sanitySlider.value > 0)
        {
            sanitySlider.value -= alarmDrainRate * Time.deltaTime;
            yield return null;
        }
        alarmCoroutine = null;
        if (!isDead) TriggerLose();
    }

    public void StartLightDrain()
    {
        if (lightCoroutine != null) return;
        lightCoroutine = StartCoroutine(LightDrain());
    }

    public void StopLightDrain()
    {
        if (lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
            lightCoroutine = null;
        }
    }

    private IEnumerator LightDrain()
    {
        while (sanitySlider.value > 0)
        {
            sanitySlider.value -= lightDrainRate * Time.deltaTime;
            yield return null;
        }
        lightCoroutine = null;
        if (!isDead) TriggerLose();
    }

    // ==================== STABILITY METHODS ====================

    public void AddIgnoredLight() => ignoredLights++;
    public void RemoveIgnoredLight() { if (ignoredLights > 0) ignoredLights--; }
    public void AddIgnoredAlarm() => ignoredAlarms++;
    public void RemoveIgnoredAlarm() { if (ignoredAlarms > 0) ignoredAlarms--; }

    private IEnumerator FillStability()
    {
        while (stabilityCycleActive)
        {
            if (stabilitySlider != null)
            {
                // Sanity multiplier
                float sanityPercent = sanitySlider.value / fullSanity;
                float mult = sanityMultiplier.Evaluate(sanityPercent);

                // Base + penalties
                float rawRate = baseStabilityFill 
                              + (ignoredLights * lightOffPenalty) 
                              + (ignoredAlarms * alarmOnPenalty);

                float currentRate = rawRate * mult;

                if (stabilitySlider.value < stabilitySlider.maxValue)
                {
                    stabilitySlider.value += currentRate * Time.deltaTime;
                }
                else
                {
                    OnStabilityCycleCompleted?.Invoke();
                    stabilitySlider.value = 0f;
                }
            }
            yield return null;
        }
        stabilityCoroutine = null;
    }

    public void PauseStability(bool pause)
    {
        stabilityCycleActive = !pause;
        if (!pause && stabilityCoroutine == null)
            stabilityCoroutine = StartCoroutine(FillStability());
        else if (pause && stabilityCoroutine != null)
        {
            StopCoroutine(stabilityCoroutine);
            stabilityCoroutine = null;
        }
    }

    // ==================== COMMON / LOSE METHODS ====================

    private void TriggerLose()
    {
        if (isDead) return;
        isDead = true;

        timerScript?.StopTimer();
        if (winLooseScript != null)
            winLooseScript.LoseLevel("You went insane...");
        else
            Debug.LogError("WinLoose missing!");

        onInsane.Invoke();
    }

    // Public sanity modifiers
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

    // Public stability modifiers
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

    void OnApplicationQuit()
    {
        if (vignette != null) vignette.intensity.value = 0f;
    }
}