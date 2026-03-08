using TMPro;
using UnityEngine;

public class ArenaActivate : MonoBehaviour
{
    //all required game objects are public so can be reused for multiple arenas
    public GameObject entryWall;
    public GameObject exitWall;
    public GameObject currentBoss;

    //bool ensures that the game knows when the condition of boss fights beginning is
    public bool bossActivate = false;

    private GameObject player;

    void Start()
    {
        // Walls are inactive until the player enters the arena
        entryWall.SetActive(false);
        exitWall.SetActive(false);

        player = GameObject.Find("KnightObject");
    }

    void FixedUpdate()
    {
        //if the boss is dead or missing then the arena walls are left down.
        if (currentBoss == null)
        {
            entryWall.SetActive(false);
            exitWall.SetActive(false);

            bossActivate = false;
            return;
        }

        //Player is inside the arena bounds and boss is still alive then they are stuck in the arena
        bool playerInsideArena = player.transform.position.x > entryWall.transform.position.x + 2f && player.transform.position.x < exitWall.transform.position.x;
        bool bossAlive = currentBoss.GetComponent<Enemy>().health > 0;

        if (playerInsideArena && bossAlive)

            if (playerInsideArena && currentBoss.GetComponent<Enemy>())
            {
                entryWall.SetActive(true);
                exitWall.SetActive(true);
                bossActivate = true;
            }
            else
            {
                //player left the arena bounds or the boss is dead
                entryWall.SetActive(false);
                exitWall.SetActive(false);
                bossActivate = false;
            }
    }
}
