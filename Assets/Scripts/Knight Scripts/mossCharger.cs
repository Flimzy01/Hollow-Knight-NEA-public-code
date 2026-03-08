using UnityEngine;

public class MossCharger : MonoBehaviour
{
    //states used to change sprites and colliders
    private enum State { Hidden, Revealing, Charging, Exposed, Burrowing }
    private State currentState = State.Hidden;

    //changes the enemy movement during runtime
    private float chargeSpeed = 10f;
    private float exposedSpeed = 8f;
    private float detectionRange = 12f;
    private float revealTime = 1f;
    private float burrowTime = 2f;

    //states during runtime
    private float stateTimer = 0f;
    private float chargeDirection = 1f;

    //references to components
    private Rigidbody2D rb;
    private SpriteRenderer mossChargerSprite;
    private BoxCollider2D objectColliderSize;
    private GameObject player;
    private Enemy enemy;

    //sprites for each state the mosscharger can enter
    public Sprite hiddenSprite;
    public Sprite revealingSprite;
    public Sprite chargingSprite;
    public Sprite exposedSprite;

    //collider shapes depending on the state
    public Vector2 hiddenColliderSize = new Vector2(2f, 1f);
    public Vector2 hiddenColliderOffset = new Vector2(0f, 0f);
    public Vector2 exposedColliderSize = new Vector2(1f, 1f);
    public Vector2 exposedColliderOffset = new Vector2(0f, 0f);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //prevents the player from pushing the mosscharger
        rb.freezeRotation = true;
        rb.mass = 1000f;

        mossChargerSprite = GetComponent<SpriteRenderer>();
        objectColliderSize = GetComponent<BoxCollider2D>();
        player = GameObject.Find("KnightObject");
        enemy = GetComponent<Enemy>();

        SetState(State.Hidden);
    }

    void FixedUpdate()
    {
        if (currentState == State.Hidden)
        {
            rb.linearVelocity = Vector2.zero;

            //only activates when the player is close enough
            float distance = Mathf.Abs(player.transform.position.x - transform.position.x);
            if (distance < detectionRange)
                SetState(State.Revealing);
        }

        else if (currentState == State.Revealing)
        {
            rb.linearVelocity = Vector2.zero;
            stateTimer -= Time.fixedDeltaTime;

            if (stateTimer <= 0f)
            {
                //decides the direction to charge depending on the position of the player
                chargeDirection = player.transform.position.x > transform.position.x ? 1f : -1f;
                transform.localScale = new Vector3(chargeDirection > 0 ? 3f : -3f, 3f, 1f);
                SetState(State.Charging);
            }
        }

        else if (currentState == State.Charging)
        {
            //maintains constant velocity
            rb.linearVelocity = new Vector2(chargeDirection * chargeSpeed, rb.linearVelocity.y);
        }

        else if (currentState == State.Exposed)
        {
            //moves away from the player
            float runDirection = transform.position.x > player.transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(runDirection * exposedSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(runDirection > 0 ? 3f : -3f, 3f, 1f);
        }

        else if (currentState == State.Burrowing)
        {
            rb.linearVelocity = Vector2.zero;
            stateTimer -= Time.fixedDeltaTime;

            if (stateTimer <= 0f)
            {
                transform.position = GetNewSpawnPosition();
                SetState(State.Revealing);
            }
        }
    }

    //allows for transitions between states
    void SetState(State newState)
    {
        currentState = newState;

        if (newState == State.Hidden)
        {
            mossChargerSprite.sprite = hiddenSprite;
            objectColliderSize.size = hiddenColliderSize;
            objectColliderSize.offset = hiddenColliderOffset;
        }

        else if (newState == State.Revealing)
        {
            mossChargerSprite.sprite = revealingSprite;
            objectColliderSize.size = hiddenColliderSize;
            objectColliderSize.offset = hiddenColliderOffset;

            //refresh health each time the moss charger appears
            enemy.health = enemy.maxHealth;

            stateTimer = revealTime;
        }

        else if (newState == State.Charging)
        {
            mossChargerSprite.sprite = chargingSprite;
            objectColliderSize.size = hiddenColliderSize;
            objectColliderSize.offset = hiddenColliderOffset;
        }

        else if (newState == State.Exposed)
        {
            mossChargerSprite.sprite = exposedSprite;
            objectColliderSize.size = exposedColliderSize;
            objectColliderSize.offset = exposedColliderOffset;
        }

        else if (newState == State.Burrowing)
        {
            mossChargerSprite.sprite = hiddenSprite;
            objectColliderSize.size = hiddenColliderSize;
            objectColliderSize.offset = hiddenColliderOffset;
            stateTimer = burrowTime;
        }
    }

    //called differently by swing and checkfireballcollision since the mosschargers states interact to damamge differently
    public void TryTakeDamage(int amount)
    {
        //invunerable while underground
        if (currentState == State.Hidden || currentState == State.Revealing)
        return;

        //the first hit will cause the mosscharger to become exposed
        if (currentState == State.Charging)
        {
            SetState(State.Exposed);
            return;
        }

        //when hit during burrowing the moss charger will burrow to prevent more than one hit
        if (currentState == State.Exposed)
        {
            enemy.TakeDamage(amount);
            SetState(State.Burrowing);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //never react to player collision here
        if (other.gameObject.CompareTag("Player"))
        return;

        //running into a wall causes the moss charger to go into burrow
        if (currentState == State.Charging && other.gameObject.CompareTag("Wall"))
            SetState(State.Burrowing);

        if (currentState == State.Exposed && other.gameObject.CompareTag("Wall"))
            SetState(State.Burrowing);
    }

    //calculates burrow position to be on the opposite side of the player
    Vector3 GetNewSpawnPosition()
    {
        float side = player.transform.position.x > transform.position.x ? -1f : 1f;
        return new Vector3(player.transform.position.x + (side * detectionRange * 1.5f), transform.position.y, transform.position.z);
    }
}