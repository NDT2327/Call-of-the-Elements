using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    private CeilingTrap ceilingTrap;
    void Start()
    {
        ceilingTrap = GetComponentInParent<CeilingTrap>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ceilingTrap.ActivateTrap();
        }
    }
}
