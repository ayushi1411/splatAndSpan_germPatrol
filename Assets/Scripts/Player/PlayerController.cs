using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float baseMoveSpeed = 8.0f;
    public float linearDrag = 3.0f;
    private Rigidbody2D rb2d;
    private Vector2 moveInput;
    private float currentMoveSpeed;

    [Header("Aiming & Nozzle")]
    public Transform nozzleTransform;
    private Camera mainCamera;

    [Header("Health & Shield")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float maxShield = 100f;
    public float currentShield = 100f;
    public float shieldRegenRate = 5.0f;
    public float shieldRegenDelay = 4.0f;
    private float lastDamageTime;

    [Header("Element Inventory")]
    public int currentCarbon = 0;
    public int maxCarbon = 60;
    public int currentOxygen = 0;
    public int maxOxygen = 30;
    public int currentBioMatter = 0;

    [Header("Vacuum System")]
    public float passiveSuctionRadius = 5.0f;
    public float activeSuctionRadius = 6.0f;
    public float activeSuctionAngle = 60.0f;
    public float activeSuctionForce = 4.0f;
    public float activeSuctionDamage = 5.0f; // DPS
    
    [Header("Vacuum Heat")]
    public float currentVacuumHeat = 0f;
    public float maxVacuumHeat = 100f;
    public float heatAccumulationRate = 25f; // Heat per second
    public float passiveCoolingRate = 33.3f; // Heat per second
    public float overheatLockoutDuration = 2.0f;
    
    private bool isActiveVacuumActive = false;
    private bool isVacuumOverheated = false;
    private float overheatLockoutTimer = 0f;

    [Header("Visuals")]
    public SpriteRenderer bodySpriteRenderer;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.drag = linearDrag;
            rb2d.gravityScale = 0f;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        mainCamera = Camera.main;
        currentMoveSpeed = baseMoveSpeed;
        lastDamageTime = -shieldRegenDelay; // Allow immediate regen if needed
    }

    private void Update()
    {
        HandleInputs();
        HandleVacuumHeat();
        HandleShieldRegeneration();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleAiming();
        ApplyVacuumForces();
    }

    private void HandleInputs()
    {
        // 8-way Movement Input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // Active Vacuum Input (Spacebar)
        if (Input.GetKey(KeyCode.Space) && !isVacuumOverheated)
        {
            isActiveVacuumActive = true;
        }
        else
        {
            isActiveVacuumActive = false;
        }
    }

    private void HandleVacuumHeat()
    {
        if (isVacuumOverheated)
        {
            isActiveVacuumActive = false;
            overheatLockoutTimer -= Time.deltaTime;
            if (overheatLockoutTimer <= 0f)
            {
                isVacuumOverheated = false;
                currentMoveSpeed = baseMoveSpeed; // Restore speed
            }
            // Heat cools down during lockout too
            currentVacuumHeat = Mathf.Max(0f, currentVacuumHeat - passiveCoolingRate * Time.deltaTime);
        }
        else
        {
            if (isActiveVacuumActive)
            {
                currentVacuumHeat = Mathf.Min(maxVacuumHeat, currentVacuumHeat + heatAccumulationRate * Time.deltaTime);
                if (currentVacuumHeat >= maxVacuumHeat)
                {
                    isVacuumOverheated = true;
                    overheatLockoutTimer = overheatLockoutDuration;
                    isActiveVacuumActive = false;
                    currentMoveSpeed = baseMoveSpeed * 0.75f; // 25% speed penalty (venting steam)
                    // Trigger sound/visual alarm here in the future
                }
            }
            else
            {
                currentVacuumHeat = Mathf.Max(0f, currentVacuumHeat - passiveCoolingRate * Time.deltaTime);
            }
        }
    }

    private void HandleMovement()
    {
        if (rb2d == null) return;

        rb2d.velocity = moveInput * currentMoveSpeed;

        // Flip body sprite to match horizontal movement vector
        if (moveInput.x > 0.01f && bodySpriteRenderer != null)
        {
            bodySpriteRenderer.flipX = false;
        }
        else if (moveInput.x < -0.01f && bodySpriteRenderer != null)
        {
            bodySpriteRenderer.flipX = true;
        }
    }

    private void HandleAiming()
    {
        if (nozzleTransform == null || mainCamera == null) return;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - nozzleTransform.position;
        direction.z = 0; // Maintain 2D plane

        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            nozzleTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void HandleShieldRegeneration()
    {
        if (Time.time - lastDamageTime >= shieldRegenDelay)
        {
            if (currentShield < maxShield)
            {
                currentShield = Mathf.Min(maxShield, currentShield + shieldRegenRate * Time.deltaTime);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        lastDamageTime = Time.time;

        if (currentShield > 0f)
        {
            float shieldDmg = damage * 0.8f;
            float healthDmg = damage * 0.2f;

            if (currentShield >= shieldDmg)
            {
                currentShield -= shieldDmg;
                currentHealth -= healthDmg;
            }
            else
            {
                // Shield fully depleted by damage split
                float remainingShield = currentShield;
                currentShield = 0f;
                // Remainder of that 80% split is applied directly to health
                float unusedShieldDmg = shieldDmg - remainingShield;
                // Add the direct 20% health damage and the remainder
                currentHealth -= (healthDmg + (unusedShieldDmg / 0.8f));
            }
        }
        else
        {
            // No shield, take 100% damage to health
            currentHealth -= damage;
        }

        currentHealth = Mathf.Max(0f, currentHealth);
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        // Death logic handled by GameManager/UI, print for now
        Debug.Log("Buster has been defeated!");
    }

    public bool CollectH2O(float healingAmount)
    {
        // H2O is ONLY consumed if Buster is below max HP (100)
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + healingAmount);
            return true; // Droplet consumed
        }
        return false; // Droplet remains physical on floor
    }

    public void AddCarbon(int amount)
    {
        currentCarbon = Mathf.Min(maxCarbon, currentCarbon + amount);
    }

    public void AddOxygen(int amount)
    {
        currentOxygen = Mathf.Min(maxOxygen, currentOxygen + amount);
    }

    public void AddBioMatter(int amount)
    {
        currentBioMatter += amount;
    }

    public void UpgradeBackpack(int carbonInc, int oxygenInc)
    {
        // Permanently increase backpack capacities
        maxCarbon = Mathf.Min(100, maxCarbon + carbonInc);
        maxOxygen = Mathf.Min(50, maxOxygen + oxygenInc);
        
        // Note: Does NOT refill elements. Player must harvest elements to fill new capacity.
        Debug.Log($"Backpack upgraded! Max Carbon: {maxCarbon}, Max Oxygen: {maxOxygen}");
    }

    private void ApplyVacuumForces()
    {
        // 1. Passive Magnet: Draw items within passiveSuctionRadius
        Collider2D[] passiveColliders = Physics2D.OverlapCircleAll(transform.position, passiveSuctionRadius);
        foreach (var col in passiveColliders)
        {
            if (col.CompareTag("Collectible"))
            {
                PullCollectible(col.gameObject, 1.0f);
            }
        }

        // 2. Active Vacuum Suction Cone (60 degrees, 6 units)
        if (isActiveVacuumActive)
        {
            Vector3 nozzleDir = nozzleTransform.right; // Facing direction of the rotating nozzle
            Collider2D[] activeColliders = Physics2D.OverlapCircleAll(transform.position, activeSuctionRadius);
            
            foreach (var col in activeColliders)
            {
                Vector3 toCollider = col.transform.position - transform.position;
                float distance = toCollider.magnitude;
                if (distance > activeSuctionRadius || distance < 0.1f) continue;

                float angleToCollider = Vector3.Angle(nozzleDir, toCollider.normalized);

                if (angleToCollider <= activeSuctionAngle * 0.5f)
                {
                    // Inside suction cone!
                    if (col.CompareTag("Collectible"))
                    {
                        // Pull at triple velocity, ignore obstacles (trigger pulls through walls)
                        PullCollectible(col.gameObject, 3.0f);
                    }
                    else if (col.CompareTag("Enemy") || col.CompareTag("TestDummy"))
                    {
                        // Pull small enemies/test-dummies at 4 units/sec and deal 5 damage/sec
                        PullEnemy(col.gameObject);
                    }
                }
            }
        }
    }

    private void PullCollectible(GameObject collectible, float speedMultiplier)
    {
        // Move the collectible towards player position
        float speed = 5.0f * speedMultiplier;
        collectible.transform.position = Vector3.MoveTowards(
            collectible.transform.position, 
            transform.position, 
            speed * Time.fixedDeltaTime
        );
    }

    private void PullEnemy(GameObject enemy)
    {
        // Apply force or move towards player
        Vector3 direction = (transform.position - enemy.transform.position).normalized;
        
        // Directly move the enemy or apply velocity towards player
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            enemyRb.velocity = direction * activeSuctionForce;
        }
        else
        {
            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position, 
                transform.position, 
                activeSuctionForce * Time.fixedDeltaTime
            );
        }

        // Deal 5 damage/sec (based on FixedUpdate delta time)
        // Check for TestDummy or Enemy components
        var dummy = enemy.GetComponent<TestDummy>();
        if (dummy != null)
        {
            dummy.TakeVacuumDamage(activeSuctionDamage * Time.fixedDeltaTime);
        }
        
        // Future-proofing for standard enemies
        var pathogen = enemy.GetComponent<AmoebaEnemy>();
        if (pathogen != null)
        {
            pathogen.TakeVacuumDamage(activeSuctionDamage * Time.fixedDeltaTime);
        }
    }

    public bool IsActiveVacuuming()
    {
        return isActiveVacuumActive;
    }

    public bool IsVacuumOverheated()
    {
        return isVacuumOverheated;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize passive suction range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, passiveSuctionRadius);

        // Visualize active suction cone
        if (nozzleTransform != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -activeSuctionAngle * 0.5f) * nozzleTransform.right;
            Vector3 leftBoundary = Quaternion.Euler(0, 0, activeSuctionAngle * 0.5f) * nozzleTransform.right;
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary * activeSuctionRadius);
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary * activeSuctionRadius);
        }
    }
}
