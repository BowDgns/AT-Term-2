using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float touchSensitivity = 0.1f;
    public GameObject bobberPrefab; // bobber
    private GameObject currentBobber;

    private CharacterController characterController;
    private Transform cameraTransform;
    private float xRotation = 0f;

    // maybe make it so you cant throw the bobber when ur using the joystick so like lock it to the top half of the screen
    private Vector2 touchStartPos;
    private bool isSwiping = false;
    public float swipeThreshold = 10f; // amount to move it to consider the touch a swipe to turn around

    
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

    void MovePlayer()
    {
        Vector3 input = new Vector3(joystick.Horizontal(), 0f, joystick.Vertical());
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * input.z + right * input.x;
        transform.position += moveDirection * speed * Time.deltaTime;
    }

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

    // make sure bobber can be thrown in a certain part of the screen
    void ProcessGameTouch(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            touchStartPos = touch.position;
            isSwiping = false;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            if (currentBobber != null)
            {
                BobberController bc = currentBobber.GetComponent<BobberController>();
                if (bc != null && bc.isAnimating)
                    return;
            }

            // make sure joystick doesnt interfere with looking around
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
            // not swipe so trying to throw bobber
            if (!isSwiping)
            {
                PlaceOrCatchBobber(touch);
            }
        }
    }

    // place bobber / reel
    void PlaceOrCatchBobber(Touch touch)
    {
        if (currentBobber != null)
        {
            // if theres a bobber reel in fish
            FishCatcher catcher = FindObjectOfType<FishCatcher>();
            if (catcher != null)
            {
                catcher.TryCatchFishAtBobber(currentBobber.transform);
            }
            // get player position to reel bobber towards it
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
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // only place bobbers on water
                if (hit.collider.CompareTag("Water"))
                {
                    currentBobber = Instantiate(bobberPrefab, transform.position, Quaternion.identity);
                    currentBobber.tag = "Bobber";

                    BobberController bobberController = currentBobber.GetComponent<BobberController>();
                    if (bobberController != null)
                    {
                        // animate bobber
                        StartCoroutine(bobberController.ThrowBobber(transform.position, hit.point));
                    }
                }
            }
        }
    }

    // reeling
    IEnumerator ReelAndDestroyBobber(BobberController bobberController, Vector3 reelTarget)
    {
        yield return StartCoroutine(bobberController.ReelBobber(reelTarget));
        Destroy(currentBobber);
        currentBobber = null;
    }
}
