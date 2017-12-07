using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // extract later?

// FIXME Need C# 6+ for this, could reduce verboseness
//using static Constants;

public class GameLoop : MonoBehaviour {

    // do we need own reference?
    private static Transform playerPrefab;
    private static Transform opponentPrefab;
    private static Transform hunterPrefab;
    private static Transform ballPrefab;

    // clean this up...
    public delegate void ListChangedEvent();
    public static List<BallCarrier> carriers, carriersFree, carriersDead;
    public static List<Ball> balls, ballsFree; // FIXME ballsFree represents two things, split it out
    public static List<BallHunter> hunters, huntersFree; // huntersDead?

    // lol nvm prolly dont need any of this, but could to remember in future...
    //// TODO Could probably parameter a single regsitration function instead of this...
    //// map from string "name" to List<>, have function add/RemoveAt(i) on value
    //// YES DO IT : https://stackoverflow.com/questions/1299920/how-to-handle-add-to-list-event
    //static event ListChangedEvent addFreeCarrierEvent, removeFreeCarrierEvent;
    //static event ListChangedEvent addFreeBallEvent, removeFreeBallEvent;
    //static event ListChangedEvent addFreeHunterEvent, removeFreeHunterEvent;
    //// Change to ObservableList<Object>
    //public void RegisterToAddFreeCarrierEvent(ListChangedEvent e){addFreeCarrierEvent -= e; addFreeCarrierEvent += e;}
    //public void UnregisterToAddFreeCarrierEvent(ListChangedEvent e){addFreeCarrierEvent -= e;}
    //public void RegisterToRemoveFreeCarrierEvent(ListChangedEvent e){removeFreeCarrierEvent -= e; removeFreeCarrierEvent += e;}
    //public void UnregisterToRemoveFreeCarrierEvent(ListChangedEvent e){removeFreeCarrierEvent -= e;}
    //public void RegisterToAddFreeBallEvent(ListChangedEvent e) { addFreeBallEvent -= e; addFreeBallEvent += e; }
    //public void UnregisterToAddFreeBallEvent(ListChangedEvent e) { addFreeBallEvent -= e; }
    //public void RegisterToRemoveFreeBallEvent(ListChangedEvent e) { removeFreeBallEvent -= e; removeFreeBallEvent += e; }
    //public void UnregisterToRemoveFreeBallEvent(ListChangedEvent e) { removeFreeBallEvent -= e; }
    //public void RegisterToAddFreeHunterEvent(ListChangedEvent e) { addFreeHunterEvent -= e; addFreeHunterEvent += e; }
    //public void UnregisterToAddFreeHunterEvent(ListChangedEvent e) { addFreeHunterEvent -= e; }
    //public void RegisterToRemoveFreeHunterEvent(ListChangedEvent e) { removeFreeHunterEvent -= e; removeFreeHunterEvent += e; }
    //public void UnregisterToRemoveFreeHunterEvent(ListChangedEvent e) { removeFreeHunterEvent -= e; }

    // TODO Part of extract configs file; Make sure localScale.y is consistent with world.
    private static Vector3 carrierOffsetY;

    private static int numPlayersLeft;
    private static int numLivesLeft;

    private static int maxBallCount;
    private static int maxHunterCount;

    public bool gameStarted = false;
    public bool roundStarted = false;

    public Text text;

    // Should be called only once
    void Awake()
    {
        playerPrefab = Constants.playerPrefab;
        opponentPrefab = Constants.opponentPrefab;
        hunterPrefab = Constants.hunterPrefab;
        ballPrefab = Constants.ballPrefab;

        // TODO we instantiate lists in same way in Restart() can we not duplicate
        // solution: simple null checks on each list in restart?
        carriers = new List<BallCarrier>();
        carriersFree = new List<BallCarrier>();
        carriersDead = new List<BallCarrier>();
        balls = new List<Ball>();
        ballsFree = new List<Ball>();
        hunters = new List<BallHunter>();
        huntersFree = new List<BallHunter>();

        carrierOffsetY = Vector3.up * playerPrefab.transform.localScale.y;
    }

    // Use this for initialization (see if we want to move any up to awake)
    void Start() {
        gameStarted = false;
        roundStarted = false;
    }

    void Restart()
    {
        gameStarted = true;

        // Destroy all current game objects
        // FIXME What about pooling / saving for later instead?
        // Consider try catch exception continue
        foreach(BallCarrier bc in carriers) { Destroy(bc.gameObject); }
        foreach(BallCarrier bc in carriersFree) { Destroy(bc.gameObject); }
        foreach(BallCarrier bc in carriersDead) { Destroy(bc.gameObject); }
        foreach(Ball b in balls) { Destroy(b.gameObject); }
        foreach(Ball b in ballsFree) { Destroy(b.gameObject); }
        foreach(BallHunter bh in hunters) { Destroy(bh.gameObject); }
        foreach(BallHunter bh in huntersFree) { Destroy(bh.gameObject); }

        // Create new lists to put new game objects in
        carriers = new List<BallCarrier>();
        carriersFree = new List<BallCarrier>();
        carriersDead = new List<BallCarrier>();
        balls = new List<Ball>();
        ballsFree = new List<Ball>();
        hunters = new List<BallHunter>();
        huntersFree = new List<BallHunter>();

        numPlayersLeft = (int)GameProperties.carriers;
        numLivesLeft = (int)GameProperties.carriers * (int)GameProperties.lives;

        CreateCarriers(); // creates and places carriers evenly along unit circle
        UpdateMaxBallHunterCount(); // checks map if we are should send in different counts
        CreateBallPool(); // get maximum number of balls spawned & ready to display
        CreateHunterPool(); // get maximum number of balls spawned & ready to display

        roundStarted = false;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (roundStarted || !gameStarted)
            {
                Restart();
                return;
            }
            if(!roundStarted)
            {
                GiveFreeBalls();
                SendFreeHunters();
                roundStarted = true;
            }
        }
    }
    
    static void GiveFreeBalls()
    {
        // TODO Send out no more than MAX count (will allow decrease over time)
        for(int i = ballsFree.Count - 1; i >= 0; i--)
        {
            if(ballsFree[i].Owner != null)
            {
                // do anything here?
                return;
            }

            Ball b = ballsFree[i];
            BallCarrier bc = GetFreeBallCarrier();
            // FIXME Null pointer exception if no free carrier
            if(bc != null)
            {
                //b.transform.position = bc.transform.position; // undo the hiding!
                b.SendMessage("ThrowTo", bc);
                if (b.Owner != null)
                {
                    // we dont want to say that the ball cant be hunted
                    //ballsFree.RemoveAt(i);
                    //removeFreeBallEvent();
                }
            }
          
        }
    }


    static void SendFreeHunters()
    {
        // TODO Send out no more than MAX count (will allow decrease over time)
        for (int i = huntersFree.Count - 1; i >= 0; i--)
        {
            BallHunter bh = huntersFree[i];
            //bh.transform.position = Vector3.zero; // undo the hiding!
            bh.SendMessage("FindPrecious"); // FIXME if all precious occupied but there is at least 1 precious, go after it
            bh.SendMessage("ChasePrecious");

            if (bh.Precious != null)
            {
                huntersFree.RemoveAt(i);
                //removeFreeHunterEvent();
            }
        }
    }

    public static BallCarrier GetFreeBallCarrier()
    {
        // TODO Can change to random index or based on scores
        if(carriersFree.Count > 0)
        {
            BallCarrier bc = carriersFree[0];
            carriersFree.Remove(bc);
            //removeFreeCarrierEvent();
            return bc;
        }
        else
        {
            Debug.Log("No free carriers!");
            return null;
        }
    }

    public static Ball GetFreeBall()
    {
        // TODO Can change to random index or based on scores
        if (ballsFree.Count > 0)
        {
            Ball b = ballsFree[0];
            ballsFree.Remove(b);
            return b;
        }
        else
        {
            Debug.Log("No free carriers!");
            return null;
        }
    }

    // need to also have a check for total lives left and what to do for count
    static void UpdateMaxBallHunterCount()
    {

        //maxBallCount = (int)(numPlayersLeft / Constants.ballPlayerRatio);
        //if (maxBallCount == 0) maxBallCount = 1;
        //maxHunterCount = maxBallCount;

        maxBallCount = (int)Mathf.Max(GameProperties.balls,1f);
        maxHunterCount = (int)Mathf.Max(GameProperties.hunters, 1f);
    }

    void CreateCarriers()
    {
        // TODO part of the extract configs, ensure scale is consistent
        float amplitude = transform.localScale.z * (5f * 0.8f);
        float radian_ratio = (2 * Mathf.PI) / GameProperties.carriers;

        for (int i = 0; i < GameProperties.carriers; i++)
        {
            Transform prefab = Constants.isHumanPlayers[i] ? playerPrefab : opponentPrefab;
            Transform t = Instantiate(prefab, carrierOffsetY, Quaternion.identity);
            t.name = Constants.playerNames[i];
            // TODO Make use of // Color c = playerColors[i];
            float r = radian_ratio * i;
            Vector3 v = new Vector3(Mathf.Cos(r), 0f, Mathf.Sin(r));
            t.position += v * amplitude;
            t.LookAt(carrierOffsetY);
            BallCarrier bc = t.GetComponent<BallCarrier>();
            bc.currentLives = (int)Mathf.Max(GameProperties.lives, 1f);
            // TODO Lives change event listener? Calls code to handle respawn broadcast etc,

            carriers.Add(bc);
            carriersFree.Add(bc);
            //addFreeCarrierEvent();
        }
    }

    // Create any extra balls we can make now, but do not assign ownership yet
    // Hides the ball from display until needed
    static void CreateBallPool()
    {
        if (balls.Count < maxBallCount)
        {
            for (int i = balls.Count; i < maxBallCount; i++)
            {
                Vector3 ballPos = new Vector3(0f, ballPrefab.transform.localScale.y / 2, 0f);
                Transform t = Instantiate(ballPrefab, ballPos, Quaternion.identity);
                Ball b = t.GetComponent<Ball>();
                balls.Add(b);
                ballsFree.Add(b); // TODO Hide until needed, use add/remove listener to toggle display
                //addFreeBallEvent();
            }
        }
    }

    // Create any extra hunters we can make now, but do not assign ownership yet
    // TODO Hides the hunter from display until needed
    static void CreateHunterPool()
    {
        if (hunters.Count < maxHunterCount)
        {
            for (int i = hunters.Count; i < maxHunterCount; i++)
            {
                Transform t = Instantiate(hunterPrefab, carrierOffsetY, Quaternion.identity);
                BallHunter bh = t.GetComponent<BallHunter>();
                hunters.Add(bh);
                huntersFree.Add(bh); // TODO Hide until needed, use add/remove listener to toggle display
                //addFreeHunterEvent();
            }
        }
    }
 
    public static void KillAndRespawnIfHaveLife(BallCarrier dead)
    {
        dead.currentLives -= 1;
        numLivesLeft -= 1;

        if (dead.currentLives == 0) {
            // cant respawn
            Debug.Log("Player Defeated!");
            numPlayersLeft -= 1;

            // KILL IT WITH FIRE!!!
            carriersFree.Remove(dead); // not sure if its actually in there right now
            if (Constants.isHumanPlayers[carriers.IndexOf(dead)])
            {
                Debug.Log("Player is elimiated!");
            }
            carriers.Remove(dead);
            carriersFree.Remove(dead);
            carriersDead.Add(dead);
            dead.GetComponent<MeshRenderer>().enabled = false; //  hidden
            //Destroy(dead.gameObject); // conseqs of this?

            if (carriers.Count == 1) // game over! last man wins!
            {
                Debug.Log(carriers[0] + " wins!");
                return;
            }
        }
        else
        {
            dead.StartCoroutine(WaitAndRespawn(dead, 2f)); // TODO Extract wait to config constant
        }
        // dead guy shouldnt be doing things! some portal object maybe?
        // fix so we dont pass dead twice  
        dead.StartCoroutine(WaitGiveChaseBalls(4f)); // TODO Extract wait to config constant
    }

    private static IEnumerator WaitAndRespawn(BallCarrier dead, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        print("WaitAndRespawn " + Time.time);
        UpdateMaxBallHunterCount();
        CreateBallPool();
        CreateHunterPool();
        dead.Respawn();
    }

    // seperate give and chase into seperate routines
    private static IEnumerator WaitGiveChaseBalls(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        print("WaitAndGiveChaseBalls " + Time.time);
        GiveFreeBalls();
        SendFreeHunters();
    }
}
