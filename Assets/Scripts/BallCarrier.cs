using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCarrier : MonoBehaviour {

    public const float DEFAULT_THROW_STRENGTH = 100f;
    
    public Ball ball;

    public float throwStrength = DEFAULT_THROW_STRENGTH; // can this be a maximum? and you throw weaker sometimes on purpose?

    public Vector3 spawnPosition;

    public Quaternion spawnQuaternion;

    public int currentLives; // add maxlives var?

	// Use this for initialization
	void Start () {
        spawnPosition = transform.position;
        spawnQuaternion = transform.rotation;
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
        GameLoop.KillAndRespawnIfHaveLife(this); // call this after short delay.
        // remove 1 from life
        // Call death animation/death script, aftwerwards...
        // hide carrier from display until we respawn
        // if remaining lives is greater than 0, respawn after certain amount of time

    }

    public void Respawn()
    {
        Debug.Log("Respawning Character");
        transform.SetPositionAndRotation(spawnPosition, spawnQuaternion);
        // show carrier once set in position
    }

    public bool HasBall()
    {
        return ball != null;
    }
}
