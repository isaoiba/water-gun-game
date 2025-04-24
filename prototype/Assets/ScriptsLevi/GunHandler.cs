using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent(typeof(LineRenderer))]
public class RaycastGun : MonoBehaviour
{
        
    public Camera playerCamera;
    public Transform laserOrigin;
    public float gunRange = 50f;
    public float fireRate = 0.05f;
    public float laserDuration = 0.05f;
 
    LineRenderer laserLine;
    float fireTimer;
 
    void Awake()
    {
        laserLine = GetComponent<LineRenderer>();
    }
 
    void Update()
    {
        fireTimer += Time.deltaTime;
        laserLine.SetPosition(0, laserOrigin.position);
        Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if(Physics.Raycast(rayOrigin, playerCamera.transform.forward, out hit, gunRange))
        {
            laserLine.SetPosition(1, hit.point);
            if(Input.GetMouseButton(0))
            {
                if(fireTimer > fireRate)
                {
                    fireTimer = 0;
                    laserLine.enabled = true;
                    if (hit.collider.gameObject.TryGetComponent(out EnemyHandler enemy))
                    {
                        enemy.SubtractHealth(10f);
                    }
                }
            }
            else
            {
                laserLine.enabled = false;
            }
        }
        else
        {
            laserLine.SetPosition(1, rayOrigin + (playerCamera.transform.forward * gunRange));
            if(Input.GetMouseButton(0))
            {
                if(fireTimer > fireRate)
                {
                    fireTimer = 0;
                    laserLine.enabled = true;
                }
            }
            else
            {
                laserLine.enabled = false;
            }
        }
    }
}