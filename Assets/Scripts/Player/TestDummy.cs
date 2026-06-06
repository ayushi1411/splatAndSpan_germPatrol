using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class TestDummy : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float flashTimer = 0f;
    private float flashDuration = 0.1f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Set tags so projectiles and player vacuum can identify it
        gameObject.tag = "TestDummy";
    }

    private void Update()
    {
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f && spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    public void TakeFoamHit(float damage)
    {
        Debug.Log($"[TestDummy] Hit by Sanitization Foam! Damage: {damage}. Slow overlay applied.");
        TriggerFlash(Color.cyan);
    }

    public void TakeSolventHit(float damage)
    {
        Debug.Log($"[TestDummy] Hit by Solvent Spray! Damage: {damage}. Chemical sizzle effect.");
        TriggerFlash(Color.green);
    }

    public void TakeBleachHit(float damage)
    {
        Debug.Log($"[TestDummy] Hit by Concentrated Bleach! Damage: {damage}. Pierced!");
        TriggerFlash(Color.white);
    }

    public void TakeVacuumDamage(float damageRate)
    {
        Debug.Log($"[TestDummy] Being vacuumed! Receiving {damageRate * (1.0f / Time.fixedDeltaTime):F1} damage per second.");
        TriggerFlash(Color.yellow);
    }

    private void TriggerFlash(Color flashColor)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
            flashTimer = flashDuration;
        }
    }
}
