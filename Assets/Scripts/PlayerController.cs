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

    void HandleInput()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButtonUp(1)) // Right click up event
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit, LayerMask.GetMask("Ground")))
            {
                target = hit.point;
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
                    Debug.Log(throwTarget.transform.name);
                    self.SendMessage("ThrowBall", throwTarget);
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
