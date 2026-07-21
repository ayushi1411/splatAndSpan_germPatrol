using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioMatter : Collectibles
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
        playerController.AddBioMatter(scoreValue);
        Destroy(gameObject);
    }
}
