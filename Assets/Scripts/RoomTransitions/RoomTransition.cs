using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransition : MonoBehaviour
{
    //the scene to load if a scene change is required
    public string targetScene;

    //the intended point to teleport to
    public string targetTransitionID;

    //an identifier for this specific teleport point
    public string thisTransitionID;
    //true for same scene teleports
    public bool sameScene = false;

    //saved accross scenes since it is static
    private static string lastTransitionID   = "";
    
    // Prevents re-trigger immediately on arrival
    private static bool waitingForMovement = false;
    
    // Seconds before the trigger is re-enabled
    private static float cooldown = 0f;

    void Awake()
    {
        //find the point to load at and place them there
        if (!string.IsNullOrEmpty(lastTransitionID))
        {
            RoomTransition[] allPoints = FindObjectsByType<RoomTransition>(FindObjectsSortMode.None);
            foreach (RoomTransition tp in allPoints)
            {
                if (tp.thisTransitionID == lastTransitionID)
                {
                    GameObject player = GameObject.Find("KnightObject");
                    if (player != null)
                    {
                        player.transform.position = tp.transform.position;

                        //gives the player a small push in the case that the room has a teleporter facing vertically
                        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                        if (playerRb != null)
                            playerRb.linearVelocity = new Vector2(0, 20f);
                    }
                    break;
                }
            }

            lastTransitionID = "";
            waitingForMovement = true;
        }

        //remembers the last scene for when the player is in the main menu to load into
        string current = SceneManager.GetActiveScene().name;
        if (current != "MainMenu")
            PlayerPrefs.SetString("LastScene", current);
    }

    void Update()
    {
        //once the player starts moving after arrival the transition trigger is re-enabled
        if (waitingForMovement)
        {
            Rigidbody2D playerRb = GameObject.Find("KnightObject")?.GetComponent<Rigidbody2D>();
            if (playerRb != null && playerRb.linearVelocity.magnitude > 0.1f)
            {
                waitingForMovement = false;

                //short time so that the player will not teleport back until they move
                cooldown = 0.5f;
            }
        }

        if (cooldown > 0f)
            cooldown -= Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (waitingForMovement || cooldown > 0f)
            return;

        if (sameScene)
        {
            //find the target point in the same scene and teleport
            RoomTransition[] allPoints = FindObjectsByType<RoomTransition>(FindObjectsSortMode.None);
            foreach (RoomTransition tp in allPoints)
            {
                if (tp.thisTransitionID == targetTransitionID)
                {
                    other.transform.position = tp.transform.position;
                    waitingForMovement       = true;
                    break;
                }
            }
        }

        else
        {
            //for moving across scenes and will save the target id and move to the scene it is directed to
            lastTransitionID = targetTransitionID;
            SceneManager.LoadScene(targetScene);
        }
    }
}