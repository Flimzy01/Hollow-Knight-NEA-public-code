using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class hideswing : MonoBehaviour
{
    //the hitbox of the nail
    public GameObject Attack;
    void Awake()
    {
        //ensures that the swing is always hidden at the start
        Attack = GameObject.Find("Attack");
        Attack.SetActive(false);
        
    }

    void Update()
    {
        //makes the attack visible while x is pressed
        if (Keyboard.current.xKey.isPressed)
        {
            Attack.SetActive(true);
        }
    }
}
