public class BallProperties : UnitProperties
{
    public static BallProperties props = new BallProperties();

    public const string KEY_SPEED = "speed";
    public const string KEY_MAX_SPEED = "maxSpeed";
    public const string KEY_HEIGHT = "height";
    //public const string KEY_MAX_HEIGHT = "maxHeight";

    public static float speed;
    public static float maxSpeed;
    public static float height;
    //protected float maxHeight;

    protected BallProperties() : base()
    {
        d[KEY_SPEED] = 100f;
        d[KEY_MAX_SPEED] = 200f;
        d[KEY_HEIGHT] = 1f;
        //d[KEY_MAX_HEIGHT] = 5f;
    }
}