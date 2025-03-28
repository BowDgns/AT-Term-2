using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public float swimSpeed = 2f;
    public float detectionRange = 3f;
    public float turnSpeed = 2f;

    private Vector3 swimDirection;
    private Transform bobber; // Reference to the bobber
    private bool isFishAttracted = false;

    void Start()
    {
        SetNewSwimDirection();
        StartCoroutine(ChangeDirectionRoutine());
    }

    void Update()
    {
        // Try to find the bobber if it hasn't been set yet.
        if (bobber == null)
        {
            FindBobber();
        }

        if (bobber != null && Vector3.Distance(transform.position, bobber.position) < detectionRange)
        {
            isFishAttracted = true;
            swimSpeed = 0.7f;
            // Calculate a direction towards the bobber
            Vector3 targetDirection = (bobber.position - transform.position).normalized;
            swimDirection = Vector3.Lerp(swimDirection, targetDirection, turnSpeed * Time.deltaTime);
        }

        // Move the fish regardless of attraction state
        transform.position += swimDirection * swimSpeed * Time.deltaTime;

        // Rotate the fish to face its direction
        if (swimDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(swimDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90f, targetRotation.eulerAngles.y + 180f, 0f), turnSpeed * Time.deltaTime);
        }
    }


    void SetNewSwimDirection()
    {
        // Pick a random swim direction
        swimDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (!isFishAttracted)
        {
            yield return new WaitForSeconds(Random.Range(5f, 10f));
            SetNewSwimDirection();
        }
    }

    // Function to find the bobber in the scene if not already set
    void FindBobber()
    {
        // Attempt to find the most recently placed bobber in the scene
        bobber = GameObject.FindWithTag("Bobber")?.transform;

        // If you have a list of bobbers and want to pick the most recent one, you could iterate through them
        // For now, we'll assume only one bobber exists in the scene
    }
}
