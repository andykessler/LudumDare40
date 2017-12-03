using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public Ball gameBall; // remove later, not final resting place.

    public float moveSpeed = 5f;

    public float rotationSpeed = 15f;

    public float arrivalDistanceMovement = 1f;

    public float arrivalDistanceRotation = 0.95f;

    public bool isMoving = false;

    public bool isRotating = false;

    Vector3 dir;

    Vector3 target;

    Rigidbody rb;

    BallCarrier carrier;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        carrier = GetComponent<BallCarrier>();
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
                    carrier.SendMessage("ThrowBall", throwTarget);
                }
            }
        }

        // temp code to test passing ball
        if (Input.GetKeyUp(KeyCode.Q) && !carrier.HasBall())
        {
            gameBall.Owner.SendMessage("ThrowBall", carrier);
        }

        if (Input.GetKeyUp(KeyCode.Alpha3) && !carrier.HasBall())
        {
            gameBall.SendMessage("ThrowTo", carrier);
        }
    }

    // maybe can check for clicking animation here lol
    void handleMovement()
    {
        if (!isMoving) return;
        if(Vector3.Distance(transform.position, target) < arrivalDistanceMovement)
        {
            isMoving = false;
        }
        else
        {
            rb.MovePosition(transform.position + (dir * moveSpeed* Time.deltaTime));
        }
    }

    void handleRotation()
    {
        if (!isRotating) return;
        if (Vector3.Dot(dir, transform.forward) >= arrivalDistanceRotation)
        {
            isRotating = false;
        }
        else
        {
            Quaternion rotation = Quaternion.LookRotation(dir);
            rb.MoveRotation( Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed));
        }
    }

    bool isLookingAtTarget()
    {
        return Vector3.Dot(dir, transform.forward) >= arrivalDistanceRotation;
    }
}
