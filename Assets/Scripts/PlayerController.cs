using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float touchSensitivity = 0.1f;
    public GameObject bobberPrefab; // Prefab for the bobber.
    private GameObject currentBobber; // Currently active bobber.

    private CharacterController characterController;
    private Transform cameraTransform;
    private float xRotation = 0f;

    // Touch handling for the game area.
    private Vector2 touchStartPos;
    private bool isSwiping = false;
    public float swipeThreshold = 10f; // Minimum movement to consider as a swipe.

    // Reference to the UI Joystick (assign in Inspector).
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

    // Moves the player based on joystick input relative to the camera’s facing direction.
    void MovePlayer()
    {
        Vector3 input = new Vector3(joystick.Horizontal(), 0f, joystick.Vertical());
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Flatten to ignore vertical components.
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * input.z + right * input.x;
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    // Processes touches that occur outside UI elements.
    void ProcessTouch()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    ProcessGameTouch(touch);
                    break;
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
            // Prevent camera rotation if a bobber animation is active.
            if (currentBobber != null)
            {
                BobberController bc = currentBobber.GetComponent<BobberController>();
                if (bc != null && bc.isAnimating)
                    return;
            }

            // Process swipe for camera rotation only if the touch started in the top 2/3 of the screen.
            if (touchStartPos.y > Screen.height / 3f)
            {
                if ((touch.position - touchStartPos).magnitude > swipeThreshold)
                {
                    isSwiping = true;
                    Vector2 touchDelta = touch.deltaPosition;
                    float mouseX = touchDelta.x * touchSensitivity;
                    float mouseY = touchDelta.y * touchSensitivity;
                    xRotation -= mouseY;
                    xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                    cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                    transform.Rotate(Vector3.up * mouseX);
                }
            }
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            // If not swiping, treat as a tap.
            if (!isSwiping)
            {
                PlaceOrCatchBobber(touch);
            }
        }
    }

    // Places a new bobber (or catches fish if one already exists).
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
            // Capture the player's current position as the reel target.
            Vector3 reelTarget = Camera.main.transform.position + new Vector3(0, 5.56f, 0);
            BobberController bobberController = currentBobber.GetComponent<BobberController>();
            if (bobberController != null)
            {
                StartCoroutine(ReelAndDestroyBobber(bobberController, reelTarget));
            }
            else
            {
                Destroy(currentBobber);
                currentBobber = null;
            }
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
                    // Instantiate the bobber at the player's position.
                    currentBobber = Instantiate(bobberPrefab, transform.position, Quaternion.identity);
                    currentBobber.tag = "Bobber";

                    BobberController bobberController = currentBobber.GetComponent<BobberController>();
                    if (bobberController != null)
                    {
                        // Animate the bobber from the player to the water hit point.
                        StartCoroutine(bobberController.ThrowBobber(transform.position, hit.point));
                    }
                }
            }
        }
    }

    // Reels the bobber in toward the captured player position and destroys it afterward.
    IEnumerator ReelAndDestroyBobber(BobberController bobberController, Vector3 reelTarget)
    {
        yield return StartCoroutine(bobberController.ReelBobber(reelTarget));
        Destroy(currentBobber);
        currentBobber = null;
    }
}
