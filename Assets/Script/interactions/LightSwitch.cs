using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public Light bulbLight;
    public Transform player;
    public float interactDistance = 3f;

    private bool wantedState = false; 

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                wantedState = !wantedState;
            }
        }

        if (Generator.powerOn)
        {
            bulbLight.enabled = wantedState;
        }
        else
        {
            bulbLight.enabled = false;
        }
    }
}