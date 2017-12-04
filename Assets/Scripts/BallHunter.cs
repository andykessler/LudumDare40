using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHunter : MonoBehaviour {
    
    public bool isChasing;
    //public const float MAX_DAMPEN_TIME = 15f;

    [Header("Path Values - READ ONLY")]
    public float dampening;
    public float maxSpeed; // is there a maximum backwards speed?
    public float acceleration;
    //public float forwardSpeed;
    //public float currDampenTime = MAX_DAMPEN_TIME;
    public float rotationSpeed;

    [Space(10)]

    [Header("Path Constants")]
    public float GLOBAL_DRAG;
    public float DEFAULT_DAMPENING;
    public float MAX_DAMPENING;
    [Space(10)]
    public float DEFAULT_ACCELERATION;
    public float MAX_ACCELERATION;
    public float MAX_ACCEL_DISTANCE;
    [Space(10)]
    public float DEFAULT_FORWARD_SPEED;
    public float MAX_FORWARD_SPEED;

    // my precious!!!
    private Ball precious;
    public Ball Precious
    {
        get { return precious; }
        set {
            if (precious != null)
            {
                precious.UnregisterToRefresh(OwnerChanged);
            }
            precious = value;
            if(value != null)
            {
                // should this be broadcasted somewhere else
                value.RegisterToRefresh(OwnerChanged);
                if (value.Owner != null)
                    target = value.Owner.transform;
                else
                    target = value.transform;
                targetRb = target.GetComponent<Rigidbody>(); 
            }
        }
    }

    private bool hasTakenPrecious;
    public bool HasTakenPrecious
    {
        get { return hasTakenPrecious; }
        private set {
            hasTakenPrecious = value;
        }
    }
   
    private Transform target; // owner of ball
    private Rigidbody targetRb;
    private Rigidbody rb;

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //forwardSpeed = DEFAULT_FORWARD_SPEED;
        acceleration = DEFAULT_ACCELERATION;
    }

    public Vector3 vel = Vector3.zero, diffMem = Vector3.zero, mydrag = Vector3.zero, a = Vector3.zero, f = Vector3.zero;
    public float distanceRatio;
    public float radius;
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, vel + vel.normalized*radius);

        if(target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, a + a.normalized*radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, f + f.normalized*radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, mydrag + mydrag.normalized*radius);
    }

    void FixedUpdate()
    {
        if (!isChasing) return;
        //currDampenTime = Mathf.Max(currDampenTime-Time.deltaTime, 0f); // don't go below 0
        //float dampenRatio = currDampenTime / MAX_DAMPEN_TIME; // better pattern to use?
        //float dampenRatioComp = 1f - dampenRatio; // complement

        //acceleration = DEFAULT_ACCELERATION;// + (dampenRatioComp * (MAX_ACCELERATION - DEFAULT_ACCELERATION));
        //dampening = DEFAULT_DAMPENING + (dampenRatioComp * (MAX_DAMPENING - DEFAULT_DAMPENING));
        rb.velocity = Vector3.Min(rb.velocity, rb.velocity.normalized * MAX_FORWARD_SPEED);
        acceleration = Mathf.Min(acceleration + Time.deltaTime, MAX_ACCELERATION);

        Vector3 diff = target.transform.position - transform.position;

        // FIXME predict logic is being weird?
        //Vector3 predict = diff;// * (1f + (targetRb.velocity.magnitude * 5f)); // FIXME
        //predict = predict - rb.velocity * Mathf.Min(Vector3.Cross(targetRb.velocity, rb.velocity).sqrMagnitude, 1f);
    
        //forwardSpeed = Mathf.Min(forwardSpeed + Time.deltaTime, MAX_FORWARD_SPEED);

        Vector3 drag = -rb.velocity * (rb.mass + dampening); // I think this is equivalent to the above.
        distanceRatio = 1f;// (Mathf.Min(diff.magnitude / MAX_ACCEL_DISTANCE, 1f));
        Vector3 force = (diff.normalized * acceleration * distanceRatio); //+ (transform.forward * forwardSpeed);
        f = force;
        //Vector3 dmin = Vector3.Min(diff, diff.normalized * drag.magnitude * GLOBAL_DRAG);
        a = force + (drag * GLOBAL_DRAG);
        rb.AddForce(force + (drag*GLOBAL_DRAG), ForceMode.Force);

        // as dampening increases, our drag increases
        // as we get faster we also apply more drag
        // further away from target, we apply more acceleration,

        //Debug.Log(rb.velocity);

        Quaternion rotation = Quaternion.LookRotation(diff.normalized);
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed));


        // delete this soon just for debugging/inspector
        vel = rb.velocity;
        diffMem = diff;
        mydrag = drag;
    }

    void FindPrecious()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        if(balls.Length > 0)
        {
            // TODO take first one for now, later get specific?
            Debug.Log("Found a precious!");
            Precious = balls[0].GetComponent<Ball>();
            HasTakenPrecious = false;
        }
        else
        {
            Debug.Log("Could not find any precious!");
        }
    }

    void ChasePrecious()
    {
        if(Precious != null)
        {
            Debug.Log("Chasing my precious!");
            isChasing = true;
        }
        else
        {
            Debug.Log("No precious to chase!");
            isChasing = false; // does this make sense to be here?
            rb.velocity = Vector3.zero;
        }
    }

    void StopChasePrecious()
    {
        if(Precious != null)
        {
            Debug.Log("Stopping the chase for my precious!");
        }
        else
        {
            Debug.Log("No precious to stop chasing!");
        }
        isChasing = false;
        rb.velocity = Vector3.zero;
    }

    void ToggleChasePrecious()
    {
        if (isChasing) {
            StopChasePrecious();
        }
        else {
            ChasePrecious();
        }
    }

    void OwnerChanged() // this could change signature to accept Owner param
    {
        Debug.Log("My precious has changed owners");
        target = Precious.Owner.transform;
        targetRb = target.GetComponent<Rigidbody>();
    }

    void StealPrecious()
    {
        if(!HasTakenPrecious)
        {
            Debug.Log("Stealing my precious!");
            StopChasePrecious();
            Precious.Owner.SendMessage("Kill");
            //Precious = null; // this messed stuff up kinda
            // caused the creation of hastakenprecious bool
            HasTakenPrecious = true;
            rb.AddForce(-rb.velocity, ForceMode.VelocityChange);
            // TODO Negate any rotational forces?
            
        }
        else
        {
            Debug.Log("Already stole my precious!");
        }
    }

    bool ownerCollided;
    bool straightEnter;
    float overlapPercentThreshold = 0.2f; // at least 42% overlap between colliders
    float straightAngleThreshold = 0.30f; // dot product check (0-1) unless you want to look behind?
    float momentumThreshold = 1f; // max momentum to kill IF you entered collision backwards

    void OnTriggerEnter(Collider other)
    {
        if (ownerCollided || Precious == null) return;
        // other.SendMessage("Kill"); // lets see if this is valid
        BallCarrier carrier = other.transform.GetComponent<BallCarrier>();
        ownerCollided = carrier != null && carrier == Precious.Owner;
        if(ownerCollided)
        {
            straightEnter = true;//Vector3.Dot(Precious.Owner.transform.position, transform.forward) > 1f - straightAngleThreshold;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!ownerCollided || Precious == null || HasTakenPrecious) return;
        // Check collision was deep (i.e. a high percentage of collider overlap)
        // Check that you went in towards carrier not backing up into it
        // Check momentum values if didn't enter collision at straight angle
        // These 3 checks together determine if you can steal the precious.
        bool overlapPercent = Vector3.Distance(Precious.Owner.transform.position, transform.position) < overlapPercentThreshold;
        bool straightAngle = false;//Vector3.Dot(precious.Owner.transform.position.normalized, transform.forward) > 1f - straightAngleThreshold;
        bool momentumCheck = false;//Vector3.Magnitude(rb.mass * rb.velocity) < momentumThreshold;
        if(overlapPercent && (straightEnter || (straightAngle && momentumCheck)))
        {
            StealPrecious();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!ownerCollided || Precious == null) return;
        BallCarrier carrier = other.transform.GetComponent<BallCarrier>();
        ownerCollided = !(carrier != null && carrier == Precious.Owner);
        straightEnter = false; // probably not neccesary since resets in ontriggerenter
    }

}
