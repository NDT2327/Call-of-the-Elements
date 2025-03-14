using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb2d;
    public Animator animator;
    public GameObject blockFlash; // Hiệu ứng flash khi block

    public float speed = 5f;
    public float jumpHeight = 5f;
    public bool isGrounded = false;

    private float movement;
    private bool facingRight = true;
    private int attackCount = 0;
    private float lastAttackTime;
    public float attackResetTime = 0.5f; // Time to reset attack count

    void Start()
    {
        if (blockFlash != null)
        {
            blockFlash.SetActive(false); // Ẩn blockFlash ban đầu
        }
    }

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
        if (Input.GetKeyDown(KeyCode.K) && isGrounded)
        {
            Jump();
            isGrounded = false;
            animator.SetBool("Grounded", false);
            animator.SetTrigger("Jump");
        }

        // Attack logic
        if (Input.GetKeyDown(KeyCode.J))
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
        if (Input.GetKeyDown(KeyCode.L) && isGrounded)
        {
            animator.SetTrigger("Roll");
        }

        // Block logic
        if (Input.GetKeyDown(KeyCode.S) && isGrounded)
        {
            animator.SetTrigger("Block");
            ShowBlockFlash(); // Hiển thị blockFlash khi Block
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
            animator.SetBool("Grounded", true);
        }
    }

    // Hiển thị blockFlash trong 0.3 giây
    void ShowBlockFlash()
    {
        if (blockFlash != null)
        {
            blockFlash.SetActive(true);
            Invoke("HideBlockFlash", 0.3f);
        }
    }

    // Ẩn blockFlash
    void HideBlockFlash()
    {
        if (blockFlash != null)
        {
            blockFlash.SetActive(false);
        }
    }
}
