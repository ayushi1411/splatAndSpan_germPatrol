using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carbon : Collectibles
{
    [SerializeField] private int carbonAmount = 1;
    [SerializeField] private float bleachChargeAmount = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void OnCollect(PlayerController playerController, PlayerWeapon playerWeapon)
    {
        playerController.AddCarbon(carbonAmount);
        playerWeapon.AddBleachCharge(bleachChargeAmount);
        Destroy(gameObject);
    }
}
