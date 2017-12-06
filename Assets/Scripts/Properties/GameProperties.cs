public class GameProperties : UnitProperties
{
    public static GameProperties props = new GameProperties();

    public const string KEY_BALLS = "balls";
    public const string KEY_HUNTERS = "hunters";
    public const string KEY_CARRIERS = "carriers";
    public const string KEY_LIVES = "lives";

    public static float balls;
    public static float hunters;
    public static float carriers;
    public static float lives;

    protected GameProperties() : base()
    {
        d[KEY_BALLS] = 1f;
        d[KEY_HUNTERS] = 1f;
        d[KEY_CARRIERS] = 2f;
        d[KEY_LIVES] = 3f;
    }
}