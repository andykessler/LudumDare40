using UnityEngine;

// could inherit base values from parent controller class
public class OpponentController : MonoBehaviour {

    private BallCarrier self;
    private Rigidbody rb;
    
    private Vector3 dir;
    private Vector3 target;
    private Quaternion targetRotation;

    private float throwBallTimer;
    private float changeDirectionTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        target = Vector3.zero;
        dir = Vector3.zero;
        self = GetComponent<BallCarrier>();
        throwBallTimer = Random.Range(OpponentProperties.minThrowTimer, OpponentProperties.maxThrowTimer);
    }
    
    void FixedUpdate()
    {
        if (rb.velocity.magnitude > OpponentProperties.maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * OpponentProperties.maxSpeed;
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
                    throwBallTimer = Random.Range(OpponentProperties.minThrowTimer, OpponentProperties.maxThrowTimer);
                }
            }
        }
    }

    void HandleMovement()
    {
        dir = transform.forward;
        rb.AddForce(dir * OpponentProperties.speed, ForceMode.Impulse);
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
            changeDirectionTimer = Random.Range(OpponentProperties.minRotateTimer, OpponentProperties.maxRotateTimer);

        }
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, OpponentProperties.rotationSpeed)); // Time.deltaTime
    }
 
}
