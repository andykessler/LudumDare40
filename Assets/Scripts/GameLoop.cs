using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour {

    // FIXME Make private visibility
    public static Transform groundPrefab;
    public static  Transform playerPrefab;
    public static Transform opponentPrefab;
    public static Transform hunterPrefab;
    public static Transform ballPrefab;

    // TODO use numLivesLeft/NumPlayersLeft to create key for # of new balls/hounds to spawn
    public static int numPlayers = 2;
    public static int MAX_NUM_LIVES = 3; // give everyone this many lives lives
    
    private int numPlayersLeft;
    private int numLivesLeft;

    // TODO Part of extract configs file; Make sure localScale.y is consistent with world.
    public static Vector3 carrierOffsetY = Vector3.up * playerPrefab.transform.localScale.y;


    // TODO CRITICAL
    // need to know when balls are claimed (e.g player died)
    // this event will cause loop to respond!
    
    List<BallCarrier> carriers, carriersFree; // carriersDead?
    List<Ball> balls, ballsFree; // ballsDead?
    List<BallHunter> hunters, huntersFree; // huntersDead?
    
    Transform player, ball, hunter;
    Transform[] opponents;

    //bool isRunning;

	// Use this for initialization
	void Start () {
        numPlayersLeft = numPlayers;
        numLivesLeft = numPlayers * MAX_NUM_LIVES;

        opponents = new Transform[numPlayers - 1];
        float radian_ratio = (2 * Mathf.PI) / numPlayers;

        // TODO part of the extract configs, ensure scale is consistent
        float amplitude = transform.localScale.z * (5f * 0.8f);
        
        player = Instantiate(playerPrefab, (Vector3.right * amplitude) + carrierOffsetY, Quaternion.identity);
        player.name = "Player";
        player.transform.LookAt(carrierOffsetY);
        for (int i=0; i < numPlayers - 1; i++)
        {
            Transform t = Instantiate(opponentPrefab, Vector3.zero, Quaternion.identity);
            t.name = "Opponent" + i;
            float radians = radian_ratio * (i + 1);
            Vector3 v = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians));
            t.position = (v * amplitude) + carrierOffsetY;
            t.LookAt(carrierOffsetY);
            opponents[i] = t;
        }

        hunter = Instantiate(hunterPrefab, carrierOffsetY, Quaternion.identity);
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
