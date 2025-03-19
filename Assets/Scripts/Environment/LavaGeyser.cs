using UnityEngine;

public class LavaGeyser : MonoBehaviour
{
    public ParticleSystem fireEffect;
    public float damage = 20f;
    public float fireInterval = 3f;
    private bool isActive = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("TriggerFire", 0f, fireInterval);
    }

    void TriggerFire()
    {
        isActive = !isActive;
        fireEffect.gameObject.SetActive(isActive);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.CompareTag("Player"))
        {
            Debug.Log("Player hit by Lava Geyser!");
        }
    }
}
