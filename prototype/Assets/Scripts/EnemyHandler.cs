using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    // Start is called before the first frame update
    
    Rect rect = new Rect(0, 0, 300, 100);
    Vector3 offset = new Vector3(0f, 0f, 0.5f); // height above the target position
    
    public Player something;

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
        something = GameObject.FindObjectOfType(typeof(Player)) as Player;
    }

    public void SubtractHealth(float Damage)
    {
        Health -= Damage;

        if (Health <= 0f)
        {
            something.SetCountText();
            ++something.count;
            Destroy(gameObject);
        }
    }
}
