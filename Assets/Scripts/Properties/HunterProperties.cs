public class HunterProperties : UnitProperties
{
    public static HunterProperties props = new HunterProperties();

    public const string KEY_SPEED = "speed";
    public const string KEY_MAX_SPEED = "maxSpeed";
    public const string KEY_ACCELERATION = "acceleration";
    public const string KEY_MAX_ACCELERATION = "maxAcceleration";
    public const string KEY_DAMPENING = "dampening";
    //public const string KEY_MAX_DAMPENING = "maxDampening";
    public const string KEY_MASS = "mass";
    public const string KEY_COLLISION_RADIUS = "collisionDistance";
    public const string KEY_ROTATION_SPEED = "rotationSpeed";

    public static float speed;
    public static float maxSpeed;
    public static float acceleration;
    public static float maxAcceleration;
    public static float dampening;
    //public static float maxDampening;
    public static float mass;
    public static float collisionRadius;
    public static float rotationSpeed;

    protected HunterProperties() : base()
    {
        d[KEY_SPEED] = 1f;
        d[KEY_MAX_SPEED] = 42f;
        d[KEY_ACCELERATION] = 1f;
        d[KEY_MAX_ACCELERATION] = 10f;
        d[KEY_DAMPENING] = 0.2f;
        //d[KEY_MAX_DAMPENING] = 1f;
        d[KEY_MASS] = 0.05f;
        d[KEY_COLLISION_RADIUS] = 6f;
        d[KEY_ROTATION_SPEED] = 0f;

        Update();
    }

    override protected void Update()
    {
        speed = d[KEY_SPEED];
        maxSpeed = d[KEY_MAX_SPEED];
        acceleration = d[KEY_ACCELERATION];
        maxAcceleration = d[KEY_MAX_ACCELERATION];
        dampening = d[KEY_DAMPENING];
        //maxDampening = d[KEY_MAX_DAMPENING];
        mass = d[KEY_MASS];
        rotationSpeed = d[KEY_ROTATION_SPEED];
        collisionRadius = d[KEY_COLLISION_RADIUS];
    }
}