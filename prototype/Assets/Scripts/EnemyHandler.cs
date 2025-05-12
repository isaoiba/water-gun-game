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
    public int count;
    
    Rect rect = new Rect(0, 0, 300, 100);
    Vector3 offset = new Vector3(0f, 0f, 0.5f); // height above the target position
    
    public Collsions something;

    public float Health = 100f;

    void OnGUI()
    {
        Vector3 point = Camera.main.WorldToScreenPoint(gameObject.transform.position + offset);
        rect.x = point.x;
        rect.y = Screen.height - point.y - rect.height; // bottom left corner set to the 3D point
        GUI.Label(rect, "Health:"+ Health); // display its name, or other string
    }

    private void Start()
    {
        something = GameObject.FindObjectOfType(typeof(Collsions)) as Collsions;
    }

    public void SubtractHealth(float Damage)
    {
        Health -= Damage;

        if (Health <= 0f)
        {
            SetCountText();
            ++count;
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Collision");
        if (collider.gameObject.CompareTag("Hitbox"))
        {
            Destroy(collider.gameObject);
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
    }
    public void SetCountText() 
    {
        // Update the count text with the current count.
        //countText.text = "Enemies killed: " + count + "/5";

        // Check if the count has reached or exceeded the win condition.
        if (count >= 5)
        {
            // Display the win text.
            winTextObject.SetActive(true);
        }
    }
}
