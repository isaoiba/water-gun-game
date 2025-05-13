using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoatGameManager : MonoBehaviour
{
    public PlayerBoatController boatController;
    public GameObject player;
    public GameObject cube; // Assign in Inspector
    public Camera mainCamera;
    public TextMeshProUGUI messageText;

    public float cameraMoveSpeed = 5f;
    public float cameraLookSpeed = 2f;
    public float getOffOffset = 2f;

    public float reboardDistance = 3f; // Distance required to reboard the boat

    private bool isOnBoat = false;

    private float yaw = 0f;
    private float pitch = 20f;


    void Start()
    {
        SetPlayerActive(true);
        boatController.isActive = false;
        boatController.mainCamera = null;

        mainCamera.transform.SetParent(player.transform);
        mainCamera.transform.localPosition = new Vector3(0f, 1.6f, -3f);
        mainCamera.transform.localRotation = Quaternion.Euler(10f, 0f, 0f);
    }


    void Update()
    {
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
            float distanceToBoat = Vector3.Distance(player.transform.position, boatController.transform.position);

            if (distanceToBoat <= reboardDistance)
            {
                messageText.text = "Press F to get back on the boat";
                if (Input.GetKeyDown(KeyCode.F))
                {
                    GetOnBoat();
                }
            }
            else
            {
                messageText.text = "";
            }
        }
        else
        {
            messageText.text = "";
        }
    }

   void GetOnBoat()
{
    isOnBoat = true;

    SetPlayerActive(false);
    boatController.isActive = true;
    boatController.mainCamera = mainCamera;

    mainCamera.transform.SetParent(boatController.transform);
    mainCamera.transform.localPosition = new Vector3(0f, 3f, -6f);
    mainCamera.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
    mainCamera.fieldOfView = 75f;

    cube.SetActive(false); // Hide cube when on boat
}

void GetOffBoat()
{
    isOnBoat = false;

    boatController.isActive = false;
    boatController.mainCamera = null;

    Vector3 exitPosition = boatController.transform.position + boatController.transform.right * getOffOffset;
    player.transform.position = exitPosition;
    SetPlayerActive(true);

    mainCamera.transform.SetParent(player.transform);
    mainCamera.transform.localPosition = new Vector3(0f, 1.6f, -3f);
    mainCamera.transform.localRotation = Quaternion.Euler(10f, 0f, 0f);
    mainCamera.fieldOfView = 75f;

    cube.SetActive(true); // Show cube when off boat
}



    void SetPlayerActive(bool active)
    {
        player.SetActive(active);

        var controller = player.GetComponent<PlayerCharacter>();
        if (controller != null)
        {
            controller.enabled = active;
        }
    }

    void HandleFreeCamera()
    {
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * cameraLookSpeed;
            pitch -= Input.GetAxis("Mouse Y") * cameraLookSpeed;
            pitch = Mathf.Clamp(pitch, 10f, 80f);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        mainCamera.transform.rotation = rotation;

        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move += mainCamera.transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= mainCamera.transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= mainCamera.transform.right;
        if (Input.GetKey(KeyCode.D)) move += mainCamera.transform.right;

        mainCamera.transform.position += move * cameraMoveSpeed * Time.deltaTime;
    }
}
