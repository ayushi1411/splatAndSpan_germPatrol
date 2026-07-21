using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Collectibles
{
    [SerializeField] private float healAmount = 25f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool CanBePulled(PlayerController player)
    {
        return player.CurrentHealth < player.MaxHealth;
    }

    protected override void OnCollect(PlayerController playerController, PlayerWeapon playerWeapon)
    {
        bool consumed = playerController.CollectH2O(healAmount);
        if (consumed)
        {
            Destroy(gameObject);
        }
    }
}
