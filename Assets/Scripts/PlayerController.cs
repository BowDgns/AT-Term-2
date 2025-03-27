using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float touchSensitivity = 0.1f;
    public GameObject bobberPrefab; // The bobber prefab to instantiate
    private GameObject currentBobber; // The current bobber placed in the world

    private CharacterController characterController;
    private Transform cameraTransform;
    private float xRotation = 0f;
    private Vector2 touchDelta;
    private Vector2 lastTouchPosition;
    private bool isTouching = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the game window
    }

    void Update()
    {
        MovePlayer();
        LookAround();
        HandleScreenTap();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    void LookAround()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isTouching = true;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                touchDelta = touch.position - lastTouchPosition;
                lastTouchPosition = touch.position;

                float mouseX = touchDelta.x * touchSensitivity;
                float mouseY = touchDelta.y * touchSensitivity;

                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                transform.Rotate(Vector3.up * mouseX);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isTouching = false;
            }
        }
    }

    void HandleScreenTap()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // If a bobber already exists, try to catch fish and remove the bobber.
            if (currentBobber != null)
            {
                // Find the FishCatcher in the scene.
                FishCatcher catcher = FindObjectOfType<FishCatcher>();
                if (catcher != null)
                {
                    catcher.TryCatchFishAtBobber(currentBobber.transform);
                }
                // Despawn the bobber.
                Destroy(currentBobber);
                currentBobber = null;
            }
            else
            {
                // If no bobber exists, instantiate a new one at the tap position.
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    currentBobber = Instantiate(bobberPrefab, hit.point, Quaternion.identity);
                    // Optionally, set its tag if needed:
                    currentBobber.tag = "Bobber";
                }
            }
        }
    }
}
