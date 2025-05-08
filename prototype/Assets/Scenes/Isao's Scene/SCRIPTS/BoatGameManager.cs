using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoatGameManager : MonoBehaviour
{
    public PlayerBoatController boatController;
    public Camera mainCamera;
    public TextMeshProUGUI  messageText;

    public float reboardDistance = 5f;
    public float cameraMoveSpeed = 5f;
    public float cameraLookSpeed = 2f;

    private bool isOnBoat = true;
    private float yaw = 0f;
    private float pitch = 20f;

    void Update()
    {
        float distance = Vector3.Distance(mainCamera.transform.position, boatController.transform.position);
        if (isOnBoat && boatController.IsTouchingLand())
        {
            messageText.text = "Press F to get off the boat";
            if (Input.GetKeyDown(KeyCode.F))
            {
                GetOffBoat();
            }
        }
        else if (!isOnBoat)
        {
            HandleFreeCamera();

            if (distance <= reboardDistance)
            {
                messageText.text = "Press F to get back on the boat";
                if (Input.GetKeyDown(KeyCode.F))
                {
                    GetOnBoat();
                }
            }
            else
            {
                messageText.text = ""; // Too far from boat
            }
        }
        else
        {
            messageText.text = "";
        }
    }

    void GetOffBoat()
    {
        isOnBoat = false;
        boatController.isActive = false;
        boatController.mainCamera = null;
    }

    void GetOnBoat()
    {
        isOnBoat = true;
        boatController.isActive = true;
        boatController.mainCamera = mainCamera;
    }

    void HandleFreeCamera()
    {
        // Mouse look
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            yaw += Input.GetAxis("Mouse X") * cameraLookSpeed;
            pitch -= Input.GetAxis("Mouse Y") * cameraLookSpeed;
            pitch = Mathf.Clamp(pitch, 10f, 80f);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        mainCamera.transform.rotation = rotation;

        // Movement (WASD)
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move += mainCamera.transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= mainCamera.transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= mainCamera.transform.right;
        if (Input.GetKey(KeyCode.D)) move += mainCamera.transform.right;

        mainCamera.transform.position += move * cameraMoveSpeed * Time.deltaTime;
    }
}
