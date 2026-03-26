using UnityEngine;

public class AlarmSwitch : MonoBehaviour
{
    public AudioSource alarmSound;
    public Transform player;
    public float interactDistance = 3f;
    public SanityStabilityManager sanityStabilityManager;

    private bool isPlaying = false;
    private bool lastIgnoredState = false;

    private SystemBreak systemBreak;

    void Start()
    {
        systemBreak = GetComponent<SystemBreak>();
        lastIgnoredState = Generator.powerOn && isPlaying;
    }

    void Update()
    {
        // 🔴 If broken → stop everything
        if (systemBreak != null && systemBreak.isBroken)
        {
            if (alarmSound.isPlaying)
                alarmSound.Stop();

            isPlaying = false;
            return;
        }

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

        // ⚡ Power OFF
        if (!Generator.powerOn && alarmSound.isPlaying)
        {
            alarmSound.Stop();
            isPlaying = false;

            sanityStabilityManager?.StopAlarmDrain();
        }

        bool currentIgnoredState = Generator.powerOn && isPlaying;

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
            sanityStabilityManager?.StartAlarmDrain();
        }
        else
        {
            alarmSound.Stop();
            sanityStabilityManager?.StopAlarmDrain();
        }
    }

    // 😈 GLITCH EFFECT
    public void GlitchSound()
    {
        if (alarmSound != null)
        {
            alarmSound.pitch = Random.Range(0.5f, 1.5f);
        }
    }
}