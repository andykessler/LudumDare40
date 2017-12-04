using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCarrier : MonoBehaviour {

    public const float DEFAULT_THROW_STRENGTH = 100f;
    
    public Ball ball;

    public float throwStrength = DEFAULT_THROW_STRENGTH; // can this be a maximum? and you throw weaker sometimes on purpose?

    public Vector3 spawnPosition;

    public Quaternion spawnQuaternion;

	// Use this for initialization
	void Start () {
        spawnPosition = transform.position;
        spawnQuaternion = transform.rotation;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}

    void ThrowBall(BallCarrier target)
    {
        if (!HasBall()) // what about sendmessage just not being able to find listener
        {
            Debug.Log("Can't throw the ball. No ball to throw!");
            return;
        }
        if (target.HasBall())
        {
            Debug.Log("Can't throw the ball. Target has a ball already!");
            return;
        }
        ball.SendMessage("ThrowTo", target); // pass strength parameter here? and change owner here?
        ball = null;
    }

    void ReceiveBall(Ball b)
    {
        if (HasBall())
        {
            Debug.Log("Can't receive the ball. Already have a ball!");
            return;
        }
        // is something else blocking you from getting a ball? other rules? distance?
        ball = b;
    }

    void Kill()
    {
        Debug.Log("Carrier killed.");
        transform.SetPositionAndRotation(spawnPosition, spawnQuaternion);
    }

    public bool HasBall()
    {
        return ball != null;
    }
}
