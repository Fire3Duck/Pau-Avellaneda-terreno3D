using UnityEngine;

public class DamageOnInteract : MonoBehaviour
{
    public int damage = 10;
    public bool oneShot = false;
    bool hasDamaged = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (hasDamaged && oneShot) return;

        if (other.CompareTag("Player"))
        {
            PlayerController p_controller = other.GetComponent<PlayerController>();
            if (p_controller != null)
            {
                p_controller.TakeDamage(damage);
                if (oneShot)
                {
                    hasDamaged = true;
                }
            }
        }
    }
}