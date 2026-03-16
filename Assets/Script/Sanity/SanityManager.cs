using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Events;

public class SanityManager : MonoBehaviour
{
    public Slider sanitySlider;
    public PostProcessProfile profile;
    private Vignette vignette;

    public int fullSanity = 100;
    public float drainRatePerSecond = 5f;        // How much sanity is lost per second when near an anomaly
    public int difficulty = 1;                    // Multiplier for sanity drain rate (if needed)

    [Header("Alarm Drain")]
    public float alarmDrainRate = 2f;             // Sanity lost per second when alarm is on
    private bool alarmActive = false;
    private Coroutine alarmCoroutine;

    [Header("Light Drain")]
    public float lightDrainRate = 1f;              // Sanity lost per second when lights are off
    private bool lightOffActive = false;
    private Coroutine lightCoroutine;

    [Header("Lose Condition")]
    public WinLoose winLooseScript;
    public TimerScript timerScript;

    [Header("Events")]
    public UnityEvent onInsane;                   // Alternative to direct WinLoose call (optional)

    private bool isDead = false;
    private int anomaliesNear = 0;                 // Number of anomaly triggers the player is inside
    private Coroutine drainCoroutine;

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
    }

    void Update()
    {
        // Continuously update vignette intensity based on current sanity
        if (vignette != null && sanitySlider != null)
        {
            float percentLost = 1f - (sanitySlider.value / sanitySlider.maxValue);
            vignette.intensity.value = percentLost;
        }

        // Safety check: if sanity reached zero (e.g., from external drains), trigger lose condition
        if (!isDead && sanitySlider != null && sanitySlider.value <= 0)
        {
            TriggerLose();
        }
    }

    /// <summary>
    /// Called when player enters an anomaly trigger.
    /// </summary>
    public void StartAnomalyDrain()
    {
        anomaliesNear++;
        if (drainCoroutine == null && sanitySlider != null && sanitySlider.value > 0)
        {
            drainCoroutine = StartCoroutine(DrainSanityNearAnomaly());
        }
    }

    /// <summary>
    /// Called when player exits an anomaly trigger.
    /// </summary>
    public void StopAnomalyDrain()
    {
        if (anomaliesNear > 0)
            anomaliesNear--;

        if (anomaliesNear == 0 && drainCoroutine != null)
        {
            StopCoroutine(drainCoroutine);
            drainCoroutine = null;
        }
    }

    private IEnumerator DrainSanityNearAnomaly()
    {
        // Continue draining as long as the player is inside at least one anomaly and sanity > 0
        while (anomaliesNear > 0 && sanitySlider != null && sanitySlider.value > 0)
        {
            // Drain sanity over time (multiplied by difficulty if desired)
            sanitySlider.value -= drainRatePerSecond * difficulty * Time.deltaTime;
            yield return null;
        }

        // Sanity reached zero – trigger lose condition
        if (sanitySlider != null && sanitySlider.value <= 0 && !isDead)
        {
            TriggerLose();
        }

        drainCoroutine = null;
    }

    /// <summary>
    /// Called when alarm is turned on.
    /// </summary>
    public void StartAlarmDrain()
    {
        if (alarmActive) return;
        alarmActive = true;
        if (alarmCoroutine == null)
            alarmCoroutine = StartCoroutine(AlarmDrainCoroutine());
    }

    /// <summary>
    /// Called when alarm is turned off.
    /// </summary>
    public void StopAlarmDrain()
    {
        alarmActive = false;
        if (alarmCoroutine != null)
        {
            StopCoroutine(alarmCoroutine);
            alarmCoroutine = null;
        }
    }

    private IEnumerator AlarmDrainCoroutine()
    {
        while (alarmActive && sanitySlider != null && sanitySlider.value > 0)
        {
            sanitySlider.value -= alarmDrainRate * Time.deltaTime;
            yield return null;
        }
        alarmCoroutine = null;
        // If sanity hits zero, Update safety check will handle lose condition.
    }

    /// <summary>
    /// Called when lights are turned off.
    /// </summary>
    public void StartLightDrain()
    {
        if (lightOffActive) return;
        lightOffActive = true;
        if (lightCoroutine == null)
            lightCoroutine = StartCoroutine(LightDrainCoroutine());
    }

    /// <summary>
    /// Called when lights are turned on.
    /// </summary>
    public void StopLightDrain()
    {
        lightOffActive = false;
        if (lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
            lightCoroutine = null;
        }
    }

    private IEnumerator LightDrainCoroutine()
    {
        while (lightOffActive && sanitySlider != null && sanitySlider.value > 0)
        {
            sanitySlider.value -= lightDrainRate * Time.deltaTime;
            yield return null;
        }
        lightCoroutine = null;
    }

    private void TriggerLose()
    {
        isDead = true;

        // Stop the timer
        if (timerScript != null)
            timerScript.StopTimer();

        // Option 1: Direct WinLoose call
        if (winLooseScript != null)
            winLooseScript.LoseLevel("You went insane...");
        else
            Debug.LogError("WinLoose script missing! Player went insane but no lose screen.");

        // Option 2: Invoke UnityEvent (you can hook this up in Inspector instead of direct call)
        onInsane.Invoke();
    }

    // Public methods to modify sanity externally (can be used for sanity pickups, etc.)
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
        if (sanitySlider != null)
            sanitySlider.value += value;
    }


    void OnApplicationQuit()
    {
        if (profile != null)
        {
            profile.TryGetSettings(out vignette);
            if (vignette != null)
            {
                vignette.intensity.value = 0f; // start with no vignette
            }
        }
    }
}