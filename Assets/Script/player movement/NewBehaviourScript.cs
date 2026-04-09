using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
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

    [Header("Audio Settings")]
    public AudioClip footstepSound;
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;
    [Range(0, 1)]
    public float footstepVolume = 0.7f;

    // Private variables
    private CharacterController characterController;
    private AudioSource audioSource;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private bool canMove = true;
    
    // Footstep control
    private Coroutine footstepCoroutine;
    private bool isMoving = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        SetupAudioSource();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Validate audio clip
        if (footstepSound == null)
        {
            Debug.LogError("Footstep sound is not assigned in the Inspector!");
        }
    }

    void SetupAudioSource()
    {
        // Try to get existing AudioSource
        audioSource = GetComponent<AudioSource>();
        
        // If no AudioSource exists, create one
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("Created new AudioSource for footstep sounds");
        }
        
        // Configure AudioSource for footsteps
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 3D sound
        audioSource.volume = footstepVolume;
    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleFootsteps();
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float inputVertical = Input.GetAxis("Vertical");
        float inputHorizontal = Input.GetAxis("Horizontal");
        
        // Calculate movement speed
        float currentSpeed = canMove ? (isRunning ? runSpeed : walkSpeed) : 0;
        float curSpeedX = currentSpeed * inputVertical;
        float curSpeedY = currentSpeed * inputHorizontal;
        
        // Store vertical movement for gravity
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        
        // Handle jumping
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        
        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        
        // Handle crouching
        if (Input.GetKey(KeyCode.LeftControl) && canMove)
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
        
        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleCamera()
    {
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    void HandleFootsteps()
    {
        // Check if player is moving on ground
        float inputVertical = Input.GetAxis("Vertical");
        float inputHorizontal = Input.GetAxis("Horizontal");
        bool isMovingOnGround = characterController.isGrounded && 
                                (Mathf.Abs(inputVertical) > 0.1f || Mathf.Abs(inputHorizontal) > 0.1f);
        
        // Handle footstep coroutine based on movement state
        if (isMovingOnGround && !isMoving)
        {
            // Player started moving - start footsteps
            isMoving = true;
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            StartFootsteps(isRunning);
            Debug.Log("Started walking - footsteps playing");
        }
        else if (!isMovingOnGround && isMoving)
        {
            // Player stopped moving - stop footsteps immediately
            isMoving = false;
            StopFootsteps();
            Debug.Log("Stopped moving - footsteps stopped");
        }
        else if (isMovingOnGround && isMoving)
        {
            // Player is moving - check if running state changed
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            // Optional: Update footstep interval if running state changed
            // This is handled automatically in the coroutine if you pass isRunning parameter
        }
    }

    void StartFootsteps(bool isRunning)
    {
        // Stop any existing footstep coroutine
        if (footstepCoroutine != null)
        {
            StopCoroutine(footstepCoroutine);
        }
        
        // Start new footstep coroutine
        footstepCoroutine = StartCoroutine(PlayFootsteps(isRunning));
    }

    void StopFootsteps()
    {
        // Stop the coroutine
        if (footstepCoroutine != null)
        {
            StopCoroutine(footstepCoroutine);
            footstepCoroutine = null;
        }
        
        // Stop any playing audio
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    IEnumerator PlayFootsteps(bool isRunning)
    {
        while (true)
        {
            // Check if we can play footstep
            if (footstepSound != null && audioSource != null && isMoving)
            {
                audioSource.PlayOneShot(footstepSound, footstepVolume);
            }
            
            // Wait based on running state
            float interval = isRunning ? runStepInterval : walkStepInterval;
            yield return new WaitForSeconds(interval);
        }
    }
    
    // Clean up when destroyed
    void OnDestroy()
    {
        StopFootsteps();
    }
    
    // Optional: Handle when the game is paused or stopped
    void OnDisable()
    {
        StopFootsteps();
    }
}