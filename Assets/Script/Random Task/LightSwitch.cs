using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public Light bulbLight;
    public Transform player;
    public float interactDistance = 3f;
    public SanityStabilityManager sanityStabilityManager;   // Reference to combined manager

    private bool wantedState = false;          // Player's desired state (true = on, false = off)
    private bool lastLightState = false;        // For sanity drain (actual light on/off)
    private bool lastIgnoredState = false;      // For stability (ignored task condition)

    void Start()
    {
        // Initialise states based on current conditions
        lastLightState = bulbLight.enabled;
        lastIgnoredState = Generator.powerOn && !wantedState;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            // Player toggles the switch (only if power is on – otherwise the light can't change)
            if (Generator.powerOn)
            {
                wantedState = !wantedState;
            }
            else
            {
                Debug.Log("No power – cannot toggle light.");
            }
        }

        // Determine actual light state (power AND wanted)
        bool currentLightState = Generator.powerOn && wantedState;
        bulbLight.enabled = currentLightState;

        // --- Sanity drain (based on actual light on/off) ---
        if (currentLightState != lastLightState)
        {
            if (currentLightState) // Light turned on
                sanityStabilityManager?.StopLightDrain();
            else                   // Light turned off
                sanityStabilityManager?.StartLightDrain();

            lastLightState = currentLightState;
        }

        // --- Stability penalty (ignored task: power on + wantedState false) ---
        bool currentIgnoredState = Generator.powerOn && !wantedState;
        if (currentIgnoredState != lastIgnoredState)
        {
            if (currentIgnoredState)
                sanityStabilityManager?.AddIgnoredLight();
            else
                sanityStabilityManager?.RemoveIgnoredLight();

            lastIgnoredState = currentIgnoredState;
        }
    }
}