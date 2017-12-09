using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    
    // public for debugging purposes, change later
    public float arrivalDistanceMovement;
    public float arrivalDistanceRotation;
    public bool isMoving = false;
    public bool isRotating = false;

    private Vector3 dir;
    private Vector3 target;
    private Rigidbody rb;
    private BallCarrier self;


    //Plane plane = new Plane(Vector3.up, Vector3.zero);
    public Plane plane;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        target = Vector3.zero;
        dir = Vector3.zero;
        self = GetComponent<BallCarrier>();
        isMoving = false;
        isRotating = false;
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > PlayerProperties.maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * PlayerProperties.maxSpeed;
        }
        rb.AddForce(PlayerProperties.speed * dir.normalized, ForceMode.Impulse);
    }

 
    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float ent;
            if (plane.Raycast(ray, out ent))
            {
                Debug.Log("Plane Raycast hit at distance: " + ent);
                Vector3 hitPoint = ray.GetPoint(ent);
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = hitPoint;
                Debug.DrawRay(ray.origin, ray.direction * ent, Color.green);
            }
            else
                Debug.DrawRay(ray.origin, ray.direction * 300, Color.red);

        }
    }

    // Update is called once per frame
    void Update () {
        HandleInput();
        if(isMoving || isRotating)
        {
            dir = Vector3.Normalize(target - transform.position);
            if(!IsLookingAtTarget())
            {
                isRotating = true;
            }
        }
        HandleMovement();
        HandleRotation();
	}

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        plane.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }


    void HandleInput()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButtonUp(1)) // Right click up event
        {
            Vector3 result = GetWorldPositionOnPlane(Input.mousePosition, Camera.main.transform.position.z);

            if (result != null)
            {
                target = result;
                target.y = transform.position.y;
                dir = Vector3.Normalize(target - transform.position);
                isMoving = true;
                isRotating = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit, LayerMask.GetMask("Opponents")))
            {
                // Use sendmessage to check these things instead?
                if (hit.rigidbody != null) // check it is also tagged "Opponent"? Redundant? 
                {
                    BallCarrier throwTarget = hit.rigidbody.transform.GetComponent<BallCarrier>();
                    if(throwTarget != null)
                    {
                        Debug.Log(throwTarget.transform.name);
                        self.SendMessage("ThrowBall", throwTarget);
                    }
                    else
                    {
                        Debug.Log("That's not a BallCarrier.");
                    }
                }
            }
        }
    }

    // maybe can check for clicking animation here lol
    void HandleMovement()
    {
        if (!isMoving) return;
        if(Vector3.Distance(transform.position, target) < arrivalDistanceMovement)
        {
            isMoving = false;
        }
    }

    void HandleRotation()
    {
        if (!isRotating) return;
        if (Vector3.Dot(dir, transform.forward) >= arrivalDistanceRotation)
        {
            isRotating = false;
        }
        Quaternion rotation = Quaternion.LookRotation(dir);
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, PlayerProperties.rotationSpeed));
        
    }

    bool IsLookingAtTarget()
    {
        return Vector3.Dot(dir, transform.forward) >= arrivalDistanceRotation;
    }
}
