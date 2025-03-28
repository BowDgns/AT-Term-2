using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;  // Needed for UI touch detection

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float touchSensitivity = 0.1f;
    public GameObject bobberPrefab; // Prefab for the bobber
    private GameObject currentBobber; // Currently active bobber

    private CharacterController characterController;
    private Transform cameraTransform;
    private float xRotation = 0f;

    // For touch handling on the game area (excluding UI)
    private Vector2 touchStartPos;
    private bool isSwiping = false;
    public float swipeThreshold = 10f; // Minimum movement (in pixels) to consider as a swipe

    // Reference to the UI Joystick (assign in Inspector)
    public Joystick joystick;
    public float speed = 5f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MovePlayer();
        ProcessTouch();
    }

    // Moves the player based on joystick input relative to the camera's facing direction.
    void MovePlayer()
    {
        // Get the raw joystick input
        Vector3 input = new Vector3(joystick.Horizontal(), 0f, joystick.Vertical());

        // Get camera's forward and right vectors
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Flatten the vectors to ignore any vertical component
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Combine the input with camera directions to get the movement direction
        Vector3 moveDirection = forward * input.z + right * input.x;

        // Update the player's position
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    // Processes touches that occur outside UI elements.
    void ProcessTouch()
    {
        if (Input.touchCount > 0)
        {
            // Process the first touch that's not over a UI element.
            foreach (Touch touch in Input.touches)
            {
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    ProcessGameTouch(touch);
                    break; // Only process one touch per frame.
                }
            }
        }
    }

    // Handles a touch on the game area.
    void ProcessGameTouch(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            touchStartPos = touch.position;
            isSwiping = false;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            // Only process swipe for camera rotation if the touch started in the top 2/3 of the screen.
            if (touchStartPos.y > Screen.height / 3f)
            {
                if ((touch.position - touchStartPos).magnitude > swipeThreshold)
                {
                    isSwiping = true;
                    Vector2 touchDelta = touch.deltaPosition;
                    float mouseX = touchDelta.x * touchSensitivity;
                    float mouseY = touchDelta.y * touchSensitivity;

                    // Rotate the camera (pitch) and the player (yaw)
                    xRotation -= mouseY;
                    xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                    cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                    transform.Rotate(Vector3.up * mouseX);
                }
            }
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            // If the finger did not move enough to count as a swipe, consider it a tap.
            if (!isSwiping)
            {
                PlaceOrCatchBobber(touch);
            }
        }
    }

    // Places a new bobber at the tapped position or, if one already exists, attempts to catch fish.
    // Bobber placement is processed from any point on the screen as long as the hit object is tagged "Water".
    void PlaceOrCatchBobber(Touch touch)
    {
        if (currentBobber != null)
        {
            // If a bobber already exists, try to catch fish.
            FishCatcher catcher = FindObjectOfType<FishCatcher>();
            if (catcher != null)
            {
                catcher.TryCatchFishAtBobber(currentBobber.transform);
            }
            Destroy(currentBobber);
            currentBobber = null;
        }
        else
        {
            // Raycast from the tapped screen position into the world.
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Only place the bobber if the hit object is tagged "Water".
                if (hit.collider.CompareTag("Water"))
                {
                    currentBobber = Instantiate(bobberPrefab, hit.point, Quaternion.identity);
                    currentBobber.tag = "Bobber";
                }
            }
        }
    }
}
