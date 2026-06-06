using UnityEngine;

public class ViralCapsidEnemy : MonoBehaviour
{
    public void TakeProjectileHit(Vector2 hitDirection, float damage, Projectile.ProjectileType type)
    {
        Debug.Log($"[ViralCapsidEnemy] Hit by {type}! Direction: {hitDirection}, Damage: {damage}");
    }
}
