using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkController : MonoBehaviour
{
    public Transform lHandIk, rHandIk, lLegIk, rLegIk;
    public Transform lLegIkAnim, rLegIkAnim;
    public Transform pedalsRotate;
    public Transform rul;
    public float speed;
    
    void Update()
    {
        if (speed > 0)
        {
            lLegIk.position = lLegIkAnim.position;
            rLegIk.position = rLegIkAnim.position;
            pedalsRotate.Rotate(new Vector3(0,0,50)*speed*Time.deltaTime);
        }
    }

    public void SetSpeed(float value)
    {
        speed = value;
    }
}
