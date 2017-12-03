using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour {
    

    public Transform playerPrefab;
    public Transform opponentPrefab;
    public Transform hunterPrefab;
    public Transform ballPrefab;

    public int numPlayers = 2;

    Transform player;

    Transform[] opponents;

    Transform ball;

    Transform hunter;

    bool isRunning;

	// Use this for initialization
	void Start () {
        opponents = new Transform[numPlayers - 1];
        float radian_ratio = (2 * Mathf.PI) / numPlayers;
        float amplitude = 8f;
        Vector3 carrierOffset = Vector3.up * playerPrefab.transform.localScale.y; // Make sure this is same between player/opponents
        player = Instantiate(playerPrefab, (Vector3.right * amplitude) + carrierOffset, Quaternion.identity);
        player.name = "Player";
        player.transform.LookAt(carrierOffset);
        for (int i=0; i < numPlayers - 1; i++)
        {
            Transform t = Instantiate(opponentPrefab, Vector3.zero, Quaternion.identity);
            t.name = "Opponent" + i;
            float radians = radian_ratio * (i + 1);
            Vector3 v = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians));
            t.position = (v * amplitude) + carrierOffset;
            t.LookAt(carrierOffset);
            opponents[i] = t;
        }

        hunter = Instantiate(hunterPrefab, Vector3.up * hunterPrefab.transform.position.y, Quaternion.identity);
        Vector3 ballPos = new Vector3(amplitude / 2f, ballPrefab.transform.localScale.y / 2, 0f);
        ball = Instantiate(ballPrefab, ballPos, Quaternion.identity);

        // TODO temp remove me 
        Ball b = ball.GetComponent<Ball>();
        player.GetComponent<PlayerController>().gameBall = b;
    }
	
	// Update is called once per frame
	void Update () {
		// TODO a lot of game logic, (e.g. gameover, life tracking)
        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            hunter.SendMessage("ToggleChasePrecious");
        }
        if(Input.GetKeyUp(KeyCode.Alpha2))
        {
            hunter.SendMessage("FindPrecious");
        }

	}
}
