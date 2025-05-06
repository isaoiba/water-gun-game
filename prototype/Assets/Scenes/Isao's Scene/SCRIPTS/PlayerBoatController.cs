using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoatController : MonoBehaviour
{
    public bool isActive = true;
    public bool isControllable = true;

    public float moveSpeed = 5f;
    public float rotationSpeed = 60f; // Degrees per second

    public Camera mainCamera;
    public Vector3 cameraOffset = new Vector3(0f, 10f, -10f);

    private float moveForward = 0.0f;
    private float moveSideways = 0.0f;

    private float currentAngleZ = 0.0f;

    // Camera rotation
    public float mouseSensitivity = 2.0f;
    private float yaw = 0.0f;
    private float pitch = 20.0f; // Slight downward look

    void Update()
    {
        if (!isActive || !isControllable)
            return;

        HandleInput();
        MoveBoat();

        if (mainCamera != null)
        {
            HandleCamera();
        }
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

    void MoveBoat()
    {
        if (moveForward != 0.0f)
        {
            float radians = currentAngleZ * Mathf.Deg2Rad;
            float x = Mathf.Sin(radians);
            float z = Mathf.Cos(radians);
            Vector3 moveDirection = new Vector3(x, 0.0f, z);
            transform.position += moveDirection * -moveForward * moveSpeed * Time.deltaTime;
        }

        if (moveSideways != 0.0f)
        {
            float rotationAmount = moveSideways * rotationSpeed * Time.deltaTime;
            currentAngleZ += rotationAmount;

            float radians = currentAngleZ * Mathf.Deg2Rad;
            float xSide = Mathf.Cos(radians);
            float zSide = Mathf.Sin(radians);
            Vector3 sideMoveDirection = new Vector3(-zSide, 0.0f, xSide);
            transform.position += sideMoveDirection * moveSideways * moveSpeed * Time.deltaTime;

            transform.Rotate(0f, 0f, rotationAmount);
        }
    }

    void HandleCamera()
    {
        if (Input.GetMouseButton(0)) // Left mouse held
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * mouseSensitivity;
            pitch -= mouseY * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, 10f, 80f); // Prevent flipping
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = transform.position + rotation * cameraOffset;

        mainCamera.transform.position = desiredPosition;
        mainCamera.transform.LookAt(transform.position);
    }
}
