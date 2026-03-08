using JetBrains.Annotations;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rb;

    //public fields that are set for each individual enemy
    public int geoDrop;
    public float health, maxHealth;
    
    //stops bosses like the false knight being damaged while invulnerable
    public bool invulnerable = false;

    //callbacks referring to enemy damage or death of the enemy
    public System.Action onDeath;
    public System.Action onHit;

    void Start()
    {
        //initialise the objects health
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount)
    {
        //always notify on a hit landed so bosses like false knight can use the player hitting it
        onHit?.Invoke();

        if (!invulnerable)
        {
            health -= amount;
            if (health <= 0)
            {
                //allows object to change states before being destroyed
                onDeath?.Invoke();
                Destroy(gameObject);
                GeoScript.geo += geoDrop;
            }
        }
    }

}