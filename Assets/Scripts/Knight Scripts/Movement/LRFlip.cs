using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class lrflip : MonoBehaviour
{
    //uses the last direction of the player and flips the scale of the player depending
    void Update()
    {
        if (Swing.attackDuration < -1)
        {
                if (!Swing.lastDirection)
            {

                Vector3 scale = transform.localScale;
                scale.x = 2.5f;

                transform.localScale = scale;

                return;
            }

            else if(Swing.lastDirection){

                Vector3 scale = transform.localScale;
                scale.x = -2.5f;

                transform.localScale = scale;


            }
        }
        
    }
}
