using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    // Start is called before the first frame update
    
    public TextMeshProUGUI countText;
    private Rigidbody rb;

    // UI object to display winning text.
    public GameObject winTextObject;
    public static int count;
    
    Rect rect = new Rect(0, 0, 300, 100);
    Vector3 offset = new Vector3(0f, 0f, 0.5f); // height above the target position
    
    private float Health = 100f;
    public GameObject playerObject;
    private Camera camera;

    void OnGUI()
    {
        Vector3 point = camera.WorldToScreenPoint(gameObject.transform.position + offset);
        rect.x = point.x;
        rect.y = Screen.height - point.y - rect.height; // bottom left corner set to the 3D point
        GUI.Label(rect, "Health:"+ Health); // display its name, or other string
    }

    private void Start()
    {
        count = 0;
        camera = Camera.main;
    }

    public void SubtractHealth(float Damage)
    {
        Debug.Log("Health was " + Health);
        Health -= Damage;
        Debug.Log("Health is " + Health);

        if (Health <= 0f)
        {
            Debug.Log("Dead");
            count = count + 1;
            Debug.Log(count);
            SetCountText();
        }
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Collision");
        if (collider.gameObject.CompareTag("Hitbox"))
        {
            Destroy(playerObject.gameObject);
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
    }
    private void SetCountText() 
    {
        // Update the count text with the current count.
        countText.text = "Enemies killed: " + count + "/70";

        // Check if the count has reached or exceeded the win condition.
        if (count >= 70)
        {
            // Display the win text.
            winTextObject.SetActive(true);
        }
        
        Destroy(gameObject);

    }
}
