using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private LayerMask groundLayer;
    //[SerializeField]
    //private int maxJumps = 2; // Số lần nhảy tối đa

    SpriteRenderer spriteRenderer;

    private Rigidbody2D body;

    private BoxCollider2D boxCollider;

    //private int jumpCount;

    private Animator anim;

    //private bool grounded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        
        anim.SetBool("run", horizontalInput != 0);
        
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);
        
        //flip player left - right
        if (horizontalInput > 0.01f) 
            spriteRenderer.flipX = false;
        else if (horizontalInput < -0.01f) 
            spriteRenderer.flipX = true;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            Jump();
        }
        anim.SetBool("grounded", isGrounded());
    }

    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
        anim.SetTrigger("jump");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
}
