using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHunter : MonoBehaviour {
    [Header("Path Values - READ ONLY")]
    public float dampening;
    public float acceleration;
    //public float forwardSpeed;
    //public float currDampenTime = MAX_DAMPEN_TIME;
    public float rotationSpeed;
    [Space(10)]
    [Header("Path Constants")]
    public float DEFAULT_DAMPENING;
    public float MAX_DAMPENING;
    [Space(10)]
    public float DEFAULT_ACCELERATION;
    public float MAX_ACCELERATION;
    public float MAX_ACCEL_DISTANCE;
    [Space(10)]
    public float DEFAULT_FORWARD_SPEED;
    public float MAX_FORWARD_SPEED;
    [Space(15)]

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
                //targetRb = target.GetComponent<Rigidbody>(); 
            }
            else
            {
                // if you were assigned NULL precious
                Debug.Log("Removed precious reference, adding to hunter free list");
                GameLoop.huntersFree.Add(this); // remove public visibility on this list later
            }
        }
    }

    [SerializeField]
    private bool hasTakenPrecious;
    public bool HasTakenPrecious
    {
        get { return hasTakenPrecious; }
        private set {
            hasTakenPrecious = value;
        }
    }

    [SerializeField]
    private bool isChasing;
    //public const float MAX_DAMPEN_TIME = 15f;

    private Transform target; // owner of ball
    //private Rigidbody targetRb; // was used for prediction pathing
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //forwardSpeed = DEFAULT_FORWARD_SPEED;
        rb.velocity = Vector3.zero; // or give initial speed?
        acceleration = DEFAULT_ACCELERATION;
        dampening = DEFAULT_DAMPENING;
        
    }
    
    [Space(5)]
    // this is just editor stuff to check values quickly...
    public Vector3 vel = Vector3.zero, diffMem = Vector3.zero, mydrag = Vector3.zero, a = Vector3.zero, f = Vector3.zero;
    //public float distanceRatio;
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
        //distanceRatio = 1f;// (Mathf.Min(diff.magnitude / MAX_ACCEL_DISTANCE, 1f));
        
        //Vector3 drag = -rb.velocity * (rb.mass + dampening); // I think this is equivalent to the above.
        rb.drag = rb.mass + dampening; // I think this is equivalent to the above.
        Vector3 dv = rb.drag * -rb.velocity;

        Vector3 force = diff.normalized * acceleration;// * distanceRatio;
        Quaternion rotation = Quaternion.LookRotation(diff.normalized);

        rb.AddForce(force * Time.deltaTime, ForceMode.Impulse); // I might like impulse better :O
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed));

        // delete this soon just for debugging/inspector
        f = force;
        a = f + dv;
        vel = rb.velocity;
        diffMem = diff;
        mydrag = a;
    }

    void FindPrecious()
    {
        // FIXME If you stop chasing precious before you capture, they will not be put back into free list
        StopChasePrecious(); 
        Ball b = GameLoop.GetFreeBall();
        if (b != null)
        {
            Debug.Log("Found a precious!");
            Precious = b;
            HasTakenPrecious = false;
        }
        else
        {
            Debug.Log("Could not find any precious!");
            Precious = null; // what if you currently already have a Precious? Continue chasing that one?
        }
    }

    void ChasePrecious()
    {
        HasTakenPrecious = false;
        if (Precious != null)
        {
            Debug.Log("Chasing my precious!");
            isChasing = true;
        }
        else
        {
            Debug.Log(GameLoop.ballsFree.Count);
            Debug.Log("No precious to chase!");
            isChasing = false; // does this make sense to be here?
            dampening = DEFAULT_DAMPENING;
            acceleration = DEFAULT_ACCELERATION;
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
        dampening = DEFAULT_DAMPENING;
        acceleration = DEFAULT_ACCELERATION;
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
        Debug.Log("My precious has changed owners or has been claimed.");
        BallCarrier bc = Precious.Owner;
        if(bc != null)
        {
            target = bc.transform;
            //targetRb = target.GetComponent<Rigidbody>();
        }
        else
        {
            StopChasePrecious();
            Precious = null; // will null target
        }
    }

    void StealPrecious()
    {
        if(!HasTakenPrecious)
        {
            Debug.Log("Stealing my precious!");
            //StopChasePrecious(); // event handlers got this (on owner changed to null)
            Precious.Owner.SendMessage("Kill");
            Precious.Capture(this);

            //Precious = null; // this messed stuff up kinda
            // caused the creation of hastakenprecious bool
            // used for proper ontriggerexit event handling -- needed, prolly not?
            HasTakenPrecious = true;
            //Precious = null;

            // what about instead assigning to Vector3.zero;
            rb.AddForce(-rb.velocity, ForceMode.VelocityChange); // instantly stop our velocity
            // TODO Negate any rotational forces?
        }
        else
        {
            Debug.Log("Already stole my precious!");
        }
    }
    
    [Space(10)]
    [Header("Hit Checks")]
    public float overlapPercentThreshold; // at least 42% overlap between colliders
    //public float straightAngleThreshold; // dot product check (0-1) unless you want to look behind?
    //public float momentumThreshold; // max momentum to kill IF you entered collision backwards

    [Space(10)]
    public bool ownerCollided;
    //public bool straightEnter;
    public bool overlapPercent;
    //public bool straightAngle;
    //public bool momentumCheck;

    void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "Wall") rb.velocity = -rb.velocity * 0.3f; // lose some speed
        if (ownerCollided || Precious == null) return;
        BallCarrier carrier = other.transform.GetComponent<BallCarrier>();
        ownerCollided = carrier != null && carrier == Precious.Owner;
        //if(ownerCollided)
        //{
        //    //straightEnter = Vector3.Dot(Precious.Owner.transform.position, transform.forward) > 1f - straightAngleThreshold;
        //    Debug.Log(Quaternion.Angle(transform.rotation, Quaternion.LookRotation(Precious.Owner.transform.position)));
        //    straightEnter = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(Precious.Owner.transform.position)) < straightAngleThreshold;
        //}
    }

    void OnTriggerStay(Collider other)
    {
        if (!ownerCollided || Precious == null || HasTakenPrecious) return;
        // Check collision was deep (i.e. a high percentage of collider overlap)
        // Check that you went in towards carrier not backing up into it
        // Check momentum values if didn't enter collision at straight angle
        // These 3 checks together determine if you can steal the precious.
        overlapPercent = Vector3.Distance(Precious.Owner.transform.position, transform.position) < overlapPercentThreshold;
        //straightAngle = Vector3.Dot(precious.Owner.transform.position.normalized, transform.forward) > 1f - straightAngleThreshold;
        //straightAngle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(Precious.Owner.transform.position)) < straightAngleThreshold;
        //momentumCheck = Vector3.Magnitude(rb.mass * rb.velocity) < momentumThreshold;
        if(overlapPercent/* && (straightEnter || (straightAngle && momentumCheck))*/)
        {
            StealPrecious();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!ownerCollided || Precious == null) return;
        BallCarrier carrier = other.transform.GetComponent<BallCarrier>();
        ownerCollided = !(carrier != null && carrier == Precious.Owner);
        //straightEnter = false; // probably not neccesary since resets in ontriggerenter
    }
}