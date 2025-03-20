using Unity.VisualScripting;
using UnityEngine;

public class Taurus_Ide : StateMachineBehaviour
{
    //chuyen sang trang thai di chuyen khi phat hien player
    Transform player;
    TaurusAI taurus;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //locale player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        taurus = animator.GetComponent<TaurusAI>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (taurus.IsPlayerInRange())
        {
            animator.SetTrigger("Walk");
        }
        return;
    }
}
