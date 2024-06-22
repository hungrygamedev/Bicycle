using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceSlider : MonoBehaviour
{
    public LayerMask floorLayer;
    public Vector3 normal;

    private void Start()
    {
        //floorLayer = LayerMask.NameToLayer("Floor");
    }

    public Vector3 Project(Vector3 forward)
    {
        return forward - Vector3.Dot(forward, normal) * normal;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 2,
            floorLayer))
        {
            Debug.Log("HIT");
                normal = hit.normal;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Floor")
        {
         //   normal = other.contacts[0].normal;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Floor")
        {
          //  normal = other.contacts[0].normal;
        }
    }
}
