using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileType { Foam, Solvent, Bleach, EnemySpore, EnemySpike, EnemyAcid }

    [Header("Projectile Properties")]
    public ProjectileType type;
    public float speed = 15f;
    public float damage = 10f;
    public float lifetime = 3.0f;

    [Header("Bleach Settings")]
    public int maxPierces = 3;
    private int currentPierces = 0;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.right * speed;
        }
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Cross-Projectile collisions are ignored
        if (collision.CompareTag("PlayerProjectile") || collision.CompareTag("EnemyProjectile") || collision.gameObject.layer == gameObject.layer)
        {
            return;
        }

        // 2. Obstacles/Walls Collision
        if (collision.CompareTag("Obstacle") || collision.CompareTag("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            HandleObstacleCollision(collision);
            return;
        }

        // 3. Enemy Collision (Player projectiles hitting enemies)
        if (IsPlayerProjectile() && (collision.CompareTag("Enemy") || collision.CompareTag("TestDummy")))
        {
            HandleEnemyCollision(collision);
            return;
        }

        // 4. Player Collision (Enemy projectiles hitting player)
        if (IsEnemyProjectile() && collision.CompareTag("Player"))
        {
            HandlePlayerCollision(collision);
            return;
        }
    }

    private bool IsPlayerProjectile()
    {
        return type == ProjectileType.Foam || type == ProjectileType.Solvent || type == ProjectileType.Bleach;
    }

    private bool IsEnemyProjectile()
    {
        return type == ProjectileType.EnemySpore || type == ProjectileType.EnemySpike || type == ProjectileType.EnemyAcid;
    }

    private void HandleObstacleCollision(Collider2D collision)
    {
        // Organic Barriers (Cholesterol gates, boss walls)
        bool isOrganicBarrier = collision.CompareTag("OrganicBarrier") || collision.gameObject.GetComponent<OrganicWall>() != null;

        if (isOrganicBarrier)
        {
            if (type == ProjectileType.Foam)
            {
                // Foam bounces off organic barriers with 0 damage
                BounceOff(collision);
                // Play rubbery bubble splash particle/sound logic here in the future
                return;
            }
            else if (type == ProjectileType.Solvent)
            {
                // Solvent melts organic barriers
                var wall = collision.gameObject.GetComponent<OrganicWall>();
                if (wall != null)
                {
                    wall.TakeDamage(damage);
                }
                Destroy(gameObject);
                return;
            }
            else if (type == ProjectileType.Bleach)
            {
                // Bleach pierces organic barriers and damages them
                var wall = collision.gameObject.GetComponent<OrganicWall>();
                if (wall != null)
                {
                    wall.TakeDamage(damage);
                }
                // Bleach does not destroy immediately unless max pierces reached
                CheckBleachPierce();
                return;
            }
        }

        // Standard walls destroy projectiles immediately (no bounce-back self-harm)
        Destroy(gameObject);
    }

    private void HandleEnemyCollision(Collider2D collision)
    {
        // 1. TestDummy hit reaction
        var dummy = collision.gameObject.GetComponent<TestDummy>();
        if (dummy != null)
        {
            if (type == ProjectileType.Foam)
            {
                dummy.TakeFoamHit(damage);
                Destroy(gameObject);
            }
            else if (type == ProjectileType.Solvent)
            {
                dummy.TakeSolventHit(damage);
                Destroy(gameObject);
            }
            else if (type == ProjectileType.Bleach)
            {
                dummy.TakeBleachHit(damage);
                CheckBleachPierce();
            }
            return;
        }

        // 2. Standard Pathogen hit reaction (Amoeba, Capsid, Boss, etc.)
        var amoeba = collision.gameObject.GetComponent<AmoebaEnemy>();
        if (amoeba != null)
        {
            if (type == ProjectileType.Foam)
            {
                amoeba.TakeDamage(damage);
                amoeba.ApplySlowDebuff(0.30f, 2.0f); // Slows target by 30% for 2.0s
                Destroy(gameObject);
            }
            else if (type == ProjectileType.Solvent)
            {
                amoeba.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (type == ProjectileType.Bleach)
            {
                amoeba.TakeDamage(damage);
                CheckBleachPierce();
            }
            return;
        }

        // Capsid charger hit reaction
        var capsid = collision.gameObject.GetComponent<ViralCapsidEnemy>();
        if (capsid != null)
        {
            // Calculate collision angle for front/rear detection
            Vector2 hitDirection = transform.right; // Projectile travel direction
            if (type == ProjectileType.Foam)
            {
                capsid.TakeProjectileHit(hitDirection, damage, type);
                Destroy(gameObject);
            }
            else if (type == ProjectileType.Solvent)
            {
                capsid.TakeProjectileHit(hitDirection, damage, type);
                Destroy(gameObject);
            }
            else if (type == ProjectileType.Bleach)
            {
                capsid.TakeProjectileHit(hitDirection, damage, type);
                CheckBleachPierce();
            }
            return;
        }

        // Boss hit reaction
        var boss = collision.gameObject.GetComponent<ParasiteBoss>();
        if (boss != null)
        {
            if (type == ProjectileType.Foam)
            {
                boss.TakeDamage(damage, false); // No shield multiplier
                Destroy(gameObject);
            }
            else if (type == ProjectileType.Solvent)
            {
                boss.TakeDamage(damage, true); // Solvent deals 3x to Boss Shield
                Destroy(gameObject);
            }
            else if (type == ProjectileType.Bleach)
            {
                boss.TakeDamage(damage, false);
                CheckBleachPierce();
            }
            return;
        }
    }

    private void HandlePlayerCollision(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    private void CheckBleachPierce()
    {
        currentPierces++;
        if (currentPierces >= maxPierces)
        {
            Destroy(gameObject);
        }
    }

    private void BounceOff(Collider2D collision)
    {
        if (rb == null) return;

        // Simple reflection angle calculation using normal to collider boundary
        Vector3 contactPoint = collision.ClosestPoint(transform.position);
        Vector2 normal = ((Vector2)transform.position - (Vector2)contactPoint).normalized;
        Vector2 reflectedVelocity = Vector2.Reflect(rb.velocity, normal);
        
        rb.velocity = reflectedVelocity;

        // Re-align the projectile's orientation to match new direction
        if (reflectedVelocity.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(reflectedVelocity.y, reflectedVelocity.x) * Mathf.RadDeg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
