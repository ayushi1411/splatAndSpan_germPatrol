using UnityEngine;

public class AmoebaEnemy : MonoBehaviour
{
    public void TakeDamage(float damage)
    {
        Debug.Log($"[AmoebaEnemy] Took {damage} damage");
    }

    public void ApplySlowDebuff(float factor, float duration)
    {
        Debug.Log($"[AmoebaEnemy] Slowed by {factor * 100}% for {duration} seconds");
    }

    public void TakeVacuumDamage(float damage)
    {
        Debug.Log($"[AmoebaEnemy] Took {damage} vacuum damage");
    }
}
