using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public float swimSpeed = 2f;
    public float detectionRange = 3f;
    public float turnSpeed = 2f;

    private Vector3 swimDirection;

    private Transform bobber;
    private bool isFishAttracted = false;

    void Start()
    {
        SetNewSwimDirection();
        StartCoroutine(ChangeDirectionRoutine());
    }

    void Update()
    {
        // fish should be checking to see if the bobber is nearby at all times
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


        // make fish move around in the water if there is no bobber in range
        transform.position += swimDirection * swimSpeed * Time.deltaTime;

        // make fish face the way its swimming
        if (swimDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(swimDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90f, targetRotation.eulerAngles.y + 180f, 0f), turnSpeed * Time.deltaTime);
        }
    }


    void SetNewSwimDirection()
    {
        // change direction randomly
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

    // as bobbers are prefabs and not already in the scene, attempt to find the bobber when its thrown in the scene
    void FindBobber()
    {
        bobber = GameObject.FindWithTag("Bobber")?.transform;
    }
}
