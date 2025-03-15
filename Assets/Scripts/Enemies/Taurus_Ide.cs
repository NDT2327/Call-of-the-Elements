using Unity.VisualScripting;
using UnityEngine;

public class Taurus_Ide : StateMachineBehaviour
{
    //chuyen sang trang thai di chuyen khi phat hien player
    Transform player;
    //nhung player trong pham vi tan cong thi se chuyen trang thai tan cong
    public float attackRange = 2f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //locale player
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //calculate the distance between player and taurus
        float distanceToPlayer = Vector2.Distance(animator.transform.position, player.transform.position);
        //if player in attack range => attack
        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("IsAttacking", true);
            //random attack type
            if (Random.value > 0.5f) animator.SetTrigger("AttackType");//random attack type
        }
        //if not => move state
        else if (distanceToPlayer > attackRange) {
            animator.SetBool("IsWalking", true);
        }
    }
}
