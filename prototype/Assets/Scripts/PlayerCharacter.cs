using KinematicCharacterController;
using Unity.Mathematics;
using UnityEngine;

public enum CrouchInput
{
        None, Toggle
}

public enum Stance
{
        Stand, Crouch
}

public struct CharacterInput
{
        public Quaternion Rotation;

        public Vector2 Move;
        
        public bool Jump;

        public bool jumpSustain;
        
        public CrouchInput Crouch;
}

public class PlayerCharacter : MonoBehaviour, ICharacterController
{
        [SerializeField] private KinematicCharacterMotor motor;
        [SerializeField] private Transform root;
        [SerializeField] private Transform cameraTarget;
        [Space] 
        [SerializeField] private float walkSpeed = 20f;
        [SerializeField] private float crouchSpeed = 7f;
        [SerializeField] private float walkResponse = 25f;
        [Space] [SerializeField] private float airSpeed = 15f;
        [SerializeField] private float airAcceleration = 70f;
        [Space]
        [SerializeField] private float crouchResponse = 20f;
        [Space] 
        [SerializeField] private float jumpSpeed = 20f;
        [Range(0f, 1f)]
        [SerializeField] private float jumpSustainGravity = 0.4f;
        [SerializeField] private float gravity = -90f;
        [Space]
        [SerializeField] private float standHeight = 2f;
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] private float crouchHeightResponse = 15f;
        [Range(0f, 1f)]
        [SerializeField] private float standCameraTargetHeight = 0.9f;
        [Range(0f, 1f)]
        [SerializeField] private float crouchCameraTargetHeight = 0.7f;


        private Stance _stance;
        
        private Quaternion _requestedRotation;

        private Vector3 _requestedMovement;
        
        private bool _requestedJump;

        private bool _requestedSustainedJump;
        
        private bool _requestedCrouch;

        public void Initialize()
        {
                _stance = Stance.Stand;
                motor.CharacterController = this;
        }

        public void UpdateInput(CharacterInput input)
        {
                _requestedRotation = input.Rotation;
                _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
                _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);
                _requestedMovement = input.Rotation * _requestedMovement;
                _requestedJump = _requestedJump || input.Jump;
                _requestedSustainedJump = input.jumpSustain;
                
                _requestedCrouch = input.Crouch switch
                {
                        CrouchInput.Toggle => !_requestedCrouch,
                        CrouchInput.None => _requestedCrouch,
                        _ => _requestedCrouch
                };
        }

        public void UpdateBody(float deltaTime)
        {
                var currentHeight = motor.Capsule.height;
                var normalizedHeight = currentHeight / standHeight;
                
                var cameraTargetHeight = currentHeight *
                (
                        _stance is Stance.Stand
                                ? standCameraTargetHeight 
                                : crouchCameraTargetHeight
                );
                var rootTargetScale = new Vector3(1f, normalizedHeight, 1f);

                cameraTarget.localPosition = Vector3.Lerp
                (
                        a: cameraTarget.localPosition,
                        b: new Vector3(0f, cameraTargetHeight, 0f),
                        t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
                );
                
                root.localScale = rootTargetScale;
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
                var forward = Vector3.ProjectOnPlane
                (
                        _requestedRotation * Vector3.forward,
                        motor.CharacterUp
                );
                if (forward != Vector3.zero)
                {
                        currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
                }
        }

        /// <summary>
        /// This is called when the motor wants to know what its velocity should be right now
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
                if (motor.GroundingStatus.IsStableOnGround)
                {
                        var groundedMovement = motor.GetDirectionTangentToSurface
                        (
                                direction: _requestedMovement,
                                surfaceNormal: motor.GroundingStatus.GroundNormal
                        ) * _requestedMovement.magnitude;

                        var speed = _stance is Stance.Stand
                                ? walkSpeed
                                : crouchSpeed;
                        var response = _stance is Stance.Stand
                                ? walkResponse
                                : crouchResponse;
                
                        var targetVelocity = groundedMovement * speed;

                        currentVelocity = Vector3.Lerp
                        (
                                a: currentVelocity,
                                b: targetVelocity,
                                t: 1f - Mathf.Exp(-response * deltaTime)
                        );
                }
                else
                {
                        if (_requestedMovement.sqrMagnitude > 0f)
                        {
                                var planarMovement = Vector3.ProjectOnPlane
                                (
                                        vector: _requestedMovement,
                                        planeNormal: motor.CharacterUp
                                ) * _requestedMovement.magnitude;

                                var currentPlanarVelocity = Vector3.ProjectOnPlane
                                (
                                        vector: currentVelocity,
                                        planeNormal: motor.CharacterUp
                                );
                                
                                var movementForce = planarMovement * airAcceleration * deltaTime;
                                var targetPlanarVelocity = currentPlanarVelocity + movementForce;
                                
                                targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);

                                currentVelocity += targetPlanarVelocity - currentPlanarVelocity;
                        }
                        
                        var effectivegravity = gravity;
                        
                        var verticleSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                        if (_requestedSustainedJump && verticleSpeed > 0f)
                                effectivegravity *= jumpSustainGravity;
                        currentVelocity += motor.CharacterUp * effectivegravity * deltaTime; 
                }

                if (_requestedJump)
                {
                        _requestedJump = false;
                        
                        motor.ForceUnground(time : 0f);

                        var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                        var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
                        
                        currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
                }
        }

        /// <summary>
        /// This is called before the motor does anything
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime)
        {
                if (_requestedCrouch && _stance is Stance.Stand)
                {
                        _stance = Stance.Crouch;
                        motor.SetCapsuleDimensions
                        (
                                radius: motor.Capsule.radius,
                                height: crouchHeight,
                                yOffset: crouchHeight * 0f
                        );
                }
        }
        /// <summary>
        /// This is called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling
        /// </summary>
        public void PostGroundingUpdate(float deltaTime){}

        /// <summary>
        /// This is called after the motor has finished everything in its update
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
        {
                if (!_requestedCrouch && _stance is not Stance.Stand)
                {
                        _stance = Stance.Stand;
                        motor.SetCapsuleDimensions
                        (
                                radius: motor.Capsule.radius,
                                height: standHeight,
                                yOffset: standHeight * 0f
                        );
                }
        }

        /// <summary>
        /// This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)
        /// </summary>
        public bool IsColliderValidForCollisions(Collider coll) => true;
        /// <summary>
        /// This is called when the motor's ground probing detects a ground hit
        /// </summary>
        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}
        /// <summary>
        /// This is called when the motor's movement logic detects a hit
        /// </summary>
        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport){}
        /// <summary>
        /// This is called after every move hit, to give you an opportunity to modify the HitStabilityReport to your liking
        /// </summary>
        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport){}
        /// <summary>
        /// This is called when the character detects discrete collisions (collisions that don't result from the motor's capsuleCasts when moving)
        /// </summary>
        public void OnDiscreteCollisionDetected(Collider hitCollider){}
        
        public Transform GetCameraTarget() => cameraTarget;
}
