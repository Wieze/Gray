
using UnityEngine;

public class ComPac : MonoBehaviour
{
    public GameObject[] cctvCameras;
    public GameObject playerCamera;

    private int currentIndex = 0;
    private bool isPlayerNearby = false;
    private bool isUsingComputer = false;

    void Update()
    {
        //Enter CCTV
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            EnterCCTV();
        }

        //Exit CCTV
        if (isUsingComputer && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCCTV();
        }

        //Switch cameras ONLY when using computer
        if (!isUsingComputer) return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextCamera();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousCamera();
        }
    }

    void EnterCCTV()
    {
        isUsingComputer = true;
        playerCamera.SetActive(false);
        currentIndex = 0;
        ActivateCamera(currentIndex);
    }

    void ExitCCTV()
    {
        isUsingComputer = false;
        playerCamera.SetActive(true);

        //Turn off all CCTV cameras
        
        foreach (GameObject cam in cctvCameras)
        {
            cam.SetActive(false);
        }
    }

    void NextCamera()
    {
        currentIndex = (currentIndex + 1) % cctvCameras.Length;
        ActivateCamera(currentIndex);
    }

    void PreviousCamera()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = cctvCameras.Length - 1;

        ActivateCamera(currentIndex);
    }

    void ActivateCamera(int index)
    {
        for (int i = 0; i < cctvCameras.Length; i++)
        {
            cctvCameras[i].SetActive(i == index);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
