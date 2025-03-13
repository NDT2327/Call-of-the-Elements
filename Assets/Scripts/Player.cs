using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb2d;

    public float speed = 5f;
    public float jumpHeight = 5f;
    public bool isGrounded = false;

    private float movement;
    private bool facingRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        movement = Input.GetAxis("Horizontal");

        //Flip condition
        if ((movement < 0f && facingRight) || (movement > 0f && !facingRight))
        {
            facingRight = !facingRight;
            transform.eulerAngles = new Vector3(0f, facingRight ? 0f : -180f, 0f);
        }

        //Single Jump (Ground Condition)
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            Jump();
            isGrounded = !isGrounded;
        }
    }

    private void FixedUpdate()
    {
        //Move
        transform.position += new Vector3(movement, 0f, 0f) * speed * Time.fixedDeltaTime;
    }

    //Control Jump
    void Jump()
    {
        rb2d.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
    }

    //Manage Collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Jump Condition (Player collides ground)
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = !isGrounded;
        }
    }
}
