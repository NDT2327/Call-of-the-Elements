using UnityEngine;

public class TaurusAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;

    private bool isFliped = false;

    public bool IsPlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) < detectionRange;
    }

    public void LookAtPlayer()
    {
        Vector3 flippeed = transform.localScale;
        flippeed.z *= -1f;

        //
        if (transform.position.x > player.position.x && isFliped)
        {
            transform.localScale = flippeed;
            transform.Rotate(0f, 180f, 0f);
            isFliped = false;
        }
        else if (transform.position.x < player.position.x && !isFliped)
        {
            transform.localScale = flippeed;
            transform.Rotate(0f, 180f, 0f);
            isFliped = true;
        }
        //
    }

    // 🛠 Vẽ phạm vi phát hiện trong Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Màu vòng tròn
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
