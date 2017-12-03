using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASmithConjecture : MonoBehaviour {

    public float accelerateSpeed = 1;
    public float dampening = 1;
    public float mass = 1;

    Rigidbody rb;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) throw new System.Exception();

    }

    Vector3 lastPos = Vector3.zero;
    // Update is called once per frame
    void FixedUpdate()
    {
        var target = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
        var dir = (target - this.transform.position);// * (1 + (target - lastPos).sqrMagnitude);
        //var vel = dir - rb.velocity * Mathf.Min((rb.velocity-dir).sqrMagnitude, 1);
        var vel = dir - rb.velocity * Mathf.Min(Vector3.Cross((target - lastPos), rb.velocity).sqrMagnitude, 1);
        var drag = -rb.velocity * mass - rb.velocity * dampening;
        vel.z = 0;

        // Debug.Log("1: " + (target-lastPos) + " 2: " + rb.velocity);
        Debug.Log(Vector3.Cross((target - lastPos), rb.velocity).sqrMagnitude);
        // Debug.Log(Vector3.Cross(new Vector3(1,0,0), new Vector3(1.1f, 0.1f, 0)).sqrMagnitude);
        //Debug.Log(Mathf.Min((-rb.velocity + dir).sqrMagnitude, 1));

        rb.AddForce((vel * 50 + drag * 0.1f), ForceMode.Acceleration);
        lastPos = target;
    }
}
