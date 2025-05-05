using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Rigidbody))]
public class Floater : MonoBehaviour
{
    [Header("Water Settings")]
    public WaterSurface waterSurface;       // Reference to ocean object (HDRP Water System)
    public Material waterMaterial;          // Reference to water shader/material used

    [Header("Object Settings")]
    public float objectVolume = 5f;         // Approximate volume of the object in m³
    public float fluidDensity = 1000f;      // Density of fluid (water = 1000 kg/m³)
    public float verticalOffset = 0f;       // Offset from pivot to bottom of the object

    private Rigidbody rb;
    private float gravity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gravity = Mathf.Abs(Physics.gravity.y);
    }

    void FixedUpdate()
    {
        if (waterSurface == null || waterMaterial == null) return;

        float waterY = GetWaterHeight(transform.position);
        Debug.Log("waterY: " + waterY);

        float objectY = transform.position.y + verticalOffset;
        float submergedDepth = waterY - objectY;

        if (submergedDepth > 0f)
        {
            float submergedFraction = Mathf.Clamp01(submergedDepth / transform.localScale.y);
            float buoyancyForce = fluidDensity * gravity * objectVolume * submergedFraction;

            rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Force);
        }
    }

    float GetWaterHeight(Vector3 position)
    {
        // Fetch wave properties from the water material
        float waveStrength = waterMaterial.GetFloat("_WaveStrength");
        float waveSpeed = waterMaterial.GetFloat("_WaveSpeed");
        float waveTile = waterMaterial.GetFloat("_WaveTile");

        float time = Time.time;

        // Approximate wave using simple sine/cosine pattern
        float wave =
            Mathf.Sin(position.x * waveTile + time * waveSpeed) +
            Mathf.Cos(position.z * waveTile + time * waveSpeed);

        wave *= waveStrength * 0.5f;

        return waterSurface.transform.position.y + wave;
    }
}
