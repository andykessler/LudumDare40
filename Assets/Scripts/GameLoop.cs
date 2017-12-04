using System.Collections.Generic;
using UnityEngine;

//using static Configuration;

public class GameLoop : MonoBehaviour {

    private Transform playerPrefab;
    private Transform opponentPrefab;
    private Transform hunterPrefab;
    private Transform ballPrefab;

    // TODO Validate through bitwise XOR on isHumanPlayer, should only be 1
    public static bool[] isHumanPlayers = { true, false, false, false, false, false };
    public static string[] playerNames = { "Human0", "Bot1", "Bot2", "Bot3", "Bot4", "Bot5" }; // could just be made from bool and color
    public static Color[] playerColors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.white };

    // TODO Use numLivesLeft/NumPlayersLeft to create key for # of new balls/hounds to spawn
    public static int numPlayers = 2;
    public static int MAX_NUM_LIVES = 3; // give everyone this many lives lives

    // TODO Create event listeners for +/- for each list (mainly free lists)
    public static List<BallCarrier> carriers, carriersFree; // carriersDead?
    public static List<Ball> balls, ballsFree; // ballsDead?
    public static List<BallHunter> hunters, huntersFree; // huntersDead?

    // TODO Part of extract configs file; Make sure localScale.y is consistent with world.
    private Vector3 carrierOffsetY;

    private int numPlayersLeft;
    private int numLivesLeft;

    private int maxBallCount;
    private int maxHunterCount;

    // TODO CRITICAL
    // need to know when balls are claimed (e.g player died)
    // this event will cause loop to respond

    // Should be called only once
    void Awake()
    {
        playerPrefab = Constants.playerPrefab;
        opponentPrefab = Constants.opponentPrefab;
        hunterPrefab = Constants.hunterPrefab;
        ballPrefab = Constants.ballPrefab;

        carrierOffsetY = Vector3.up * playerPrefab.transform.localScale.y;
    }

    // Use this for initialization (see if we want to move any up to awake)
    void Start() {
        numPlayersLeft = numPlayers;
        numLivesLeft = numPlayers * MAX_NUM_LIVES;
        CreateCarriers(); // creates and places carriers evenly along unit circle
        UpdateBallHoundCount(); // checks map if we are should send in different counts
        CreateBallPool(); // get maximum number of balls spawned & ready to display
        CreateHunterPool(); // get maximum number of balls spawned & ready to display
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            GiveNewBalls();
            SendNewHunters();
        }
    }
    
    void GiveNewBalls()
    {
        foreach (Ball b in ballsFree)
        {
            BallCarrier bc = GetFreeBallCarrier();
            // FIXME Null pointer exception if no free carrier
            b.transform.position = bc.transform.position; // undo the hiding!
            b.SendMessage("ThrowTo", bc);
            if (b.Owner != null)
            {
                ballsFree.Remove(b); // does this work without screwing up the iterator?
            }
        }
    }

    void SendNewHunters()
    {
        foreach (BallHunter bh in huntersFree) {
            bh.transform.position = Vector3.zero; // undo the hiding!
            bh.SendMessage("FindPrecious");
            bh.SendMessage("ChasePrecious");
            if(bh.Precious != null)
            {
                huntersFree.Remove(bh); // does this work without screwing up the iterator?
            }
        }
    }

    BallCarrier GetFreeBallCarrier()
    {
        // TODO Can change to random index or based on scores
        BallCarrier bc = carriersFree[0];
        carriersFree.Remove(bc);
        return bc;
    }

    void UpdateBallHoundCount()
    {
        maxBallCount = 1;
        maxHunterCount = 1;
    }

    void CreateCarriers()
    {
        // TODO part of the extract configs, ensure scale is consistent
        float amplitude = transform.localScale.z * (5f * 0.8f);
        float radian_ratio = (2 * Mathf.PI) / numPlayers;

        carriers = new List<BallCarrier>();
        carriersFree = new List<BallCarrier>();
        for (int i = 0; i < numPlayers; i++)
        {
            Transform prefab = isHumanPlayers[i] ? playerPrefab : opponentPrefab;
            Transform t = Instantiate(prefab, carrierOffsetY, Quaternion.identity);
            t.name = playerNames[i];
            // TODO Make use of // Color c = playerColors[i];
            float r = radian_ratio * i;
            Vector3 v = new Vector3(Mathf.Cos(r), 0f, Mathf.Sin(r));
            t.position += v * amplitude;
            t.LookAt(carrierOffsetY);
            BallCarrier bc = t.GetComponent<BallCarrier>();
            carriers.Add(bc);
            carriersFree.Add(bc);
        }
    }


    // Create any extra balls we can make now, but do not assign ownership yet
    // Hides the ball from display until needed
    void CreateBallPool()
    {
        balls = new List<Ball>();
        ballsFree = new List<Ball>();
        if (balls.Count < maxBallCount)
        {
            for (int i = balls.Count; i < maxBallCount; i++)
            {
                Vector3 ballPos = new Vector3(0f, ballPrefab.transform.localScale.y / 2, 0f);
                Transform t = Instantiate(ballPrefab, ballPos, Quaternion.identity);
                Ball b = t.GetComponent<Ball>();
                balls.Add(b);
                ballsFree.Add(b); // TODO Hide until needed, use add/remove listener to toggle display
            }
        }
    }

    // Create any extra hunters we can make now, but do not assign ownership yet
    // TODO Hides the hunter from display until needed
    void CreateHunterPool()
    {
        hunters = new List<BallHunter>();
        huntersFree = new List<BallHunter>();
        if (hunters.Count < maxHunterCount)
        {
            for (int i = hunters.Count; i < maxHunterCount; i++)
            {
                Transform t = Instantiate(hunterPrefab, carrierOffsetY, Quaternion.identity);
                BallHunter bh = t.GetComponent<BallHunter>();
                hunters.Add(bh);
                huntersFree.Add(bh); // TODO Hide until needed, use add/remove listener to toggle display
            }
        }
    }
}
