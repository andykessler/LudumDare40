using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public Ball gameBall; // remove later, not final resting place.

    public float moveSpeed;

    public float rotationSpeed;

    public float arrivalDistanceMovement;

    public float arrivalDistanceRotation;

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
            // TODO If can't finish this, then instead just let gravity kill people with killbox?
            //else
            //{
            //    // if we didnt hit, check to see if we are still walking on ground
            //    // if we are, lets keep trying to walk towards mouse until then
            //    // otherwise stop movement and rotation.
            //    RaycastHit hit2;
            //    Ray ray = new Ray(transform.position, Vector3.down);
            //    if(!Physics.Raycast(ray, out hit2, LayerMask.GetMask("Ground")))
            //    {
            //        isMoving = false;
            //        isRotating = false;
            //    }
            //}
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


        // "start game"
        if (Input.GetKeyUp(KeyCode.Alpha3) && !carrier.HasBall())
        {
            // this is a hack.
            gameBall.transform.position = carrier.transform.position;
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
