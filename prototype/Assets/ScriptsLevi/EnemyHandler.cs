using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{
    // Start is called before the first frame update

    public float Health = 10f;

    public void SubtractHealth(float Damage)
    {
        Health -= Damage;

        if (Health <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
