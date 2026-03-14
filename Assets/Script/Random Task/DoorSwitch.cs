using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    public Transform door;       // Door_Hinge
    public Transform player;     // Main Camera
    public float interactDistance = 3f;

    public float openAngle = 90f;
    public float speed = 3f;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = door.rotation;
        openRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        // Only allow interaction if near
        if (distance <= interactDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isOpen = !isOpen;
            }
        }

        // Door animation
        if (isOpen)
        {
            door.rotation = Quaternion.Slerp(door.rotation, openRotation, Time.deltaTime * speed);
        }
        else
        {
            door.rotation = Quaternion.Slerp(door.rotation, closedRotation, Time.deltaTime * speed);
        }
    }
}