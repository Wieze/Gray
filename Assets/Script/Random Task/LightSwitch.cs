using UnityEngine;
using System.Collections;

public class LightSwitch : MonoBehaviour
{
    public Light bulbLight;

    public Transform player;
    public float interactDistance = 3f;

    public SanityStabilityManager sanityStabilityManager;

    private bool isOn = true;
    private bool lastIgnoredState = false;
    private bool isFlickering = false;

    private SystemBreak systemBreak;

    void Start()
    {
        systemBreak = GetComponent<SystemBreak>();
        lastIgnoredState = Generator.powerOn && !isOn;
    }

    void Update()
    {
        // 🔴 If broken → light OFF
        if (systemBreak != null && systemBreak.isBroken)
        {
            bulbLight.enabled = false;
            isOn = false;
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!Generator.powerOn)
                {
                    Debug.Log("No electricity. Light won't work.");
                    return;
                }

                ToggleLight();
            }
        }

        // ⚡ Generator OFF → force OFF
        if (!Generator.powerOn && bulbLight.enabled)
        {
            bulbLight.enabled = false;
            isOn = false;
        }

        // 😈 SANITY SYSTEM (light ignored = OFF while power ON)
        bool currentIgnoredState = Generator.powerOn && !isOn;

        if (currentIgnoredState != lastIgnoredState)
        {
            if (currentIgnoredState)
                sanityStabilityManager?.AddIgnoredLight();
            else
                sanityStabilityManager?.RemoveIgnoredLight();

            lastIgnoredState = currentIgnoredState;
        }
    }

    void ToggleLight()
    {
        isOn = !isOn;
        bulbLight.enabled = isOn;
    }

    // 😈 FLICKER BEFORE BREAK
    public void FlickerEffect()
    {
        if (!isFlickering)
            StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        isFlickering = true;

        for (int i = 0; i < 10; i++)
        {
            bulbLight.enabled = !bulbLight.enabled;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
        }

        bulbLight.enabled = false;
        isOn = false;

        isFlickering = false;
    }
}