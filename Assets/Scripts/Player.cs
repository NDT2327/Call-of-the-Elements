using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public Animator animator;

    public float speed = 5f;
    public float jumpHeight = 5f;
    public bool isGrounded = false;

    private float movement;
    private bool facingRight = true;
    private bool isJumping = false;
    private int attackCount = 0;
    private float lastAttackTime;
    public float attackResetTime = 0.5f; // Time to reset attack count

    void Update()
    {
        movement = Input.GetAxis("Horizontal");

        // Flip character
        if ((movement < 0f && facingRight) || (movement > 0f && !facingRight))
        {
            facingRight = !facingRight;
            transform.eulerAngles = new Vector3(0f, facingRight ? 0f : -180f, 0f);
        }

        // Jump logic
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
            isGrounded = false;
            isJumping = true;
            animator.SetBool("Jump", true);
        }

        // Check if player wants to do a jump attack
        if (isJumping && Input.GetKey(KeyCode.Z))
        {
            animator.SetBool("JumpHit", true);
        }
        else
        {
            animator.SetBool("JumpHit", false);
        }

        // Attack logic
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (Time.time - lastAttackTime > attackResetTime)
            {
                attackCount = 0; // Reset combo if too much time has passed
            }

            attackCount++;
            lastAttackTime = Time.time;

            if (attackCount == 1)
            {
                animator.SetTrigger("Attack1");
            }
            else if (attackCount == 2)
            {
                animator.SetTrigger("Attack2");
            }
            else if (attackCount >= 3)
            {
                animator.SetTrigger("Attack3");
                attackCount = 0; // Reset combo after third attack
            }
        }

        // Roll logic
        if (Input.GetKeyDown(KeyCode.X) && isGrounded && !isJumping)
        {
            animator.SetTrigger("Roll");
        }

        // Block logic
        if (Input.GetKeyDown(KeyCode.V) && isGrounded && !isJumping)
        {
            animator.SetTrigger("Block");
        }

        // Animator for running
        animator.SetInteger("AnimState", Mathf.Abs(movement) > 0f ? 1 : 0);
    }

    private void FixedUpdate()
    {
        // Move character
        transform.position += new Vector3(movement, 0f, 0f) * speed * Time.fixedDeltaTime;
    }

    void Jump()
    {
        rb2d.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if player lands on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
            animator.SetBool("Jump", false);
            animator.SetBool("JumpHit", false);
        }
    }
}
