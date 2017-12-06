public class PlayerProperties : UnitProperties
{
    public static PlayerProperties props = new PlayerProperties();

    public const string KEY_SPEED = "speed";
    public const string KEY_MAX_SPEED = "maxSpeed";
    public const string KEY_MASS = "mass";
    public const string KEY_ROTATION_SPEED = "rotationSpeed";
    public const string KEY_THROW_STRENGTH = "throwStrength";

    public static float speed;
    public static float maxSpeed;
    public static float mass;
    public static float rotationSpeed;
    public static float throwStrength;

    protected PlayerProperties() : base()
    {
        d[KEY_SPEED] = 16f;
        d[KEY_MAX_SPEED] = 50f;
        d[KEY_MASS] = 2f;
        d[KEY_ROTATION_SPEED] = 0.3f;
        d[KEY_THROW_STRENGTH] = 200f;

        Update();
    }

    override protected void Update()
    {
        speed = d[KEY_SPEED];
        maxSpeed = d[KEY_MAX_SPEED];
        mass = d[KEY_MASS];
        rotationSpeed = d[KEY_ROTATION_SPEED];
        throwStrength = d[KEY_THROW_STRENGTH];
    }
}