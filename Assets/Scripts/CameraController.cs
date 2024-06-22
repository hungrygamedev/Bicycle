using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform camRoot, target;
    public FixedTouchField touchField;
    public float oldHorizontalInput = 0f;
    public float oldVerticalInput = 0f;
    [Range(1f, 50f)]
    public float cameraSmoothingFactor = 25f;
    [Range(1f, 50f)]
    public float cameraSmoothingFactorIdle = 25f;
    public float currentXAngle = 0f;
    public float currentYAngle = 0f;

    //Upper and lower limits (in degrees) for vertical rotation (along the local x-axis of the gameobject);
    [Range(0f, 90f)]
    public float upperVerticalLimit = 60f;
    [Range(0f, 90f)]
    public float lowerVerticalLimit = 60f;
    public float cameraSpeed = 250f;
    public float cameraSpeedRoot = 250f;

    public Transform visualTransform;
    private float delayInput;
    private void Start()
    {
        touchField = UIController.instance.touchField;
    }

    private void Update()
    {
        //camRoot.rotation = Quaternion.Lerp(camRoot.rotation,Quaternion.Euler(Vector3.zero), cameraSpeedRoot*Time.deltaTime);
    }

    void FixedUpdate()
    {
        camRoot.position = Vector3.Lerp(camRoot.position,target.position+new Vector3(0,2,0),10*Time.deltaTime);
        
        float _inputHorizontal = touchField.TouchDist.x;//cameraInput.GetHorizontalCameraInput();
        float _inputVertical = -touchField.TouchDist.y;

        if (touchField.Pressed)
        {
            oldHorizontalInput =
                Mathf.Lerp(oldHorizontalInput, _inputHorizontal, Time.deltaTime * cameraSmoothingFactor);
            oldVerticalInput = Mathf.Lerp(oldVerticalInput, _inputVertical, Time.deltaTime * cameraSmoothingFactor);

            //Add input to camera angles;
            currentXAngle += oldVerticalInput * cameraSpeed * Time.deltaTime;
            currentYAngle += oldHorizontalInput * cameraSpeed * Time.deltaTime;

            //Clamp vertical rotation;
            currentXAngle = Mathf.Clamp(currentXAngle, -upperVerticalLimit, lowerVerticalLimit);


            transform.localRotation = Quaternion.Euler(new Vector3(currentXAngle, currentYAngle, 0));
            delayInput = 0;
        }
        else
        {
           /* if (delayInput < 1f)
            {
                delayInput += 1f * Time.deltaTime;
            }
            else
            {
                oldHorizontalInput = oldVerticalInput = 0f;
                Quaternion rot = Quaternion.Lerp(transform.localRotation, visualTransform.rotation,
                    Time.deltaTime * cameraSmoothingFactorIdle);
                transform.localRotation = rot;
                currentXAngle = transform.localEulerAngles.x;
                currentYAngle= -transform.localEulerAngles.y;
            }
*/
            
        }

    }

    public void RotateCam(Vector3 r)
    {
        if (touchField.Pressed == false)
        {
            camRoot.Rotate(new Vector3(0,r.y,0));
        }
    }

    public void ResetCamera()
    {
        transform.DOLocalRotate(Vector3.zero, 0.5f).OnComplete(() =>
        {
            currentXAngle = transform.localEulerAngles.x;
            currentYAngle= -transform.localEulerAngles.y;
        });
        oldHorizontalInput = oldVerticalInput = 0f;
    }
}
