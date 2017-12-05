﻿using UnityEngine;

// could inherit base values from parent controller class
public class OpponentController : MonoBehaviour {

    public const float MAX_SPEED = 50f;
    public float moveSpeed;
    public float rotationSpeed;

    public const float DEFAULT_THROW_TIMER = 3f;
    private float throwBallTimer;
    private float changeDirectionTimer;

    Quaternion targetRotation;

    Vector3 dir;
    Vector3 target;
    Rigidbody rb;
    BallCarrier self;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        target = Vector3.zero;
        dir = Vector3.zero;
        self = GetComponent<BallCarrier>();
        throwBallTimer = Random.Range(DEFAULT_THROW_TIMER * .5f, DEFAULT_THROW_TIMER * 1.5f);
    }
    
    void FixedUpdate()
    {
        if (rb.velocity.magnitude > MAX_SPEED)
        {
            rb.velocity = rb.velocity.normalized * MAX_SPEED;
        }

        HandleInput();
        HandleMovement();
        HandleRotation();
    }

    void HandleInput()
    {
        if (self.HasBall())
        {
            if(throwBallTimer > 0)
            {
                throwBallTimer -= Time.deltaTime;
            }
            else if(throwBallTimer <= 0)
            {
                BallCarrier t = GameLoop.GetFreeBallCarrier();
                if(t != null)
                {
                    self.SendMessage("ThrowBall", t);
                    throwBallTimer = DEFAULT_THROW_TIMER;
                }
            }
        }
    }

    void HandleMovement()
    {
        dir = transform.forward;
        rb.AddForce(dir * moveSpeed, ForceMode.Impulse);
    }

    void HandleRotation()
    {
        // TODO Give them preferences towards origin so they don't run into walls
        if(changeDirectionTimer > 0f)
        {
            changeDirectionTimer -= Time.deltaTime;
        }
        else if(changeDirectionTimer <= 0f)
        {
            Quaternion rotation = Random.rotation; // put a range on this random val
            Vector3 angles = transform.rotation.eulerAngles;
            angles.y = (angles.y + rotation.eulerAngles.y) % 360;
            targetRotation = Quaternion.Euler(angles);
            changeDirectionTimer = Random.Range(0.25f, 1.5f); // extract to constant

        }
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed)); // Time.deltaTime
    }
 
}