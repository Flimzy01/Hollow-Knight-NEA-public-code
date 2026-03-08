
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class GruzMotherAttack : MonoBehaviour
{
    //refers to components
    private GameObject gruzMother;
    private Rigidbody2D rb;

    //rather than an enum, just referred to two sprites due to a lack of state changes
    public Sprite gruzMotherAwake;
    public Sprite gruzMotherAttack;

    //prevents the boss being activated multiple times
    private bool activateOnlyOnce = true;

    //variables to change the behaviour of the boss during the fight
    private bool requiresVelocityChange = false;
    private float attackTime = 0f;
    private float cooldownTime = 5f;
    private float dashSpeed = 18f;
    private Vector3 dashAttack = new Vector3(0, 0, 0);

    public static bool gruzMotherBossDefeated = false;
    public ArenaActivate arenaActivate;


    void Start()
    {
        gruzMother = this.gameObject;
        rb = GetComponent<Rigidbody2D>();

        //sets the rigidbody to high mass and no rotation due to being a dynamic body
        rb.freezeRotation = true;
        rb.mass = 1000f;

        //destroys the gruzMother if the boss has already been defeated
        if (gruzMotherBossDefeated)
        {
            Destroy(gameObject);
            return;
        }

        //checks if the boss is dead to store in a save file
        GetComponent<Enemy>().onDeath += () => gruzMotherBossDefeated = true;
    }

    void Update()
    {
        //all conditions from activating the arena must be cleared for the boss to begin
        if (!activateOnlyOnce && !arenaActivate.bossActivate)
        {
            activateOnlyOnce = true;
            attackTime = 0f;
            cooldownTime = 5f;
            GetComponent<Enemy>().health = GetComponent<Enemy>().maxHealth;
        }

        //changes variables so the boss fight begins and the boss starts attacking
        if (arenaActivate.bossActivate && activateOnlyOnce)
        {
            activateOnlyOnce = false;
            cooldownTime = 5f;
            attackTime = 20f;
            GenerateRandomVelocity();
        }

        //the boss will not do anything if they have already been defeated or the boss fight start conditions arent met
        if (gruzMotherBossDefeated || !arenaActivate.bossActivate)
            return;

        //if the boss has finished its attack it will stop moving until the cooldown is finished
        if (attackTime <= 0f)
        {
            rb.linearVelocity = Vector2.zero;
            cooldownTime -= Time.deltaTime;
            gameObject.GetComponent<SpriteRenderer>().sprite = gruzMotherAwake;
        }

        //if the cooldown is finished the attack will restart
        if (cooldownTime <= 0f)
        {
            attackTime = 20f;
            cooldownTime = 5f;
            GenerateRandomVelocity();
        }

        //while the attack is still active the boss will move and the sprite will change accordingly
        if (attackTime > 0f)
        {
            attackTime -= Time.deltaTime;
            rb.linearVelocity = dashAttack;
            gameObject.GetComponent<SpriteRenderer>().sprite = gruzMotherAttack;

            //will change the velocity if any collisions with the walls or ceilings to change the direction
            if (requiresVelocityChange)
            {
                rb.linearVelocity = dashAttack;
                requiresVelocityChange = false;
            }
        }

        //flips the boss to be moving in the direction they are going in
        if (dashAttack.x >= 0)
            transform.localScale = new(-3, 3);
        else
            transform.localScale = new(3, 3);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //if the boss collides with the walls roof or floor, they will need to change their velocity
        bool hitWall = other.gameObject.CompareTag("Wall");
        bool hitFloorOrRoof = other.gameObject.CompareTag("Roof") || other.gameObject.CompareTag("Ground");

        if (hitWall)
            dashAttack.x *= -1;
        if (hitFloorOrRoof)
            dashAttack.y *= -1;

        if (hitWall || hitFloorOrRoof)
            rb.linearVelocity = dashAttack;
    }


    void GenerateRandomVelocity()
    {
        //generates a random direction in both the x and y
        Vector2 randomVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        //normalises the vector and multiplies by the speed of the boss to ensure the same speed no matter the direction
        randomVector.Normalize();
        dashAttack = new Vector3(randomVector.x, randomVector.y, 0) * dashSpeed;
        requiresVelocityChange = true;

    }
}
