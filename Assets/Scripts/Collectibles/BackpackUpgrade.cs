using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackUpgrade : Collectibles
{
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
        // Generate a random number between 1 and 20 (inclusive)
        int carbonUpgradeAmount = Random.Range(1, 21);

        // Generate a random number for Oxygen (maybe 1 to 10 to keep the 2:1 ratio?)
        int oxygenUpgradeAmount = Random.Range(1, 11);

        playerController.UpgradeBackpack(carbonUpgradeAmount, oxygenUpgradeAmount);
        Destroy(gameObject);
    }
}
