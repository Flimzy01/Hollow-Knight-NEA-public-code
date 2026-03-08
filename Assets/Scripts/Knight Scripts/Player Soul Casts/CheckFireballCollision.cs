using UnityEngine;

public class CheckFireballCollision : MonoBehaviour
{
    //static so the player fireball can be reset
    public static bool hasHit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //stops the attack registering multiple times
        if (hasHit) 
        return;

        //calculates differently for a moss charger
        if (collision.TryGetComponent<MossCharger>(out MossCharger mossCharger))
        {
            hasHit = true;
            mossCharger.TryTakeDamage(PlayerStats.fireBallDamage);
            return;
        }

        //normal fireball deals a set amouunt of damage
        if (collision.TryGetComponent<Enemy>(out Enemy enemyComponent))
        {
            hasHit = true;
            enemyComponent.TakeDamage(PlayerStats.fireBallDamage);
        }
    }
}