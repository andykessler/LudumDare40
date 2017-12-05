using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// could inherit base values from parent controller class
public class OpponentController : MonoBehaviour {

    public const float MAX_SPEED = 50f;
    public float moveSpeed;
    public float rotationSpeed;
    public float arrivalDistanceMovement;
    public float arrivalDistanceRotation;

    Vector3 dir;
    Vector3 target;
    Rigidbody rb;
    BallCarrier self;
    
    // use this to do circles and make more intelligent paths
    public const float DEFAULT_CIRCLE_RADIUS = 25f;
    private float circlePathTimer;

    public const float DEFAULT_THROW_TIMER = 3f;
    private float throwBallTimer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        self = GetComponent<BallCarrier>();
        throwBallTimer = DEFAULT_THROW_TIMER;
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
                self.SendMessage("ThrowBall", t);
                throwBallTimer = DEFAULT_THROW_TIMER;
            }
        }
    }

    void HandleMovement()
    {
        dir = transform.forward;//(transform.position + transform.forward) - transform.position;
        //Vector3 f = transform.position + dir * Time.deltaTime;
        rb.AddForce(dir * moveSpeed, ForceMode.Impulse);
        //rb.AddForce(transform.position)

    }

    float changeDirectionTimer;
    Quaternion targetRotation;

    void HandleRotation()
    {
        if(changeDirectionTimer > 0f)
        {
            changeDirectionTimer -= Time.deltaTime;
        }
        else if(changeDirectionTimer <= 0f)
        {
            Quaternion rotation = Random.rotation;
            Vector3 angles = transform.rotation.eulerAngles;
            angles.y = (angles.y + rotation.eulerAngles.y) % 360;
            targetRotation = Quaternion.Euler(angles);
            changeDirectionTimer = Random.Range(1, 3f);

        }
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed)); // Time.deltaTime?

        // TODO Add random rotation amount, to simulate moving back/forth + zig-zag
        //transform.localEulerAngles += new Vector3(0f, rotationSpeed, 0f);


        //Quaternion rotation = Quaternion.LookRotation(dir);
        //rb.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed));
    }

    //bool IsLookingAtTarget()
    //{
    //    return Vector3.Dot(dir, transform.forward) >= arrivalDistanceRotation;
    //}
}
