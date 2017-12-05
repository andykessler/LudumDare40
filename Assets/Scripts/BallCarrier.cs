using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCarrier : MonoBehaviour {

    public const float DEFAULT_THROW_STRENGTH = 200f;
    
    public Ball ball;

    public float throwStrength = DEFAULT_THROW_STRENGTH; // can this be a maximum? and you throw weaker sometimes on purpose?

    public Vector3 spawnPosition;

    public Quaternion spawnQuaternion;

    public int currentLives; // add maxlives var?

    Renderer[] renderers;

	// Use this for initialization
	void Start () {
        spawnPosition = transform.position;
        spawnQuaternion = transform.rotation;
        renderers = GetComponentsInChildren<Renderer>();
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
        if (HasBall()) // redundant because only this is called by trigger, and you already have been assigned
        {
            // Debug.Log("Can't receive the ball. Already have a ball!");
            return;
        }
        // is something else blocking you from getting a ball? other rules? distance?
        ball = b;
    }

    void Kill()
    {
        Debug.Log("Carrier killed: " + name);

        enabled = false;
        foreach (Renderer r in renderers) r.enabled = false; // also disable any controller on it?

        // TODO hacky we will fix this with inheritance later...
        if(name.StartsWith("Human"))
        {
            GetComponent<PlayerController>().enabled = false;
        } else
        {
            GetComponent<OpponentController>().enabled = false;
        }

        GameLoop.KillAndRespawnIfHaveLife(this); // call this after short delay.
        // remove 1 from life
        // Call death animation/death script, aftwerwards...
        // hide carrier from display until we respawn
        // if remaining lives is greater than 0, respawn after certain amount of time

    }

    public void Respawn()
    {
        Debug.Log("Respawning carrier: " + name);

        enabled = true;
        foreach (Renderer r in renderers) r.enabled = true; // also enable any controller on it?

        // TODO hacky we will fix this with inheritance later...
        if (name.StartsWith("Human"))
        {
            GetComponent<PlayerController>().enabled = true;
        }
        else
        {
            GetComponent<OpponentController>().enabled = true;
        }

        transform.SetPositionAndRotation(spawnPosition, spawnQuaternion);
    }

    public bool HasBall()
    {
        return ball != null;
    }
}
