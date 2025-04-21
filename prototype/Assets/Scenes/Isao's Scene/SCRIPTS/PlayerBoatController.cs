using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoatController : MonoBehaviour
{
    public bool isActive = true;
    public bool isControllable = true;

    public Transform cameraTarget;  // Should be the boat's transform
    public sui_demo_InputController inputController; // Assign in Inspector

    public float moveSpeed = 5f;
    public float rotationSpeed = 60f; // Degrees per second

    private float moveForward = 0.0f;
    private float moveSideways = 0.0f;

    void Start()
    {
        if (cameraTarget == null)
        {
            cameraTarget = this.transform;
        }

        if (inputController == null)
        {
            Debug.LogWarning("Input Controller not assigned to PlayerBoatController.");
        }
    }

    void Update()
    {
        if (!isActive || !isControllable || cameraTarget == null || inputController == null)
            return;

        // Reset movement inputs
        moveForward = 0.0f;
        moveSideways = 0.0f;

        // Read input from the input controller
        if (inputController.inputKeyW) moveForward = 1.0f;
        if (inputController.inputKeyS) moveForward = -1.0f;
        if (inputController.inputKeyA) moveSideways = -1.0f;
        if (inputController.inputKeyD) moveSideways = 1.0f;

        // Handle movement
        MoveBoat();
    }

    void MoveBoat()
    {
        // Forward/Backward movement
        if (moveForward != 0.0f)
        {
            Vector3 moveDirection = cameraTarget.forward;
            moveDirection.y = 0f;
            moveDirection.Normalize();

            cameraTarget.position += moveDirection * moveForward * moveSpeed * Time.deltaTime;
        }

        // Rotation
        if (moveSideways != 0.0f)
        {
            float rotationAmount = moveSideways * rotationSpeed * Time.deltaTime;
            cameraTarget.Rotate(0f, rotationAmount, 0f);
        }
    }
}
