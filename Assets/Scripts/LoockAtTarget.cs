using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoockAtTarget : MonoBehaviour
{
    public Transform loockGO,target;
    public bool atCamera;

    private void Awake()
    {
        if (atCamera)
        {
            target = Camera.main.transform;
        }
    }


    void LateUpdate()
    {
        if (loockGO.gameObject.activeSelf)
        {
            var lookPos = target.position - loockGO.position;
            lookPos.y = 0f;
            var rotation = Quaternion.LookRotation(lookPos);
            loockGO.rotation = rotation;
            loockGO.eulerAngles +=180f * Vector3.up;
        }
    }
}
