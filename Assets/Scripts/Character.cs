using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum CharacterType
    {
        player,
        enemy
    }

    public CharacterType _Type;
    public LayerMask surfaceLayer,wallLayer;

    public float forwardDistance;
    public float distanceToGround = 0.6f;
    public float surfaceDistance = 3f;
    public float moveSpeed = 20f;
    public float rotationSpeed = 200f;
    public float rotarionSurfaceDamping = 10f;
    public float jumpPover=15f;
    public float gravity = 30f;
    private bool isJumped;
    private float jumpDelay;
    private float yVel = 0;
    public bool _isGrounded;
    
    private Vector3 velosity;

    private Vector3 offset, dirSurface;
    private float speed;
    [HideInInspector]
    public Vector3 surfaceNormal, hitSurfacePoint, myNormal;
    public Transform visual, groundPoint, forwardPoint;
    public Rigidbody _Rigidbody;
    private Vector3 toRotate;
    public bool canMove;
    public float axisH;
    void Start()
    {
        myNormal = transform.up;
        canMove = true;
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            if (!isJumped)
            {
                yVel = jumpPover;
                isJumped = true;
            }
        }
    }
    void Update()
    {
        
            _isGrounded = IsGrounded();
            RaycastHit hit;
            if (Physics.Raycast(groundPoint.position, -groundPoint.up, out hit, surfaceDistance, surfaceLayer))
            {
                surfaceNormal = hit.normal;
                hitSurfacePoint = hit.point;
            }
            else
            {
                surfaceNormal = Vector3.up;
                hitSurfacePoint = Vector3.up;
            }

            if (_Type == CharacterType.player)
            {
                myNormal = Vector3.Lerp(myNormal, surfaceNormal, rotarionSurfaceDamping * Time.deltaTime);
                toRotate = Vector3.up * axisH * rotationSpeed * Time.deltaTime;
                visual.Rotate(toRotate);
                var myForward = Vector3.Cross(transform.right, myNormal);
                var targetRot = Quaternion.LookRotation(myForward, myNormal);
                transform.rotation =
                    Quaternion.Lerp(transform.rotation, targetRot, rotarionSurfaceDamping * Time.deltaTime);
            }

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
                    yVel = 0;
                }

                if (speed < moveSpeed)
                {
                    speed += 15f * Time.deltaTime;
                }
            }
            else
            {
                if (speed > moveSpeed / 2f)
                {
                    speed -= 5f * Time.deltaTime;
                }

                yVel -= gravity * Time.deltaTime;
            }
        
    }

    private void FixedUpdate()
    {
        
            if (IsGrounded())
            {
                if (!isForwardWall())
                {
                    dirSurface = (visual.forward.normalized -
                                  Vector3.Dot(visual.forward.normalized, surfaceNormal) * surfaceNormal);
                    offset = (dirSurface) * (speed * Time.deltaTime);
                    _Rigidbody.MovePosition(_Rigidbody.position + offset);
                }

                if (isJumped)
                {
                    transform.position += transform.up * yVel * Time.deltaTime;
                }
            }
            else
            {
                dirSurface = (visual.forward.normalized -
                              Vector3.Dot(visual.forward.normalized, visual.up) * visual.up);
                offset = dirSurface * (speed * Time.deltaTime);
                _Rigidbody.MovePosition(_Rigidbody.position + offset + (transform.up * (yVel) * Time.deltaTime));
            }
        
    }
    public bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundPoint.position, -groundPoint.up, out hit, distanceToGround,
            surfaceLayer))
        {
            return true;
        }
        return false;
        
    }

    public bool isForwardWall()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(forwardPoint.position, forwardPoint.forward, out hit, forwardDistance,
            wallLayer))
        {
            return true;
        }
        return false;
    }
}
