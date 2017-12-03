using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHunter : MonoBehaviour {
    
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
                target = value.Owner.transform;
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
    
    public bool isChasing;

    public float rotationSpeed;

    public float GLOBAL_DRAG = 0.1f;
    float globalDrag; // TODO Set this somewhere

    public float DEFAULT_DAMPENING;
    public float MAX_DAMPENING;
    float dampening;

    public const float MAX_DAMPEN_TIME = 15f;
    public float currDampenTime = MAX_DAMPEN_TIME;

    public float maxSpeed; // is there a maximum backwards speed?

    public float DEFAULT_ACCELERATION;
    public float MAX_ACCELERATION;
    float acceleration;
   
    private Transform target; // owner of ball
    private Rigidbody targetRb;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isChasing) return;
        //currDampenTime = Mathf.Max(currDampenTime-Time.deltaTime, 0f); // don't go below 0
        //float dampenRatio = currDampenTime / MAX_DAMPEN_TIME; // better pattern to use?
        //float dampenRatioComp = 1f - dampenRatio; // complement
        acceleration = DEFAULT_ACCELERATION;// + (dampenRatioComp * (MAX_ACCELERATION - DEFAULT_ACCELERATION));
        //dampening = DEFAULT_DAMPENING + (dampenRatioComp * (MAX_DAMPENING - DEFAULT_DAMPENING));
        rb.velocity = Vector3.Min(maxSpeed * rb.velocity.normalized, rb.velocity);
        // FIXME predict logic is being weird?
        Vector3 predict = (target.transform.position - transform.position);// * (1f + (targetRb.velocity.magnitude * 5f)); // FIXME
        Vector3 diff = predict;/*- rb.velocity * Mathf.Min(Vector3.Cross(targetRb.velocity, rb.velocity).sqrMagnitude, 1f);*/
        Vector3 drag = -rb.velocity * rb.mass - rb.velocity * dampening; // attempt putting as rigid body property .drag
        rb.AddForce(diff*acceleration + drag*GLOBAL_DRAG, ForceMode.Acceleration);


        Debug.Log(rb.velocity);

        Quaternion rotation = Quaternion.LookRotation(diff.normalized);
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed));
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
            straightEnter = false;//Vector3.Dot(Precious.Owner.transform.position, transform.forward) > 1f - straightAngleThreshold;
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
        bool straightAngle = true;//Vector3.Dot(precious.Owner.transform.position.normalized, transform.forward) > 1f - straightAngleThreshold;
        bool momentumCheck = true;//Vector3.Magnitude(rb.mass * rb.velocity) < momentumThreshold;
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
