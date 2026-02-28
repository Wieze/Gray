using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    public Transform door;  
    public float openAngle = 3f;
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

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