using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BenchCheck : MonoBehaviour
{
    public static bool satOnBench = false;
    public static bool inBenchTrigger = false;
    private GameObject bench = null;
    private GameObject benchPrompt = null;
    public Vector2 sitOffset = new(0, 1f);
    public TMP_Text keyboardPrompt;

    void Start()
    {
        benchPrompt = GameObject.Find("BenchPrompt");
        benchPrompt.SetActive(false);
        satOnBench = false;
        inBenchTrigger = false;
        keyboardPrompt.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //checks if the player is in the collision hitbox of the bench, to give them the opportunity to sit on it
        if (collision.CompareTag("Bench"))
        {
            bench = collision.gameObject;
            inBenchTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        //the player cannot sit on it when they leave the hitbox
        if (collision.CompareTag("Bench"))
        {
            inBenchTrigger = false;
            bench = null;
        }
    }

    void FixedUpdate()
    {
        //if the player is sat on the bench and the current bench is not null, then will transform the player onto sitting on the bench
        if (satOnBench && bench != null)
        {
            this.gameObject.transform.position = new(bench.transform.position.x + sitOffset.x, bench.transform.position.y + sitOffset.y, transform.position.z);
        }
    }

    void Update()
    {
        if (inBenchTrigger && !satOnBench)
            benchPrompt.SetActive(true);
        else
            benchPrompt.SetActive(false);

        //if the player presses z and are not sat down already, they will sit down.
        if (Keyboard.current.zKey.wasPressedThisFrame && inBenchTrigger && !satOnBench)
        {
            satOnBench = true;
            //gets player health and sets it to max
            gameObject.GetComponent<PlayerHealth>().SetHealth((int)PlayerHealth.maxHealth);
            PlayerStats.lastBench = this.transform.position;
            return;
        }
        //player gets off the bench
        if (Keyboard.current.zKey.wasPressedThisFrame && satOnBench)
            satOnBench = false;

        //if the player is in the trigger range of the bench, the keyboard input to sit down is shown
        if (inBenchTrigger)
            keyboardPrompt.gameObject.SetActive(true);
        else
            keyboardPrompt.gameObject.SetActive(false);
    }
}