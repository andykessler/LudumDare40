using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using static Constants;

public class GameLoop : MonoBehaviour {

    private static Transform playerPrefab;
    private static Transform opponentPrefab;
    private static Transform hunterPrefab;
    private static Transform ballPrefab;

    // TODO Validate through bitwise XOR on isHumanPlayer, should only be 1
    public static bool[] isHumanPlayers = { true, false, false, false, false, false };
    public static string[] playerNames = { "Human0", "Bot1", "Bot2", "Bot3", "Bot4", "Bot5" }; // could just be made from bool and color
    public static Color[] playerColors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.white };

    // TODO Use numLivesLeft/NumPlayersLeft to create key for # of new balls/hounds to spawn
    public static int numPlayers = 4;
    public static int MAX_NUM_LIVES = 3; // give everyone this many lives

    
    public delegate void ListChangedEvent();
    public static List<BallCarrier> carriers, carriersFree, carriersDead;
    public static List<Ball> balls, ballsFree; // FIXME ballsFree represents two things, split it out
    public static List<BallHunter> hunters, huntersFree; // huntersDead?
    static event ListChangedEvent addFreeCarrierEvent, removeFreeCarrierEvent;
    static event ListChangedEvent addFreeBallEvent, removeFreeBallEvent;
    static event ListChangedEvent addFreeHunterEvent, removeFreeHunterEvent;

    // TODO Could probably parameter a single regsitration function instead of this...lol
    // map from string "name" to List<>, have function add/RemoveAt(i) on value
    // YES DO IT : https://stackoverflow.com/questions/1299920/how-to-handle-add-to-list-event
    // Change to ObservableList<Object>
    public void RegisterToAddFreeCarrierEvent(ListChangedEvent e){addFreeCarrierEvent -= e; addFreeCarrierEvent += e;}
    public void UnregisterToAddFreeCarrierEvent(ListChangedEvent e){addFreeCarrierEvent -= e;}
    public void RegisterToRemoveFreeCarrierEvent(ListChangedEvent e){removeFreeCarrierEvent -= e; removeFreeCarrierEvent += e;}
    public void UnregisterToRemoveFreeCarrierEvent(ListChangedEvent e){removeFreeCarrierEvent -= e;}

    public void RegisterToAddFreeBallEvent(ListChangedEvent e) { addFreeBallEvent -= e; addFreeBallEvent += e; }
    public void UnregisterToAddFreeBallEvent(ListChangedEvent e) { addFreeBallEvent -= e; }
    public void RegisterToRemoveFreeBallEvent(ListChangedEvent e) { removeFreeBallEvent -= e; removeFreeBallEvent += e; }
    public void UnregisterToRemoveFreeBallEvent(ListChangedEvent e) { removeFreeBallEvent -= e; }

    public void RegisterToAddFreeHunterEvent(ListChangedEvent e) { addFreeHunterEvent -= e; addFreeHunterEvent += e; }
    public void UnregisterToAddFreeHunterEvent(ListChangedEvent e) { addFreeHunterEvent -= e; }
    public void RegisterToRemoveFreeHunterEvent(ListChangedEvent e) { removeFreeHunterEvent -= e; removeFreeHunterEvent += e; }
    public void UnregisterToRemoveFreeHunterEvent(ListChangedEvent e) { removeFreeHunterEvent -= e; }

    // TODO Part of extract configs file; Make sure localScale.y is consistent with world.
    private static Vector3 carrierOffsetY;

    private static int numPlayersLeft;
    private static int numLivesLeft;

    private static int maxBallCount;
    private static int maxHunterCount;

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
        numPlayersLeft = numPlayers;
        numLivesLeft = numPlayers * MAX_NUM_LIVES;
        CreateCarriers(); // creates and places carriers evenly along unit circle
        UpdateMaxBallHoundCount(); // checks map if we are should send in different counts
        CreateBallPool(); // get maximum number of balls spawned & ready to display
        CreateHunterPool(); // get maximum number of balls spawned & ready to display
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            GiveFreeBalls();
            SendFreeHunters();
        }
    }
    
    static void GiveFreeBalls()
    {
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
            b.transform.position = bc.transform.position; // undo the hiding!
            
            b.SendMessage("ThrowTo", bc); 
            if (b.Owner != null)
            {
                // we dont want to say that the ball cant be hunted
                //ballsFree.RemoveAt(i);
                //removeFreeBallEvent();
            }
        }
    }

    static void SendFreeHunters()
    {
        for (int i = huntersFree.Count - 1; i >= 0; i--)
        {
            BallHunter bh = huntersFree[i];
            bh.transform.position = Vector3.zero; // undo the hiding!
            bh.SendMessage("FindPrecious"); // FIXME if all precious occupied but there is at least 1 precious, go after it
            bh.SendMessage("ChasePrecious");

            if (bh.Precious != null)
            {
                huntersFree.RemoveAt(i);
                //removeFreeHunterEvent();
            }
        }
    }

    static BallCarrier GetFreeBallCarrier()
    {
        // TODO Can change to random index or based on scores
        BallCarrier bc = carriersFree[0];
        carriersFree.Remove(bc);
        //removeFreeCarrierEvent();
        return bc;
    }

    static void UpdateMaxBallHoundCount()
    {
        maxBallCount = 1;
        maxHunterCount = 1;
    }

    void CreateCarriers()
    {
        // TODO part of the extract configs, ensure scale is consistent
        float amplitude = transform.localScale.z * (5f * 0.8f);
        float radian_ratio = (2 * Mathf.PI) / numPlayers;


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
            bc.currentLives = MAX_NUM_LIVES;
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
            carriersFree.Remove(dead); // not sure if its actually in there right now
            Destroy(dead.gameObject);
            // permantely hidden, if player show game over!!
            return;
        }

        // dead guy shouldnt be doing things! some portal object maybe?
        // fix so we dont pass dead twice  

        // Respawn after 5 seconds if can
        dead.StartCoroutine(WaitAndRespawn(dead, 5f));
        // seperate from giving and chasing
        dead.StartCoroutine(WaitGiveChaseBalls(7.5f));
    }

    private static IEnumerator WaitAndRespawn(BallCarrier dead, float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            print("WaitAndRespawn " + Time.time);
            UpdateMaxBallHoundCount();
            CreateBallPool();
            CreateHunterPool();
            dead.Respawn();
        }
    }

    private static IEnumerator WaitGiveChaseBalls(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            print("WaitAndGiveChaseBalls " + Time.time);
            GiveFreeBalls();
            SendFreeHunters();
        }
    }

}
