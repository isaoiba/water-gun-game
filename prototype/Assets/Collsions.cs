using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Collsions : MonoBehaviour
{
    public LayerMask layerMask;
    public GameObject origin;
    public GameObject Player;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Cube"))
        {
            Player.gameObject.transform.position = origin.transform.position;
        }
    }
}
