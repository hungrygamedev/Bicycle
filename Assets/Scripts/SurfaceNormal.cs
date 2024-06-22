using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceNormal : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask surfaceLayer;
    public VariableJoystick _Joystick;
    public float rotationSpeed;

    public float rotarionSurfaceDamping;

    public float distanceToGround;
    public float speed;
    
    public float yVel = 0;
    public float gravity = 9.81f;
    public float jumpPover;
    public bool isJumped;
    public float jumpDelay;


    public Vector3 myNormal,surfaceNormal;
    public Vector3 hitSurfacePoint;
    public Vector3 normal;
    void Start()
    {
        myNormal = transform.up;
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position+transform.up*1f, -transform.up, out hit,3,surfaceLayer))
        {
            normal = hit.normal;
            surfaceNormal=hit.normal;
            hitSurfacePoint = hit.point;
        }
        else
        {
            surfaceNormal = Vector3.up;
            hitSurfacePoint = Vector3.zero;
        }

        if (_Joystick.Horizontal != 0)
        {
            transform.Rotate(0, _Joystick.Horizontal*rotationSpeed*Time.deltaTime, 0);
        }
        else
        {
            transform.Rotate(0, Input.GetAxis("Horizontal")*rotationSpeed*Time.deltaTime, 0);
        }
        
        myNormal = Vector3.Lerp(myNormal, surfaceNormal, rotarionSurfaceDamping*Time.deltaTime);
        var myForward = Vector3.Cross(transform.right, myNormal);
        var targetRot = Quaternion.LookRotation(myForward, myNormal);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotarionSurfaceDamping*Time.deltaTime);
        //var toRotate = transform.up * _Joystick.Horizontal*rotationSpeed;
        //transform.rotation = Quaternion.FromToRotation(transform.forward,toRotate);

        if (isJumped)
        {
            if (jumpDelay < 0.5f)
            {
                jumpDelay += 1f * Time.deltaTime;
            }
            else
            {
                jumpDelay = 0f;
                isJumped = false;
            }
        }
        
        if (IsGrounded())
        {
            if (!isJumped)
            {
                if (speed < moveSpeed)
                {
                    speed += 15f * Time.deltaTime;
                }

                if (hitSurfacePoint != Vector3.zero)
                {
                    transform.position = hitSurfacePoint+surfaceNormal*0.1f;
                }
                Vector3 dirSurface = Project(transform.forward.normalized);
                Vector3 offset = (dirSurface) * (speed * Time.deltaTime);
                
                transform.Translate(offset);
  
                yVel = 0;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    yVel = jumpPover;
                    isJumped = true;
                }

                
            }
        }
        else
        {
            //transform.Translate(transform.forward*speed*Time.deltaTime);
            //transform.position += transform.up * yVel * Time.deltaTime;
            yVel -= gravity * Time.deltaTime;
        }
        //transform.position += transform.up * yVel * Time.deltaTime;
        
        
    }
    
    
    public Vector3 Project(Vector3 forward)
    {
        return forward - Vector3.Dot(forward, normal) * normal;
    }
    
    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position+transform.up*1f, -transform.up, out hit, 1f+distanceToGround,
            surfaceLayer))
        {
            return true;
        }
        return false;
        
    }
}
