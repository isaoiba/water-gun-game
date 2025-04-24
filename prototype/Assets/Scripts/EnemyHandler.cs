using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    // Start is called before the first frame update

    public static float Health = 100f;

    public void SubtractHealth(float Damage)
    {
        Health -= Damage;

        if (Health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
