using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oxygen : Collectibles
{
    [SerializeField] private int oxygenAmount = 1;
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
        playerController.AddOxygen(oxygenAmount);
        playerWeapon.AddBleachCharge(bleachChargeAmount);
        Destroy(gameObject);
    }
}
