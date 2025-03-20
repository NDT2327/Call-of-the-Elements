using UnityEngine;

public class Taurus_Walk : StateMachineBehaviour
{

    public float speed = 2.5f;
    public float attackRange = 2f;

    Transform player;
    Rigidbody2D rb;
    TaurusAI taurus;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        taurus = animator.GetComponent<TaurusAI>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        taurus.LookAtPlayer();

        //move toward to player
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        float distance = Vector2.Distance(player.position, rb.position);

        //tinh khoang cach player va boss   
        if (distance <= attackRange)
        {
            animator.SetTrigger("Attack");
            if (Random.value > 0.5f) animator.SetTrigger("AttackType");

        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
}
