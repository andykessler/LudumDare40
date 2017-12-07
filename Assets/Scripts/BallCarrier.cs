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
        if (ball.Owner != this)
        {
            Debug.Log("Can't throw the ball. Have not caught it yet!");
            return;
        }
        ball.SendMessage("ThrowTo", target); // pass strength parameter here? and change owner here?
        ball = null;
    }

    void ReceiveBall(Ball b)
    {
        if (HasBall()) // redundant because only this is called by trigger, and you already have been assigned
        {
            //Debug.Log("Can't receive the ball. Already have a ball!");
            return;
        }
        // is something else blocking you from getting a ball? other rules? distance?
        ball = b;
        GameLoop.carriersFree.Remove(this);
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

        GameLoop.KillAndRespawnIfHaveLife(this);
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
