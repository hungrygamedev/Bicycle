using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
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
    public PlayerAnimationController _AnimationController;
    public Transform fallSensor;
    public IkController _IkController;

    

    public float fallSensorDistance;
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
    //[HideInInspector]
    public bool canMove;
    
    private float defaultJumpPower, defaultGravity;

    public float inputJump=15, inputGravity=20;
    public TrailRenderer _Trail;

    public GroundPlatform currentCheckPointPlatform;
    public int currentTargetPlatformID;
    public Transform checkPoint;
    public EnemyPath myPath;
    public  Transform currentTarget;
    public  int lastPathPointID;
    
    
    //public ParticleSystem snowParticle;
    public TrailRenderer boardTrail;
    void Start()
    {
        defaultGravity = gravity;
        defaultJumpPower = jumpPover;
        //inputJump = 15; inputGravity=20;
        myNormal = transform.up;
        
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }
    public void Jump(float _jumpPower, float _gravity)
    {
        if (IsGrounded())
        {
            if (!isJumped)
            {
                yVel = _jumpPower;
                gravity = _gravity;
                isJumped = true;
                _AnimationController.PlayAnimationBoard("jumpUp");
            }
        }
    }
    void Update()
    {
        CheckAnimation();
        if (canMove)
        {
            if (IsGrounded())
            {
                boardTrail.emitting = true;
                if (speed > 2f)
                {
                    //if (snowParticle.isPlaying == false)
                    //{
                    //    snowParticle.Play();
                   // }
                }
                else
                {
                  //  snowParticle.Stop();
                }
            }
            else
            {
                boardTrail.emitting = false;
               // snowParticle.Stop();
            }
            
            if (isFallSensorActive())
            {
                Jump(inputJump,inputGravity);
            }


            if (currentTarget != null)
            {
                Vector3 myPos = transform.position;
                Vector3 targetPos = currentTarget.position;
                myPos.y = 0;
                targetPos.y = 0;
                if (Vector3.Distance(myPos, targetPos) > 5f)
                {
                    RotateToTagret(currentTarget.position);
                }
                else
                {
                    GetNextPathPoint();
                }
            }
            else
            {
                GetNextPathPoint();
            }

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

            myNormal = Vector3.Lerp(myNormal, surfaceNormal, rotarionSurfaceDamping * Time.deltaTime);
            var myForward = Vector3.Cross(transform.right, myNormal);
            var targetRot = Quaternion.LookRotation(myForward, myNormal);
            transform.rotation =
                Quaternion.Lerp(transform.rotation, targetRot, rotarionSurfaceDamping * Time.deltaTime);

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
                _Trail.emitting = true;
                if (!isJumped)
                {
                    yVel = 0;
                }

                if (speed < moveSpeed)
                {
                    speed += 10f * Time.deltaTime;
                }
            }
            else
            {
                _Trail.emitting = false;
                if (speed > moveSpeed / 2f)
                {
                    speed -= 5f * Time.deltaTime;
                }

                yVel -= gravity * Time.deltaTime;
            }
        }
        _IkController.SetSpeed(speed);
        MoveForward();
    }

    public void Finish()
    {
        canMove = false;
        GameManager.instance.AddEnemyFinish(nameText.text);
        _IkController.SetSpeed(0);
    }

    public void Jump()
    {
        Jump(inputJump,inputGravity);
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
        else
        {
            
        }
        if (isJumped)
        {
            _animState = AnimState.jumpUp;
            _AnimationController.PlayAnimationBoard("jumpUp");
            
            
            int r = Random.Range(0, 3);
            _AnimationController.Jump360(r);
            
            animDelay = 0.5f;
        }
    }
    private void MoveForward()
    {
        if (canMove)
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

    public void ResetPositionOnCheckPoint()
    {
        currentCheckPointPlatform = null;
        _Rigidbody.velocity = Vector3.zero;
        transform.position = checkPoint.position;
        currentTargetPlatformID = 0;
        currentTarget = null;
        visual.localRotation = Quaternion.Euler(Vector3.zero);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        speed = 0;
        yVel = 0;
    }

    public void Boost(float force)
    {
        speed = force;
    }


    void GetNextPathPoint()
    {
        if (myPath != null)
        {
            currentTarget = myPath.GetNextPoint(lastPathPointID);
            if (currentTarget == null)
            {
                //GetNewPath();
            }
            else
            {
                lastPathPointID++;
            }
        }
        else
        {
            GetNewPath();
        }
    }
    void GetNewPath()
    {
        myPath = null;
        currentTargetPlatformID = WorldMap.instance.GetCurrentplatformID(currentCheckPointPlatform);
        EnemyPath newPath = WorldMap.instance.GetEnemyPath(currentTargetPlatformID+1);
        if (newPath != null)
        {
            myPath = newPath;
            lastPathPointID = -1;
            GetNextPathPoint();
            currentTargetPlatformID++;
        }
    }

    public void SetCheckPoint(Transform point, GroundPlatform chekPoint)
    {
        currentCheckPointPlatform = chekPoint;
        checkPoint = point;
        GetNewPath();
    }
    
    void RotateToTagret(Vector3 _target)
    {
        var lookPos = _target - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        visual.localRotation = Quaternion.Slerp(visual.localRotation, rotation, Time.deltaTime * rotationSpeed);
    }

    bool isFallSensorActive()
    {
        RaycastHit hit;
        if (Physics.Raycast(fallSensor.position, -fallSensor.up, out hit, fallSensorDistance,
            surfaceLayer))
        {
            return false;
        }
        return true;
    }
    
    
    
}
