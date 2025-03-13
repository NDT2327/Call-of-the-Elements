using UnityEngine;

public class Player : MonoBehaviour
{
    private float movement;
    public float speed = 5f;
    private bool facingRight = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
          
    }

    // Update is called once per frame
    void Update()
    {
        movement = Input.GetAxis("Horizontal");
        if ((movement < 0f && facingRight) || (movement > 0f && !facingRight))
        {
            facingRight = !facingRight;
            transform.eulerAngles = new Vector3(0f, facingRight ? 0f : -180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(movement, 0f, 0f) * speed * Time.fixedDeltaTime;
    }
}
