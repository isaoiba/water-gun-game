using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Collisions : MonoBehaviour
{
    public GameObject character;
    public GameObject winTextObject;
    public Image screenOverlay; // UI Image (blue overlay)
    public float drownTime = 5f;
    private float currentDrownTime = 0f;
    private bool isInWater = false;

    private void Start()
    {
        isInWater = false;
        currentDrownTime = 0f;
    }

    private void Update()
    {
        if (isInWater)
        {
            currentDrownTime += Time.deltaTime;

            // Calculate progress
            float progress = Mathf.Clamp01(currentDrownTime / drownTime);

            // Apply pulsing alpha (fade and pulse)
            float pulse = 0.9f + Mathf.Sin(Time.time * 5f) * 0.1f; // Pulse between 0.8 and 1
            SetOverlayAlpha(progress * pulse);

            if (currentDrownTime >= drownTime)
            {
                Drown();
            }
        }
        else if (currentDrownTime > 0f)
        {
            currentDrownTime -= Time.deltaTime * 2f;
            float alpha = Mathf.Clamp01(currentDrownTime / drownTime);
            SetOverlayAlpha(alpha);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ocean"))
        {
            isInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ocean"))
        {
            isInWater = false;
        }
    }

    private void Drown()
    {
        winTextObject.gameObject.SetActive(true);
        winTextObject.GetComponent<TextMeshProUGUI>().text = "You Drowned!";
        character.SetActive(false);
    }

    private void SetOverlayAlpha(float alpha)
    {
        if (screenOverlay != null)
        {
            Color color = screenOverlay.color;
            color.a = alpha;
            screenOverlay.color = color;
        }
    }
}