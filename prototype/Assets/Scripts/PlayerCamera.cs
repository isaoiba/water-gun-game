using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CameraInput
{
    public Vector2 Look;
}

public class PlayerCamera : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField] private float sensitivity = 0.1f;
    
    private Vector3 _eulerAngles;
    
    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.eulerAngles = _eulerAngles = target.eulerAngles;
    }

    public void UpdateRotation(CameraInput input)
    {
        _eulerAngles += new Vector3(-input.Look.y, input.Look.x) * sensitivity;
        transform.eulerAngles = _eulerAngles;
    }
    
    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
