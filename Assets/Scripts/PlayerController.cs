using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public PlayerAnimationController _AnimationController;
    public Transform modelsPoint;
    public VariableJoystick _Joystick;

    public int currentTargetPlatformID;
    public Transform checkPoint;
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
    private Vector3 velosity;

    private Vector3 offset, dirSurface;
    private float speed;
    [HideInInspector]
    public Vector3 surfaceNormal, hitSurfacePoint, myNormal;
    public Transform visual, groundPoint,groundPoint2, forwardPoint;
    public Rigidbody _Rigidbody;
    private Vector3 toRotate;
    public bool canMove;
    private float axisH,axisV;

    private float defaultJumpPower, defaultGravity;

    public float inputJump, inputGravity;
    private float boostSpeed=1,boostSpeedDown=1;

    public TrailRenderer _Trail;
    
    void Start()
    {
        defaultGravity = gravity;
        defaultJumpPower = jumpPover;
        myNormal = transform.up;
        canMove = true;
    }

    public void Jump(float _jumpPower, float _gravity)
    {

        if (!isJumped)
        {
            yVel = _jumpPower;
            gravity = _gravity;
            isJumped = true;
            _AnimationController.PlayAnimationBoard("jumpUp");
        }

    }

    public void BTNJump()
    {
        Jump(inputJump,inputGravity);
    }
    void Update()
    {
        CheckAnimation();
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump(inputJump,inputGravity);
            }

            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                axisH = Input.GetAxisRaw("Horizontal");
            }
            else
            {
                axisH = _Joystick.Horizontal;
                if (_Joystick.Vertical >= 0)
                {
                    axisV = _Joystick.Vertical;
                }
                else
                {
                    if (axisV >= 0.1f)
                    {
                        axisV -= 1f * Time.deltaTime;
                    }
                    else
                    {
                        axisV = 0;
                    }
                }

                RotateModels();
            }

            RotateOnSurface();


            if (isJumped)
            {
                if (jumpDelay < 0.25f)
                {
                    jumpDelay += 1f * Time.deltaTime;
                }
                else
                {
                    gravity = defaultGravity;
                    jumpDelay = 0f;
                    isJumped = false;
                }
            }

            if (IsGrounded())
            {
                
                if (!isJumped)
                {
                    yVel = 0;
                    _Trail.emitting = true;
                }

                if (axisV > 0)
                {
                    if (speed < moveSpeed)
                    {
                        speed += axisV * 10f * Time.deltaTime;
                    }
                    else if (speed > moveSpeed * 1.5f)
                    {
                        speed -= axisV * (5f * boostSpeedDown) * Time.deltaTime;
                    }
                }
                else
                {
                    _Trail.emitting = false;
                    if (speed >0)
                    {
                        speed -=  ((-100f*axisV)+15f * boostSpeedDown) * Time.deltaTime;
                    }
                    else
                    {
                        speed = 0;
                    }
                }
            }
            else
            {
                _Trail.emitting = false;
                if (axisV > 0)
                {
                    if (speed > moveSpeed / 2f)
                    {
                        speed -=  (5f * boostSpeedDown) * Time.deltaTime;
                    }
                    else
                    {
                        boostSpeedDown = 1;
                    }
                }
                else
                {
                    if (speed >0)
                    {
                        speed -=  (5f * boostSpeedDown) * Time.deltaTime;
                    }
                    else
                    {
                        speed = 0;
                        boostSpeedDown = 1;
                    }
                }

                yVel -= gravity * Time.deltaTime;
            }
        }
    }

    enum AnimState
    {
        idle,
        stop,
        stopIdle,
        jumpUp,
        jumpIdle,
        jumpDown
    }

    private AnimState _animState;

    private float animDelay = 0f;
    void CheckAnimation()
    {
        if (animDelay > 0f)
        {
            animDelay -= 1f * Time.deltaTime;
            return;
        }
        else
        {
            animDelay = 0f;
        }
        if (axisV >0)
        {
            if (IsGrounded())
            {
                if (_animState != AnimState.jumpUp)
                {
                    _AnimationController.PlayAnimationBoard("idle");
                }
                else
                {
                    _AnimationController.PlayAnimationBoard("jumpDown");
                    _animState = AnimState.idle;
                    animDelay = 0.5f;
                }
            }
        }
        else
        {
            if (speed > 0)
            {
                if (IsGrounded())
                {
                    if (_animState != AnimState.jumpUp)
                    {
                        _AnimationController.PlayAnimationBoard("stop");
                    }
                    else
                    {
                        _AnimationController.PlayAnimationBoard("jumpDown");
                        _animState = AnimState.idle;
                        animDelay = 0.5f;
                    }
                }
                
            }
            else
            {
                if (IsGrounded())
                {
                    if (_animState != AnimState.jumpUp)
                    {
                        _AnimationController.PlayAnimationBoard("idle");
                    }
                    else
                    {
                        _AnimationController.PlayAnimationBoard("jumpDown");
                        _animState = AnimState.idle;
                        animDelay = 0.5f;
                    }
                }
            }
        }
        if (isJumped)
        {
            _animState = AnimState.jumpUp;
            _AnimationController.PlayAnimationBoard("jumpUp");
            
            if (axisV > 0.5f)
            {
                int r = Random.Range(0, 3);
                _AnimationController.Jump360(r);
            }
            animDelay = 0.5f;
        }
    }

    void RotateModels()
    {
        if (axisV > 0)
        {
            Quaternion toRot = Quaternion.Euler(0, 0, -axisH * 15);
            modelsPoint.localRotation =
                Quaternion.Lerp(modelsPoint.localRotation, toRot, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion toRot = Quaternion.Euler(0, 0, 0);
            modelsPoint.localRotation =
                Quaternion.Lerp(modelsPoint.localRotation, toRot, rotationSpeed * Time.deltaTime);
        }
    }

    void RotateOnSurface()
    {
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


        myNormal = Vector3.Lerp(myNormal, surfaceNormal, rotarionSurfaceDamping * Time.deltaTime);
        toRotate = Vector3.up * axisH * rotationSpeed;
        visual.localRotation = Quaternion.Euler(toRotate);
        var myForward = Vector3.Cross(transform.right, myNormal);
        var targetRot = Quaternion.LookRotation(myForward, myNormal);
        transform.rotation =
            Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRot.eulerAngles.x,0,targetRot.eulerAngles.z), rotarionSurfaceDamping * Time.deltaTime);
    }
    private void FixedUpdate()
    {
        if (canMove)
        {
            if (IsGrounded())
            {
                if (!isForwardWall())
                {
                    speed = speed*boostSpeed;
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
                speed = speed*boostSpeed;
                dirSurface = (visual.forward.normalized -
                              Vector3.Dot(visual.forward.normalized, visual.up) * visual.up);
                offset = dirSurface * (speed * Time.deltaTime);
                _Rigidbody.MovePosition(_Rigidbody.position + offset + (transform.up * (yVel) * Time.deltaTime));
            }
        }
    }
    

    public void ResetPositionOnCheckPoint()
    {
        _Rigidbody.velocity = Vector3.zero;
        transform.position = checkPoint.position;
        currentTargetPlatformID = 0;
        visual.localRotation = Quaternion.Euler(Vector3.zero);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        gravity = 9.8f;
        speed = 0;
        yVel = 0;
    }

    private Coroutine boostCoroutine;
    public void BoostSpeed(float speed)
    {
        if (boostCoroutine != null)
        {
            StopCoroutine(boostCoroutine);
        }

        boostCoroutine = StartCoroutine(Boost(speed));
    }
    IEnumerator Boost(float _speed)
    {
        boostSpeed = _speed;
        yield return new WaitForSeconds(2f);
        boostSpeedDown = 5f;
        boostSpeed = 1;
    }
    
    public bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundPoint.position, -groundPoint.up, out hit, distanceToGround,
            surfaceLayer))
        {
            return true;
        }
        if (Physics.Raycast(groundPoint2.position, -groundPoint2.up, out hit, distanceToGround,
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
