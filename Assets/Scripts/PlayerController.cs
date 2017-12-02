using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public float moveSpeed = 5f;

    public float rotationSpeed = 15f;

    public float arrivalDistanceMovement = 1f;

    public float arrivalDistanceRotation = 1f;

    public bool isMoving = false;

    public bool isRotating = false;

    Vector3 dir;

    Vector3 target;

    CharacterController controller;

    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        handleInput();
        if (isMoving)
            handleMovement();
        if (isRotating)
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
    }

    // maybe can check for clicking animation here lol
    void handleMovement()
    {
        if(Vector3.Distance(transform.position, target) < arrivalDistanceMovement)
        {
            isMoving = false;
        }
        else
        {
            controller.Move(dir * moveSpeed * Time.deltaTime);
        }
    }

    void handleRotation()
    {
        if (Vector3.Dot(dir, transform.forward) == arrivalDistanceRotation)
        {
            isRotating = false;
        }
        else
        {
            Quaternion rotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }
}
