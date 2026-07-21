using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon References")]
    public PlayerController playerController;
    public Transform firePoint;

    [Header("Sanitization Foam (Left-Click)")]
    public GameObject foamPrefab;
    public float foamFireRate = 6.0f; // Shots per second
    private float nextFoamTime = 0f;

    [Header("Solvent Spray (Right-Click)")]
    public GameObject solventPrefab;
    public float solventFireRate = 10.0f; // Continuous projectile rate
    private float nextSolventTime = 0f;
    public float solventCarbonCostPerSec = 2.0f;
    public float solventOxygenCostPerSec = 1.0f;

    [Header("Concentrated Bleach Ultimate (Q-Key)")]
    public GameObject bleachPrefab;
    public float bleachCharge = 0f; // Percentage 0 to 100
    public float maxBleachCharge = 100f;

    private void Start()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        // Don't allow shooting while active vacuuming or overheated
        if (playerController != null && (playerController.IsActiveVacuuming() || playerController.IsVacuumOverheated()))
        {
            return;
        }

        HandleFoamShooting();
        HandleSolventShooting();
        HandleBleachUltimate();
    }

    private void HandleFoamShooting()
    {
        if (Input.GetButton("Fire1")) // Left Mouse Button
        {
            if (Time.time >= nextFoamTime)
            {
                nextFoamTime = Time.time + (1.0f / foamFireRate);
                FireFoam();
            }
        }
    }

    private void HandleSolventShooting()
    {
        if (Input.GetButton("Fire2")) // Right Mouse Button
        {
            // Calculate per-shot costs
            float carbonCostPerShot = solventCarbonCostPerSec / solventFireRate;
            float oxygenCostPerShot = solventOxygenCostPerSec / solventFireRate;

            if (playerController.CurrentCarbon >= carbonCostPerShot && playerController.CurrentOxygen >= oxygenCostPerShot)
            {
                if (Time.time >= nextSolventTime)
                {
                    nextSolventTime = Time.time + (1.0f / solventFireRate);
                    
                    // Deduct costs
                    // We can cast to int if we store elements as int, or handle fractional costs.
                    // The GDD says: "2 Carbon + 1 Oxygen per second".
                    // Since inventory elements are ints, let's deduct 2 Carbon and 1 Oxygen per second.
                    // To do this simply, we can check if 1 second has elapsed or accumulate fractional costs.
                    // Let's accumulate fractional costs or just deduct 2 Carbon and 1 Oxygen over the course of a second.
                    // A simple way is to use a counter that deducts 2 Carbon and 1 Oxygen when they accumulate to >= 1.
                    // Let's do that!
                    
                    DeductSolventCosts(carbonCostPerShot, oxygenCostPerShot);
                    FireSolvent();
                }
            }
            else
            {
                // Play out of ammo effect/sound click
            }
        }
    }

    private float accumulatedCarbonCost = 0f;
    private float accumulatedOxygenCost = 0f;

    private void DeductSolventCosts(float carbonCost, float oxygenCost)
    {
        accumulatedCarbonCost += carbonCost;
        accumulatedOxygenCost += oxygenCost;

        if (accumulatedCarbonCost >= 1.0f)
        {
            int intC = Mathf.FloorToInt(accumulatedCarbonCost);
            playerController.ConsumeCarbon(intC);
            accumulatedCarbonCost -= intC;
        }

        if (accumulatedOxygenCost >= 1.0f)
        {
            int intO = Mathf.FloorToInt(accumulatedOxygenCost);
            playerController.ConsumeOxygen(intO);
            accumulatedOxygenCost -= intO;
        }
    }

    private void HandleBleachUltimate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (bleachCharge >= maxBleachCharge)
            {
                FireBleach();
                bleachCharge = 0f; // Consume entire charge
            }
        }
    }

    private void FireFoam()
    {
        if (foamPrefab != null && firePoint != null)
        {
            Instantiate(foamPrefab, firePoint.position, firePoint.rotation);
        }
    }

    private void FireSolvent()
    {
        if (solventPrefab != null && firePoint != null)
        {
            Instantiate(solventPrefab, firePoint.position, firePoint.rotation);
        }
    }

    private void FireBleach()
    {
        if (bleachPrefab != null && firePoint != null)
        {
            Instantiate(bleachPrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void AddBleachCharge(float amount)
    {
        bleachCharge = Mathf.Min(maxBleachCharge, bleachCharge + amount);
    }
}
