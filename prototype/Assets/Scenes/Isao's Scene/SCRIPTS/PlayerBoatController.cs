using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoatController : MonoBehaviour
{
    public bool isActive = true;
    public bool isControllable = true;

    public float moveSpeed = 5f;
    public float rotationSpeed = 60f; // Degrees per second

    private float moveForward = 0.0f;
    private float moveSideways = 0.0f;

    void Update()
    {
        if (!isActive || !isControllable)
            return;

        HandleInput();
        MoveBoat();
    }

    void HandleInput()
    {
        moveForward = 0.0f;
        moveSideways = 0.0f;

        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            moveForward = -1.0f;
        }
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            moveForward = 1.0f;
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            moveSideways = -1.0f;
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            moveSideways = 1.0f;
        }
    }

private float currentAngleZ = 0.0f; // Store the boat's rotation (Z-axis)

void MoveBoat()
{
    // Forward/backward movement using the boat's current rotation (calculated from currentAngleZ)
    if (moveForward != 0.0f)
    {
        // Convert current rotation angle to radians
        float radians = currentAngleZ * Mathf.Deg2Rad;

        // Calculate direction based on current rotation
        float x = Mathf.Sin(radians);
        float z = Mathf.Cos(radians);

        // Move forward or backward along the direction the boat is facing
        Vector3 moveDirection = new Vector3(x, 0.0f, z);
        
        // Apply movement along the direction in the XZ-plane
        transform.position += moveDirection * -moveForward * moveSpeed * Time.deltaTime;
    }

    // Sideways movement (left/right) using the boat's rotation (calculated from currentAngleZ)
    if (moveSideways != 0.0f)
    {
        // Calculate the rotation amount for sideways movement
        float rotationAmount = moveSideways * rotationSpeed * Time.deltaTime;
        
        // Update the boat's current rotation based on the input
        currentAngleZ += rotationAmount;

        // Convert the updated rotation to radians for sideways movement
        float radians = currentAngleZ * Mathf.Deg2Rad;

        // Calculate sideways movement (perpendicular to the forward direction)
        float xSide = Mathf.Cos(radians);
        float zSide = Mathf.Sin(radians);

        // Sideways movement happens opposite to the forward direction
        Vector3 sideMoveDirection = new Vector3(-zSide, 0.0f, xSide);  // Perpendicular direction
        transform.position += sideMoveDirection * moveSideways * moveSpeed * Time.deltaTime;

        // Rotate the boat around the Z-axis based on the sideways input
        transform.Rotate(0f, 0f, rotationAmount); // Rotate the boat around Z-axis
    }
}



}