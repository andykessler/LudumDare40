using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public const float DEFAULT_BALL_HEIGHT = 1f;

    public delegate void RefreshEvent();
    event RefreshEvent refreshEvent;

    private BallCarrier owner;
    public BallCarrier Owner
    {
        get { return owner; }
        private set {
            owner = value;
            if(refreshEvent != null) refreshEvent();
        }
    }
    
    BallCarrier target;

    Rigidbody rb;

    public bool isMoving;

    float speed;


    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        //isMoving = false; // causing issues on init on gameloop
	}
	
	// Update is called once per frame
	void Update () {
        if (!isMoving)
        {
            if (Owner != null)
            {
                transform.position = Owner.transform.position + (Vector3.up * DEFAULT_BALL_HEIGHT); // set as a child instead?
            }
            return;
        }
        Vector3 dir = Vector3.Normalize(target.transform.position - transform.position); // rotate at all?
        rb.MovePosition(transform.position + (dir * speed * Time.deltaTime));
    }

    void ThrowTo(BallCarrier t)
    {
        if(isMoving)
        {
            Debug.Log("Can't throw the ball. Ball is already thrown.");
            return;
        }
        if (t.HasBall())
        {
            Debug.Log("Can't throw the ball. Target has a ball already.");
            return;
        }
        
        if(Owner != null)
        {
            transform.position = Owner.transform.position + (Vector3.up * DEFAULT_BALL_HEIGHT);
            speed = Owner.throwStrength;
        }
        else
        {
            speed = BallCarrier.DEFAULT_THROW_STRENGTH; // instant instead?
        }

        target = t;
        Owner = target;
        isMoving = true;

    }

    void OnTriggerEnter(Collider other)
    {
        if(isMoving && target != null && other.GetComponent<BallCarrier>() == target)
        {
            target.SendMessage("ReceiveBall", this);
            isMoving = false;
            // TODO Does does ball hide on possession? or do you visibly hold it somewhere? or just aura particles
        }
    }

    public void RegisterToRefresh(RefreshEvent e)
    {
        // Check to see if we actually should be doing a remove before add
        refreshEvent -= e;
        refreshEvent += e;
    }

    public void UnregisterToRefresh(RefreshEvent e)
    {
        refreshEvent -= e;
    }
}
