using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Taurus_Atk2 : StateMachineBehaviour
{
    private Rigidbody2D rb;
    private float groundY;
    private float heightOffset = 1f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        groundY = rb.position.y;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float normalizedTime = stateInfo.normalizedTime % 1f;
        float offset;

        if (normalizedTime < 0.5f)
        {
            offset = Mathf.Lerp(0f, heightOffset, normalizedTime *2f);
        }
        else
        {
            offset = Mathf.Lerp(heightOffset, 0f, (normalizedTime - 0.5f) * 2f);
        }
        rb.MovePosition(new Vector2(rb.position.x, groundY + offset));
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsAttacking", false);
        rb.MovePosition(new Vector2(rb.position.x, groundY));
    }
}
