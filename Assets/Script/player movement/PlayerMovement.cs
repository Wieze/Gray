using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    // Audio Settings
    public AudioClip footstepSound;
    public AudioSource externalAudioSource; // Optional: assign an external AudioSource
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private AudioSource audioSource;
    private Coroutine footstepCoroutine;

    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Setup AudioSource
        SetupAudioSource();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Debug check
        if (footstepSound == null)
        {
            Debug.LogError("Footstep sound is not assigned in the Inspector!");
        }
        else
        {
            Debug.Log("Footstep sound assigned: " + footstepSound.name);
        }
    }

    void SetupAudioSource()
    {
        // Try to get AudioSource from this GameObject or use external one
        audioSource = GetComponent<AudioSource>();
        
        // If no AudioSource found on this GameObject, use the external one
        if (audioSource == null && externalAudioSource != null)
        {
            audioSource = externalAudioSource;
        }
        
        // If still no AudioSource, create one
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("Created new AudioSource component on " + gameObject.name);
        }
        
        // Configure AudioSource
        if (audioSource != null)
        {
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1.0f; // 3D sound
            audioSource.volume = 0.7f;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.maxDistance = 20f;
            Debug.Log("AudioSource configured successfully");
        }
        else
        {
            Debug.LogError("Failed to setup AudioSource!");
        }
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float inputVertical = Input.GetAxis("Vertical");
        float inputHorizontal = Input.GetAxis("Horizontal");
        
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * inputVertical : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * inputHorizontal : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;

        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // Handle footstep sounds
        HandleFootsteps(inputVertical, inputHorizontal, isRunning);
    }

    void HandleFootsteps(float inputVertical, float inputHorizontal, bool isRunning)
    {
        bool isMoving = characterController.isGrounded &&
                        (Mathf.Abs(inputVertical) > 0.1f || Mathf.Abs(inputHorizontal) > 0.1f);

        if (isMoving)
        {
            if (footstepCoroutine == null)
            {
                footstepCoroutine = StartCoroutine(PlayFootsteps(isRunning));
            }
        }
        else
        {
            if (footstepCoroutine != null)
            {
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    IEnumerator PlayFootsteps(bool isRunning)
    {
        while (true)
        {
            if (footstepSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(footstepSound, 0.5f);
                Debug.Log("Footstep played");
            }
            yield return new WaitForSeconds(isRunning ? runStepInterval : walkStepInterval);
        }
    }
    
    // Optional: Add this to clean up coroutine when object is destroyed
    void OnDestroy()
    {
        if (footstepCoroutine != null)
        {
            StopCoroutine(footstepCoroutine);
        }
    }
}