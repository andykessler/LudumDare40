using UnityEngine;

public class Constants
{
    //public static Transform groundPrefab;
    public static Transform playerPrefab = ((GameObject) Resources.Load("Prefabs/Player", typeof(GameObject))).transform;
    public static Transform opponentPrefab = ((GameObject) Resources.Load("Prefabs/Opponent", typeof(GameObject))).transform;
    public static Transform hunterPrefab = ((GameObject) Resources.Load("Prefabs/Hunter", typeof(GameObject))).transform;
    public static Transform ballPrefab = ((GameObject) Resources.Load("Prefabs/Ball", typeof(GameObject))).transform;
}