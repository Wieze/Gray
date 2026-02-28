using UnityEngine;

public class Generator : MonoBehaviour
{
    public static bool powerOn = true; 

    public Transform player;
    public float interactDistance = 3f;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                powerOn = !powerOn; 

                if (powerOn)
                    Debug.Log("Generator ON");
                else
                    Debug.Log("Generator OFF");
            }
        }
    }
}