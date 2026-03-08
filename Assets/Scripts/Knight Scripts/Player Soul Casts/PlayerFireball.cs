using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFireball : MonoBehaviour
{
    public GameObject fireball;
    private Rigidbody2D rb;

    //units moved per fixedupdate tick
    private float fireballSpeed = 0.5f;

    //these affect how the fireball works during runtime
    private int fireballCooldown = 0;
    private bool shouldLaunch = false;
    private bool wasLaunched = false;
    private float fireballDirection;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //starts hidden
        fireball.SetActive(false);
    }
    void Update()
    {
        //initiates a launch if the cooldown is finished
        if (Keyboard.current.aKey.wasPressedThisFrame && fireballCooldown <= 0)
            shouldLaunch = true;
        
    }
    void FixedUpdate()
    {
        //fireball fires in the direction the player was last facing
        if (fireballCooldown == 0 && !wasLaunched)
            fireballDirection = fireballSpeed * (Swing.lastDirection ? 1 : -1);

        //causes the launch
        if (shouldLaunch && PlayerStats.soul >= 33)
        {
            shouldLaunch = false;
            fireballCooldown = 80;
            wasLaunched = true;
            PlayerStats.soul -= 33;

            CheckFireballCollision.hasHit = false;

            //detatches the fireball from the player so player movement doesent effect it
            fireball.transform.SetParent(null);
            fireball.transform.position = this.transform.position;
            fireball.transform.localScale = new Vector3(Swing.lastDirection ? 2.5f : -2.5f, 2.5f, 1);
            fireball.SetActive(true);
        }

        //moves the fireball while active
        if (fireballCooldown > 0)
        {
            fireball.transform.position += new Vector3(fireballDirection, 0, 0);
            fireballCooldown -= 1;
        }

        //fireball goes back to the player once the cooldown expires
        if (fireballCooldown <= 0 && wasLaunched)
        {
            wasLaunched = false;

            fireball.transform.SetParent(this.transform);

            fireball.transform.localPosition = Vector3.zero;
            fireball.SetActive(false);
        }
    }
}