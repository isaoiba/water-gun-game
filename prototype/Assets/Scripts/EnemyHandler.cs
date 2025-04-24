using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    // Start is called before the first frame update

    public Player something;

    public static float Health = 100f;

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
