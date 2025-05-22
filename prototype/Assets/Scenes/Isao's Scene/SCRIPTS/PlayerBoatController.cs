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

    public GameObject oceanSurface;  // Reference to the ocean surface GameObject
    public Vector3 oceanSurfaceOffset;  // Offset for the ocean surface to follow the boat

    private float moveForward = 0.0f;
    private float moveSideways = 0.0f;

    private float currentAngleZ = 0.0f;

    // Camera rotation
    public float mouseSensitivity = 2.0f;
    private float yaw = -102.4f;       
    private Vector3 cameraOffset;
    private float pitch = 20.0f;
 
    private bool initialized = false;
    private bool isTouchingLand = false;

    private Vector3 landDirectionNormalized = Vector3.right;

    void Start()
    {
        if (mainCamera != null)
        {
            // Set offset based on current distance between camera and boat
            cameraOffset = mainCamera.transform.position - transform.position;

            // Optionally, you can directly set the camera’s initial position to match the boat
            mainCamera.transform.position = transform.position + cameraOffset;

            // Initialize yaw and pitch based on current camera rotation
            Vector3 angles = mainCamera.transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }
    }

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
    if (!initialized)
    {
        // Set the initial yaw and pitch to maximum values
        yaw = 0f;           // You can set it to any value, e.g., 0f for starting view
        pitch = 80f;       // Maximum downward angle for the camera view

        initialized = true;
    }

    if (Input.GetMouseButton(0)) // Left mouse held
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;

        // Adjust pitch range for smoother control
        pitch = Mathf.Clamp(pitch, -80f, 80f);
    }

    // Create rotation based on updated pitch and yaw
    Quaternion rotation = Quaternion.Euler(pitch, yaw, 50f);

    // Set camera position relative to the boat's position
    mainCamera.transform.position = transform.position + rotation * cameraOffset;

    // Make the camera look at the boat
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
            landDirectionNormalized = (collision.gameObject.transform.position-transform.position).normalized;
            Debug.Log("isTouchingLand" + isTouchingLand);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Terrain>())
        {
            isTouchingLand = false;
            Debug.Log("isTouchingLand" +  isTouchingLand);
        }
    }

    public bool IsTouchingLand()
    {
        return isTouchingLand;
    }

    public Vector3 GetLandDirection(){
        return landDirectionNormalized;
    }
}
