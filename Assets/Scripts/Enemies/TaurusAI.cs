using UnityEngine;

public class TaurusAI : MonoBehaviour
{
    public Transform player;

    private bool isFliped = false;


    public void LookAtPlayer()
    {
        Vector3 flippeed = transform.localScale;
        flippeed.z *= -1f;

        //
        if(transform.position.x > player.position.x && isFliped)
        {
            transform.localScale = flippeed;
            transform.Rotate(0f, 180f, 0f);
            isFliped = false;
        }else if(transform.position.x < player.position.x && !isFliped)
        {
            transform.localScale = flippeed;
            transform.Rotate(0f, 180f, 0f);
            isFliped = true;
        }
        //
    }
}
