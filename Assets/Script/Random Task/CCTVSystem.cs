using UnityEngine;

public class CCTVSystem : MonoBehaviour
{
    public Camera playerCamera;
    public Camera[] cctvCameras;

    public Transform player;
    public float interactDistance = 3f;

    private bool usingComputer = false;
    private int currentCamera = 0;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (!Generator.powerOn)
        {
            if (usingComputer)
                ExitCCTV();

            return;
        }

        if (!usingComputer && distance <= interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            EnterCCTV();
        }

        if (usingComputer && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCCTV();
        }

        if (usingComputer)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SwitchCamera(0);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                SwitchCamera(1);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                SwitchCamera(2);
        }
    }

    void EnterCCTV()
    {
        usingComputer = true;

        playerCamera.enabled = false;

        SwitchCamera(0);
    }

    void ExitCCTV()
    {
        usingComputer = false;

        foreach (Camera cam in cctvCameras)
            cam.enabled = false;

        playerCamera.enabled = true;
    }

    void SwitchCamera(int index)
    {
        for (int i = 0; i < cctvCameras.Length; i++)
        {
            cctvCameras[i].enabled = false;
        }

        cctvCameras[index].enabled = true;
        currentCamera = index;
    }
}