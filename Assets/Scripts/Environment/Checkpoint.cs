using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator animator;
    private bool isActivated = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            animator.SetTrigger("Activate");
            GameManager.Instance.SetCheckpoint(transform.position);
        }
    }
}
