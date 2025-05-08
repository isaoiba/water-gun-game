using UnityEngine;

public class OceanTilt : MonoBehaviour
{
    public float tiltSpeed = 1f; // Speed of tilt in degrees per second
    public float maxTiltAngle = 10f; // Maximum tilt angle on the x-axis (in degrees)
    
    private float currentTilt = 0f; // Current tilt angle
    private bool isTiltingPositive = true; // Flag to track if the ocean is tilting positively or negatively

    void Update()
    {
        // Calculate the tilt amount based on tiltSpeed and time
        float tiltAmount = tiltSpeed * Time.deltaTime;

        // Adjust currentTilt depending on the direction of tilt
        if (isTiltingPositive)
        {
            currentTilt += tiltAmount;
            if (currentTilt >= maxTiltAngle)
            {
                currentTilt = maxTiltAngle; // Ensure it doesn't exceed the max angle
                isTiltingPositive = false; // Reverse the direction when max angle is reached
            }
        }
        else
        {
            currentTilt -= tiltAmount;
            if (currentTilt <= -maxTiltAngle)
            {
                currentTilt = -maxTiltAngle; // Ensure it doesn't go below the negative max angle
                isTiltingPositive = true; // Reverse the direction when the negative max angle is reached
            }
        }

        // Apply the rotation to the ocean object's x-axis
        transform.rotation = Quaternion.Euler(-90, 0f, 0f);
    }
}
