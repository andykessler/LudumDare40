public class OpponentProperties : UnitProperties
{
    public static OpponentProperties props = new OpponentProperties();

    public const string KEY_SPEED = "speed";
    public const string KEY_MAX_SPEED = "maxSpeed";
    public const string KEY_MASS = "mass";
    public const string KEY_ROTATION_SPEED = "rotationSpeed";
    public const string KEY_THROW_STRENGTH = "throwStrength";
    public const string KEY_MIN_THROW_TIMER = "minThrowTimer";
    public const string KEY_MAX_THROW_TIMER = "maxThrowTimer";
    public const string KEY_MIN_ROTATE_TIMER = "minRotateTimer";
    public const string KEY_MAX_ROTATE_TIMER = "maxRotateTimer";

    public static float speed, maxSpeed;
    public static float mass;
    public static float rotationSpeed;
    public static float throwStrength;
    public static float minThrowTimer, maxThrowTimer;
    public static float minRotateTimer, maxRotateTimer;

    protected OpponentProperties() : base()
    {
        d[KEY_SPEED] = 16f;
        d[KEY_MAX_SPEED] = 16f;
        d[KEY_MASS] = 2f;
        d[KEY_ROTATION_SPEED] = 0.3f;
        d[KEY_THROW_STRENGTH] = 200f;

        d[KEY_MIN_THROW_TIMER] = 1.5f;
        d[KEY_MAX_THROW_TIMER] = 4.2f;
        d[KEY_MIN_ROTATE_TIMER] = 0.25f;
        d[KEY_MAX_ROTATE_TIMER] = 1.5f;

        Update();
    }

    override protected void Update()
    {
        speed = d[KEY_SPEED];
        maxSpeed = d[KEY_MAX_SPEED];
        mass = d[KEY_MASS];
        rotationSpeed = d[KEY_ROTATION_SPEED];
        throwStrength = d[KEY_THROW_STRENGTH];

        minThrowTimer = d[KEY_MIN_THROW_TIMER];
        maxThrowTimer = d[KEY_MAX_THROW_TIMER];
        minRotateTimer = d[KEY_MIN_ROTATE_TIMER];
        maxRotateTimer = d[KEY_MAX_ROTATE_TIMER];
    }
}