using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GravityTest : MonoBehaviour
{
    public static GravityTest instance;
    
    private Rigidbody _Rigidbody;
    public float moveSpeed,rotationSpeed, jumpPower;
    public Transform visual,modelsPoint,checkPoint;
    public Transform groundPoint,groundPoint2;
    public float distanceToGround = 0.6f;
    public LayerMask surfaceLayer;
    public float surfaceDistance = 3f;
    
    public float gravity = 9.8f;
    public float fallMultipluer;
    public float rotarionSurfaceDamping;
    public float stopingForceIdle, stopingForceActive;
    public float speed;

    private float axisV, axisH;

    private Vector3 surfaceNormal, hitSurfacePoint,myNormal,toRotate;

    private VariableJoystick _joystick;

    public Vector3 debugVelocity;
    public bool isGround;

    public bool isJumped, isAirJumped;
    private float jumpDelay;

    public Vector3 upVelocity;

    private PlayerAnimationController _AnimationController;
    public TMP_Text ddd;

    public CameraController _CameraController;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _joystick = UIController.instance._Joystick;
        myNormal = transform.up;
        _Rigidbody = GetComponent<Rigidbody>();
        _AnimationController = GetComponent<PlayerAnimationController>();
    }

    public Vector3 GetVelocity()
    {
        return _Rigidbody.velocity;
    }

    void Update()
    {
        CheckAnimation();
        debugVelocity = _Rigidbody.velocity;
        //axisV = Input.GetAxisRaw("Vertical");
        //axisH = Input.GetAxisRaw("Horizontal");
        axisV = _joystick.Vertical;
        axisH = _joystick.Horizontal;
        ddd.text = "AxisH-" + axisH + " | rSpeed-" + rotationSpeed * Time.deltaTime;
        if (debugVelocity == Vector3.zero)
        {
            //speed = 0;
        }
        
        if (IsGrounded())
        {
            if (isAirJumped)
            {
                isAirJumped = false;
            }
            if (axisV > 0)
            {
                if (speed < moveSpeed)
                {
                    speed += axisV * 10f * Time.deltaTime;
                }
                else if (speed > moveSpeed * 1.5f)
                {
                    speed -= stopingForceIdle * Time.deltaTime;
                }
            }
            else if(axisV>-0.3f)
            {
                if (speed > 0)
                {
                    speed -= stopingForceIdle * Time.deltaTime;
                }
                else
                {
                    speed = 0;
                }
            }
            else
            {
                if (speed > 0)
                {
                    speed -= (-axisV)*stopingForceActive * Time.deltaTime;
                }
                else
                {
                    speed = 0;
                }
            }
        }
        else
        {
            if (!isJumped)
            {
                if (axisV > 0)
                {
                    if (speed > moveSpeed / 2f)
                    {
                        speed -= (5f) * Time.deltaTime;
                    }
                    else
                    {
                    }
                }
                else
                {
                    if (speed > 0)
                    {
                        speed -= (5f) * Time.deltaTime;
                    }
                    else
                    {
                        speed = 0;
                    }
                }
            }
            else
            {
                if (axisV > 0)
                {
                    if (speed < moveSpeed)
                    {
                        speed += axisV * 10f * Time.deltaTime;
                    }
                }
            }
        }
        RotateOnSurface();
        RotateModels();
        isGround = IsGrounded();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump(jumpPower,0.5f);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Boost(60);
        }

        if (isJumped)
        {
            if (jumpDelay > 0)
            {
                jumpDelay -= 1f * Time.deltaTime;
            }
            else
            {
                jumpDelay =0;
                isJumped = false;
            }
        }
    }

    public void Jump(float force, float time)
    {
        if (!isAirJumped)
        {
            upVelocity = transform.up * force;
            isJumped = true;
            jumpDelay = time / 2f;
            DOTween.To(() => upVelocity, x => upVelocity = x, new Vector3(0, 0, 0), time);
            isAirJumped = true;
        }
        else
        {
            if (IsGrounded())
            {
                upVelocity = transform.up * force;
                isJumped = true;
                jumpDelay = time / 2f;
                DOTween.To(() => upVelocity, x => upVelocity = x, new Vector3(0, 0, 0), time);
            }
        }
    }
    
    private void FixedUpdate()
    {
        _Rigidbody.velocity = visual.forward * speed + upVelocity;

        if (!isJumped)
        {
            if (!IsGrounded())
            {
                _Rigidbody.velocity += transform.up.normalized * (-gravity);
                gravity += fallMultipluer * Time.fixedDeltaTime;
            }
            else
            {
                gravity = 9.8f;
            }
        }
    }


    void RotateOnSurface()
    {
        RaycastHit hit;
        Debug.DrawRay(groundPoint.position, -groundPoint.up*surfaceDistance, Color.green);
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


        toRotate = Vector3.up * axisH*rotationSpeed*Time.deltaTime;
        //visual.localRotation = Quaternion.Lerp(visual.localRotation,visual.localRotation*Quaternion.Euler( Vector3.up * axisH),rotationSpeed*Time.deltaTime);
        //visual.Rotate(Vector3.up * axisH*rotationSpeed*Time.deltaTime);
        //visual.Rotate(toRotate);
        //visual.localRotation = Quaternion.Lerp(visual.localRotation,);
        _CameraController.RotateCam(toRotate);
        var targetRot = Quaternion.LookRotation(Vector3.Cross(transform.right, surfaceNormal), surfaceNormal);
        transform.rotation =
            Quaternion.Lerp(transform.rotation, targetRot, rotarionSurfaceDamping * Time.deltaTime);
    }
    float currentXAngle = 0f;
    float currentYAngle = 0f;
    public float _lookSpeed;
   
    
    void RotateModels()
    {
        if (axisH != 0)
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
    public void Boost(float boost)
    {
        speed = boost;
    }
    bool IsGrounded()
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

    public void JumpDefoult()
    {
        Jump(jumpPower,0.5f);
    }
    
    public void ResetPositionOnCheckPoint()
    {
        _Rigidbody.velocity = Vector3.zero;
        transform.position = checkPoint.position;
        //currentTargetPlatformID = 0;
        visual.localRotation = Quaternion.Euler(Vector3.zero);
        gravity = 9.8f;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        speed = 0;
        _CameraController.ResetCamera();
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
                        if (axisV < -0.5f)
                        {
                            _AnimationController.PlayAnimationBoard("stop");
                        }
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != surfaceLayer)
        {
            //speed = 0;
            Debug.Log("ENTER "+other.gameObject.name);
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer != surfaceLayer)
        {
            //speed = 0;
            Debug.Log("EXIT "+other.gameObject.name);
        }
    }
}
