using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KinematicCharacterController;

public class BoatGameManager : MonoBehaviour
{
    public PlayerBoatController boatController;
    public GameObject player;
    public GameObject cube; // Assign in Inspector

    public Camera playerCamera; // Camera for walking around
    public Camera boatCamera;   // Camera for when on the boat
    public GameObject ocean;

    public TextMeshProUGUI messageText;

    public float cameraMoveSpeed = 5f;
    public float cameraLookSpeed = 2f;
    public float getOffOffset = 2f;

    public float reboardDistance = 3f; // Distance required to reboard the boat

    private bool isOnBoat = false;
    
    private BoxCollider[] boatBoxColliders;

    void Start()
    {
        SetPlayerActive(true);
        boatController.isActive = false;
        boatController.mainCamera = null;

        // Enable only the player camera initially
        playerCamera.enabled = true;
        boatCamera.enabled = false;
        
        boatBoxColliders = boatController.GetComponentsInChildren<BoxCollider>();
    }

    void Update()
    {
        foreach (BoxCollider col in boatBoxColliders)
            {
                col.enabled = true;
            }
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
            ocean.transform.localScale = new Vector3(10000f, 10000f, 1f);
            foreach (BoxCollider col in boatBoxColliders)
            {
                col.enabled = false;
            }
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
        ocean.transform.localScale = new Vector3(300f, 300f, 1f);
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
    if (!boatController.IsTouchingLand())
    {
        messageText.text = "You can only get off near land!";
        return;
    }

    isOnBoat = false;

    boatController.isActive = false;
    boatController.mainCamera = null;

    // Calculate exit position
    Vector3 exitPosition = boatController.transform.position + boatController.GetLandDirection() * getOffOffset + Vector3.up * 5f;


    // Teleport player correctly
    var motor = player.GetComponent<KinematicCharacterMotor>();
    var characterController = player.GetComponent<CharacterController>();
    var rigidbody = player.GetComponent<Rigidbody>();

    if (motor != null)
    {
        motor.SetPosition(exitPosition, false);
    }
    else if (characterController != null)
    {
        characterController.enabled = false;
        player.transform.position = exitPosition;
        characterController.enabled = true;
    }
    else if (rigidbody != null)
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.position = exitPosition;
    }
    else
    {
        player.transform.position = exitPosition;
    }

    // Activate the player
    SetPlayerActive(true);
    
    player.transform.position = exitPosition;

    // Camera switch
    playerCamera.enabled = true;
    boatCamera.enabled = false;

    cube.SetActive(true);
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

    void RespawnPlayerByBoat()
{
    Vector3 respawnPosition = boatController.transform.position;
    player.transform.position = respawnPosition;

    // Optional: reset camera rotation or other states
    if (playerCamera != null)
    {
        playerCamera.transform.LookAt(boatController.transform.position);
    }
}

}
