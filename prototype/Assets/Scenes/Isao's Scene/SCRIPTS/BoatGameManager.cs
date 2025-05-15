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

    public Camera playerCamera; // Camera for walking around
    public Camera boatCamera;   // Camera for when on the boat

    public TextMeshProUGUI messageText;

    public float cameraMoveSpeed = 5f;
    public float cameraLookSpeed = 2f;
    public float getOffOffset = 2f;

    public float reboardDistance = 3f; // Distance required to reboard the boat

    private bool isOnBoat = false;

    void Start()
    {
        SetPlayerActive(true);
        boatController.isActive = false;
        boatController.mainCamera = null;

        // Enable only the player camera initially
        playerCamera.enabled = true;
        boatCamera.enabled = false;
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
        boatController.mainCamera = boatCamera;

        // Camera switch
        playerCamera.enabled = false;
        boatCamera.enabled = true;

        cube.SetActive(false); // Hide cube when on boat
    }

 void GetOffBoat()
{
    // Only allow getting off if boat is touching land
    if (!boatController.IsTouchingLand())
    {
        messageText.text = "You can only get off near land!";
        return;
    }

    isOnBoat = false;

    boatController.isActive = false;
    boatController.mainCamera = null;

    // Move player slightly to the side of the boat when disembarking
    
    Vector3 exitPosition = boatController.transform.position + boatController.transform.right * getOffOffset;

    SetPlayerActive(true);
    
    player.transform.position = exitPosition;

    // Camera switch
    playerCamera.enabled = true;
    boatCamera.enabled = false;

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
}
