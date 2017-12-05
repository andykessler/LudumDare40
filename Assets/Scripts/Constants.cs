using UnityEngine;

public class Constants
{
    //public static Transform groundPrefab;
    public static Transform playerPrefab = ((GameObject) Resources.Load("Prefabs/Player", typeof(GameObject))).transform;
    public static Transform opponentPrefab = ((GameObject) Resources.Load("Prefabs/Opponent", typeof(GameObject))).transform;
    public static Transform hunterPrefab = ((GameObject) Resources.Load("Prefabs/Hunter", typeof(GameObject))).transform;
    public static Transform ballPrefab = ((GameObject) Resources.Load("Prefabs/Ball", typeof(GameObject))).transform;

    // TODO Validate through bitwise XOR on isHumanPlayer, should only be 1
    public static bool[] isHumanPlayers = { true, false, false, false, false, false, false, false };
    public static string[] playerNames = { "Human0", "Bot1", "Bot2", "Bot3", "Bot4", "Bot5", "Bot6", "Bot7" }; // could just be made from bool and color
    public static Color[] playerColors = { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.white, Color.black, Color.grey };

    // TODO Use numLivesLeft/NumPlayersLeft to create key for # of new balls/hounds to spawn
    public static int numPlayers = 8;
    public static int MAX_NUM_LIVES = 3; // give everyone this many lives
    
    public static float ballPlayerRatio = 2f;
    /*public static int maxHunterCount = maxBallCount;*/
}