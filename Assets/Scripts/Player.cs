using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static Player instance;
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

    public TMP_Text nameText;
    public IkController _IkController;
    private AdvancedWalkerController _walkerController;
    public CharacterKeyboardInput _CKI;
    public SmoothRotation _SmoothRotation;
    public Transform modelRoot,models,visual,groundPoint,checkPoint;

    public float moveSpeed,rotationSpeed;
    //[HideInInspector]
    public float speed;
    public float stopingForceIdle,stopingForceActive;
    public float surfaceDistance,rotarionSurfaceDamping;
    public LayerMask surfaceLayer;
    private float axisV,axisH;
    public float axisHToMove,axisVToMove;
    private Vector3 surfaceNormal;
    private PlayerAnimationController _AnimationController;
    private bool isJumped;
    private float jumpDelay;
    [HideInInspector] public bool canMove;

    public ParticleSystem snowParticle, speedParticle;
    public TrailRenderer boardTrail;
    
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _CKI = GetComponent<CharacterKeyboardInput>();
        _AnimationController = GetComponent<PlayerAnimationController>();
        _walkerController = GetComponent<AdvancedWalkerController>();
    }
    public void SetName(string name)
    {
        nameText.text = name;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundPoint.position,-groundPoint.up*surfaceDistance+groundPoint.position);
    }


    public void Finish()
    {
        GameManager.instance.PlayerFinished();
        canMove = false;
        speed = 0;
        axisHToMove = axisVToMove = 0;
        _IkController.SetSpeed(0);
        PlayerSFX.instance.PauseRunBoard();
    }
    void Update()
    {
        RotateOnSurface();
        RotateModels();
        CheckAnimation();
        if (canMove)
        {
            axisV = _CKI.GetVerticalMovementInput();
            axisH = _CKI.GetHorizontalMovementInput();

            if (_walkerController.IsGrounded())
            {
                if (jumpCount != 0) jumpCount = 0;
                boardTrail.emitting = true;
                if (speed > 2f)
                {
                    if (snowParticle.isPlaying == false)
                    {
                        PlayerSFX.instance.PlayBoard(1);
                        snowParticle.Play();
                    }
                }
                else
                {
                    PlayerSFX.instance.PauseRunBoard();
                    snowParticle.Stop();
                }
                if (axisV != axisVToMove)
                {
                    axisVToMove = Mathf.Lerp(axisVToMove, axisV, 2f * Time.deltaTime);
                }
            }
            else
            {
                boardTrail.emitting = false;
                if (snowParticle.isPlaying)
                {
                    snowParticle.Stop();
                    PlayerSFX.instance.PauseRunBoard();
                }

                if (axisV != axisVToMove)
                {
                    axisVToMove = Mathf.Lerp(axisVToMove, axisV, 1f * Time.deltaTime);
                }
            }

            if (speed > moveSpeed / 2f)
            {
                speedParticle.Play();
            }
            else
            {
                speedParticle.Stop();
            }
            
            if (isJumped)
            {
                if (jumpDelay > 0)
                {
                    jumpDelay -= 1f * Time.deltaTime;
                }
                else
                {
                    jumpDelay = 0;
                    isJumped = false;
                }
            }

            
            if (axisV > 0)
            {
                if (axisV > 0.8f)
                {
                    if (axisH > -0.4f && axisH < 0.4f)
                    {
                        axisHToMove = axisH;
                    }
                    else
                    {
                        axisHToMove = Mathf.Lerp(axisHToMove, axisH, 2f * Time.deltaTime);
                    }
                }
                else
                {
                    if (speed < 10)
                    {
                        axisHToMove = axisH;
                    }
                    else
                    {
                        axisHToMove = Mathf.Lerp(axisHToMove, axisH, 2f * Time.deltaTime);
                    }
                }

                if (speed < moveSpeed)
                {
                    speed += axisV * 10f * Time.deltaTime;
                }
                else if (speed > moveSpeed * 1.5f)
                {
                    speed -= stopingForceIdle * Time.deltaTime;
                }
            }
            else if (axisV > -0.5f)
            {
                if (axisH != axisHToMove)
                {
                    axisHToMove = Mathf.Lerp(axisHToMove, axisH, 2f * Time.deltaTime);
                }
                if (axisH < -0.55f || axisH > 0.55f)
                {
                    if (speed < moveSpeed)
                    {
                        speed += Mathf.Abs(axisV) * 10f * Time.deltaTime;
                    }
                    else if (speed > moveSpeed * 1.5f)
                    {
                        speed -= stopingForceIdle * Time.deltaTime;
                    }
                }
                else
                {
                    if (speed > 0)
                    {
                        speed -= stopingForceIdle * Time.deltaTime;
                    }
                }
            }
            else
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
            _IkController.SetSpeed(speed);
            /*if (axisH < -0.55f || axisH > 0.55f)
            {
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
                else
                {
                    if (speed > 10)
                    {
                        speed -= stopingForceIdle * Time.deltaTime;
                    }
                    else if (speed < 9.5f)
                    {
                        speed += 10f * Time.deltaTime;
                    }
                }
            }
            else if (axisV > 0)
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
            else if (axisV > -0.3f)
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
                    speed -= (-axisV) * stopingForceActive * Time.deltaTime;
                }
                else
                {
                    speed = 0;
                }
            }*/
        }
    }
    
    void RotateOnSurface()
    {
        RaycastHit hit;
        Debug.DrawRay(groundPoint.position, -groundPoint.up*surfaceDistance, Color.green);
        if (Physics.Raycast(groundPoint.position, -groundPoint.up, out hit, surfaceDistance, surfaceLayer))
        {
            surfaceNormal = hit.normal;
            if (!isJumped)
            {
                _walkerController.gravity = 10;
            }
        }
        else
        {
            surfaceNormal = Vector3.up;
            if (!isJumped)
            {
                _walkerController.gravity = 50;
            }
        }
        
        var targetRot = Quaternion.LookRotation(Vector3.Cross(transform.right, surfaceNormal), surfaceNormal);
        var fwd = Vector3.ProjectOnPlane(transform.forward, surfaceNormal);
        Quaternion slopeRotation = Quaternion.LookRotation(fwd, surfaceNormal);
        transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation, rotarionSurfaceDamping * Time.deltaTime);
            
        
    }

    void RotateModels()
    {
        if (axisH != 0)
        {
            Quaternion toRot = Quaternion.Euler(0, 0, -axisH * 15);
            visual.localRotation =
                Quaternion.Lerp(visual.localRotation, toRot, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion toRot = Quaternion.Euler(0, 0, 0);
            visual.localRotation =
                Quaternion.Lerp(visual.localRotation, toRot, rotationSpeed * Time.deltaTime);
        }
    }
    
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
            if (_walkerController.IsGrounded())
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
                if (_walkerController.IsGrounded())
                {
                    if (_animState != AnimState.jumpUp)
                    {
                        if (axisV < -0.5f)
                        {
                            if (axisH > -0.5f && axisH < 0.5f)
                            {
                                _AnimationController.PlayAnimationBoard("stop");
                            }
                        }
                        else
                        {
                            _AnimationController.PlayAnimationBoard("idle");
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
                if (_walkerController.IsGrounded())
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

     private int jumpCount;
     public void Jump()
     {
         if (canMove)
         {
             if (jumpCount < 1)
             {
                 _walkerController.JumpUI();
                 isJumped = true;
                 jumpDelay = 0.5f;
                 jumpCount++;
             }
         }
     }

     public void Boost(float force)
     {
         PlayerSFX.instance.PauseRunBoard();
         _walkerController.AddMomentum(transform.forward*force);
     }

     public void Jump(float force)
     {
         PlayerSFX.instance.PauseRunBoard();
         _walkerController.AddMomentum(transform.up*force);
     }
     public void ResetPositionOnCheckPoint()
     {
         transform.position = checkPoint.position;
         transform.rotation = Quaternion.Euler(Vector3.zero);
         speed = 0;
         modelRoot.localRotation = Quaternion.Euler(Vector3.zero);
         _walkerController.SetMomentum(Vector3.zero);
         _walkerController.cameraTransform.localEulerAngles =
             new Vector3(_walkerController.cameraTransform.localEulerAngles.x, 0, 0)
             ;
         _walkerController.cameraTransform.GetComponent<CMF.CameraController>().ResetRotation();
     }
}
