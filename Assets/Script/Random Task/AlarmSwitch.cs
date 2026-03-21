using UnityEngine;

public class AlarmSwitch : MonoBehaviour
{
    public AudioSource alarmSound;
    public Transform player;
    public float interactDistance = 3f;
    public SanityStabilityManager sanityStabilityManager;   // Reference to combined manager

    private bool isPlaying = false;
    private bool lastIgnoredState = false;

    void Start()
    {
        lastIgnoredState = Generator.powerOn && isPlaying;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!Generator.powerOn)
                {
                    Debug.Log("No electricity. Alarm won't work.");
                    return;
                }

                ToggleAlarm();
            }
        }

        // If power goes off while alarm is playing, stop it and update states
        if (!Generator.powerOn && alarmSound.isPlaying)
        {
            alarmSound.Stop();
            isPlaying = false;

            // Stop sanity drain
            if (sanityStabilityManager != null)
                sanityStabilityManager.StopAlarmDrain();

            // Stability ignored state will be updated below automatically
        }

        // Determine if this alarm is currently an "ignored task" (on due to player choice)
        bool currentIgnoredState = Generator.powerOn && isPlaying;

        // Check if ignored state changed
        if (currentIgnoredState != lastIgnoredState)
        {
            if (currentIgnoredState)
                sanityStabilityManager?.AddIgnoredAlarm();
            else
                sanityStabilityManager?.RemoveIgnoredAlarm();

            lastIgnoredState = currentIgnoredState;
        }
    }

    void ToggleAlarm()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
        {
            alarmSound.Play();
            if (sanityStabilityManager != null)
                sanityStabilityManager.StartAlarmDrain();
        }
        else
        {
            alarmSound.Stop();
            if (sanityStabilityManager != null)
                sanityStabilityManager.StopAlarmDrain();
        }
    }
}