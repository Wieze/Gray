using UnityEngine;

public class AlarmSwitch : MonoBehaviour
{
    public AudioSource alarmSound;
    public Transform player;
    public float interactDistance = 3f;

    private bool isPlaying = false;

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

        if (!Generator.powerOn && alarmSound.isPlaying)
        {
            alarmSound.Stop();
            isPlaying = false;
        }
    }

    void ToggleAlarm()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
            alarmSound.Play();
        else
            alarmSound.Stop();
    }
}