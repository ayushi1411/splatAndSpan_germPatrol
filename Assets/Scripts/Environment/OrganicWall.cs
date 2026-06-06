using UnityEngine;

public class OrganicWall : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP = 100f;

    public Sprite healthySprite;
    public Sprite crackedSprite;
    public Sprite crumblingSprite;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisuals();
    }

    public void TakeDamage(float damage)
    {
        currentHP = Mathf.Max(0f, currentHP - damage);
        UpdateVisuals();
        
        if (currentHP <= 0f)
        {
            DestroyWall();
        }
    }

    private void UpdateVisuals()
    {
        if (spriteRenderer == null) return;

        float hpPercent = currentHP / maxHP;

        if (hpPercent > 0.66f)
        {
            spriteRenderer.sprite = healthySprite;
        }
        else if (hpPercent > 0.33f)
        {
            spriteRenderer.sprite = crackedSprite;
        }
        else
        {
            spriteRenderer.sprite = crumblingSprite;
        }
    }

    private void DestroyWall()
    {
        Debug.Log("[OrganicWall] Destructible barrier destroyed!");
        Destroy(gameObject);
    }
}
