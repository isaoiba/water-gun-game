using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoatController : MonoBehaviour
{
    public bool isActive = true;
    public bool isControllable = true;

    public float moveSpeed = 5f;
    public float rotationSpeed = 60f; // Degrees per second
    public float slowedMoveSpeed = 1f;
    public float slowedRotationSpeed = 15f;

    public Camera mainCamera;
    public Vector3 cameraOffset;
    public GameObject oceanSurface;  // Reference to the ocean surface GameObject
    public Vector3 oceanSurfaceOffset;  // Offset for the ocean surface to follow the boat

    private float moveForward = 0.0f;
    private float moveSideways = 0.0f;

    private float currentAngleZ = 0.0f;

    // Camera rotation
    public float mouseSensitivity = 2.0f;
    private float yaw = 0.0f;
    private float pitch = 20.0f; // Slight downward look
    private bool isTouchingLand = false;

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

        MoveOceanSurface();  // Move the ocean surface with the boat
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
        float currentMoveSpeed = isTouchingLand ? slowedMoveSpeed : moveSpeed;
        float currentRotationSpeed = isTouchingLand ? slowedRotationSpeed : rotationSpeed;

        if (moveForward != 0.0f)
        {
            float radians = currentAngleZ * Mathf.Deg2Rad;
            float x = Mathf.Sin(radians);
            float z = Mathf.Cos(radians);
            Vector3 moveDirection = new Vector3(x, 0.0f, z);
            transform.position += moveDirection * -moveForward * currentMoveSpeed * Time.deltaTime;
        }

        if (moveSideways != 0.0f)
        {
            float rotationAmount = moveSideways * currentRotationSpeed * Time.deltaTime;
            currentAngleZ += rotationAmount;

            float radians = currentAngleZ * Mathf.Deg2Rad;
            float xSide = Mathf.Cos(radians);
            float zSide = Mathf.Sin(radians);
            Vector3 sideMoveDirection = new Vector3(-zSide, 0.0f, xSide);
            transform.position += sideMoveDirection * moveSideways * currentMoveSpeed * Time.deltaTime;

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
        Vector3 desiredCameraPosition = transform.position + rotation * cameraOffset;

        mainCamera.transform.position = desiredCameraPosition;
        mainCamera.transform.LookAt(transform.position);
    }

    void MoveOceanSurface()
    {
        // Adjust ocean surface position based on the boat's position and the offset
        if (oceanSurface != null)
        {
            oceanSurface.transform.position = transform.position + oceanSurfaceOffset;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<Terrain>())
        {
            isTouchingLand = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Terrain>())
        {
            isTouchingLand = false;
        }
    }

    public bool IsTouchingLand()
    {
        return isTouchingLand;
    }
}
