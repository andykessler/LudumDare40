using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public const float MAX_SPEED = 50f;

    public float moveSpeed;

    public float rotationSpeed;

    public float arrivalDistanceMovement;

    public float arrivalDistanceRotation;

    public bool isMoving = false;

    public bool isRotating = false;

    Vector3 dir;

    Vector3 target;

    Rigidbody rb;

    BallCarrier self;

    private Vector3 fixedUpdateForce;// aka FU force

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        self = GetComponent<BallCarrier>();
	}

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > MAX_SPEED)
        {
            rb.velocity = rb.velocity.normalized * MAX_SPEED;
        }
        rb.AddForce(moveSpeed * dir.normalized, ForceMode.Impulse); // * Time.deltaTime; ?
    }

    // Update is called once per frame
    void Update () {
        handleInput();

        if(isMoving || isRotating)
        {
            dir = Vector3.Normalize(target - transform.position);
            if(!isLookingAtTarget())
            {
                isRotating = true;
            }
        }
        handleMovement();
        handleRotation();

	}

    void handleInput()
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

        // temp test code to force pass ball
        if (Input.GetKeyUp(KeyCode.Q) && !self.HasBall())
        {
            GameLoop.ballsFree[0].Owner.SendMessage("ThrowBall", self);
        }
    }

    // maybe can check for clicking animation here lol
    void handleMovement()
    {
        if (!isMoving) return;
        if(Vector3.Distance(transform.position, target) < arrivalDistanceMovement)
        {
            isMoving = false;
            //fixedUpdateForce = Vector3.zero;

        }
        else
        {
            //rb.AddForce(transform.position + (transform.forward * moveSpeed * Time.deltaTime));
            //rb.AddForce(moveSpeed * dir, ForceMode.Impulse); // * Time.deltaTime; ?
            fixedUpdateForce = moveSpeed * dir.normalized;
        }
    }

    void handleRotation()
    {
        if (!isRotating) return;
        if (Vector3.Dot(dir, transform.forward) >= arrivalDistanceRotation)
        {
            isRotating = false;
        }

        Quaternion rotation = Quaternion.LookRotation(dir);
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, rotationSpeed));
        
    }

    bool isLookingAtTarget()
    {
        return Vector3.Dot(dir, transform.forward) >= arrivalDistanceRotation;
    }
}
