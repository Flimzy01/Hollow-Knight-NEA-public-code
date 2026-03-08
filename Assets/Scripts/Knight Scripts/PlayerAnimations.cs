using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimations : MonoBehaviour
{
    private Rigidbody2D rb;

    Animator animator;
    string currentState;

    //animation state names are stored as constants
    const string PLAYER_IDLE = "Player_Idle";
    const string PLAYER_SPRINT = "Player_Sprint";
    const string PLAYER_JUMP = "Player_Jump";
    const string PLAYER_SWING = "Player_Swing";
    const string PLAYER_FALL = "Player_Fall";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        ChangeCurrentAnimation(PLAYER_IDLE);
    }

    void Update()
    {
        //evaluates which animation to choose in priority of what would be the most important animation
        if (Keyboard.current.xKey.isPressed && Swing.attackDuration <= 0)
        {
            ChangeCurrentAnimation(PLAYER_SWING);
            return;
        }

        else if (rb.linearVelocityY > 0f)
        {
            ChangeCurrentAnimation(PLAYER_JUMP);
            return;
        }

        else if (rb.linearVelocityX > 0.1f || rb.linearVelocityX < -0.1f)
        {
            ChangeCurrentAnimation(PLAYER_SPRINT);
            return;
        }

        else if (rb.linearVelocityY < -0.1f && !PlayerMovement.grounded)
        {
            ChangeCurrentAnimation(PLAYER_FALL);
            return;
        }

        else if (rb.linearVelocityX == 0f && PlayerMovement.grounded)
        {
            ChangeCurrentAnimation(PLAYER_IDLE);
            return;
        }
    }

    //allows for transition from one state to another given that the next state isnt the same as the last
    public void ChangeCurrentAnimation(string nextState)
    {
        if (nextState == currentState || playingAnimation(animator, currentState))
        {
            return;
        }

        animator.Play(nextState);
        currentState = nextState;
    }

    //checks if the current state has finished playing or not
    private bool playingAnimation(Animator animator, string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            return true;
        else
            return false;
        
    }

}
