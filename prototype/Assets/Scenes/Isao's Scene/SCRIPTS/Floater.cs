using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Floater : MonoBehaviour
{
    public Transform[] floaters;
    public float underWaterDrag = 3f;
    public float underWaterAngularDrag = 1f;
    public float airDrag = 0f;
    public float airAngularDrag = 0.05f;
    public float floatingPower = 15f;
    public float waterHeight = 0f; // Set this to the y-position of your water surface

    private Rigidbody m_Rigidbody;
    int floatersUnderwater;
    private bool underwater;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() // This must be FixedUpdate, not FixedBody
    {
        floatersUnderwater = 0;
        for (int i = 0; i < floaters.Length; i++)
        {
            float difference = floaters[i].position.y - waterHeight;

            if (difference < 0)
            {
                m_Rigidbody.AddForceAtPosition(
                    Vector3.up * floatingPower * Mathf.Abs(difference),
                    floaters[i].position,
                    ForceMode.Force
                );
                floatersUnderwater += 1;

                if (!underwater)
                {
                    underwater = true;
                    SwitchState(true);
                }
            }
        }
        if (underwater && floatersUnderwater == 0)
        {
            underwater = false;
            SwitchState(false);
        }
    }

    void SwitchState(bool isUnderwater)
    {
        if (isUnderwater)
        {
            m_Rigidbody.drag = underWaterDrag;
            m_Rigidbody.angularDrag = underWaterAngularDrag;
        }
        else
        {
            m_Rigidbody.drag = airDrag;
            m_Rigidbody.angularDrag = airAngularDrag;
        }
    }
}
