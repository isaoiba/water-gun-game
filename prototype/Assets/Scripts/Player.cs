using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    
    private PlayerInputActions inputActions;
    
    // Start is called before the first frame update

    // Update is called once per frame
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());
    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    void Update()
    {
        var input = inputActions.Gameplay;
        var deltaTime = Time.deltaTime;
        
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

        var characterInput = new CharacterInput
        {
            Rotation = playerCamera.transform.rotation,
            Move = input.Move.ReadValue<Vector2>(),
            Jump = input.Jump.WasPressedThisFrame(),
            jumpSustain = input.Jump.IsPressed(),
            Crouch = input.Crouch.WasPressedThisFrame()
                ? CrouchInput.Toggle
                : CrouchInput.None
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);
    }

    private void LateUpdate()
    {
        playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
    }
}
