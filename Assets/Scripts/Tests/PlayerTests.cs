using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerTests
{
    private GameObject playerObj;
    private PlayerController player;

    [SetUp]
    public void SetUp()
    {
        // Instantiates a clean mockup player before each test case runs
        playerObj = new GameObject("TestPlayer");
        player = playerObj.AddComponent<PlayerController>();
        
        // Initialization relies on PlayerController defaults.
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TestShieldAbsorption()
    {
        // Apply 20 damage: Shield absorbs 16 (80%), Health takes 4 (20%)
        player.TakeDamage(20f);
        
        Assert.AreEqual(84f, player.CurrentShield, 0.01f, "Shield did not absorb 80% of damage correctly.");
        Assert.AreEqual(96f, player.CurrentHealth, 0.01f, "Health did not take 20% bleed-through damage correctly.");
    }

    [Test]
    public void TestBackpackUpgrade()
    {
        // Max Carbon should increase from 60 to 80, Max Oxygen from 30 to 40
        player.UpgradeBackpack(20, 10);
        
        Assert.AreEqual(80, player.MaxCarbon, "Max Carbon did not upgrade correctly.");
        Assert.AreEqual(40, player.MaxOxygen, "Max Oxygen did not upgrade correctly.");
        
        // Ensure current stock doesn't magically fill up
        Assert.AreEqual(0, player.CurrentCarbon, "Current Carbon should not refill on upgrade.");
        Assert.AreEqual(0, player.CurrentOxygen, "Current Oxygen should not refill on upgrade.");
    }

    [Test]
    public void TestH2OFullHP()
    {
        // If Health is 100, picking up water should return false and not consume it
        bool consumed = player.CollectH2O(25f);
        Assert.IsFalse(consumed, "Water should not be consumed if health is full.");
        
        // Damage player to 80 HP
        typeof(PlayerController).GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(player, 80f);
        consumed = player.CollectH2O(25f);
        
        Assert.IsTrue(consumed, "Water should be consumed if health is below max.");
        Assert.AreEqual(100f, player.CurrentHealth, 0.01f, "Health should cap at 100 and not exceed max.");
    }

    [Test]
    public void TestCarbonAndOxygenClamping()
    {
        // Try to add 100 Carbon (Base max is 60)
        player.AddCarbon(100);
        Assert.AreEqual(60, player.CurrentCarbon, "Carbon should not exceed its maximum capacity.");

        // Try to add 100 Oxygen (Base max is 30)
        player.AddOxygen(100);
        Assert.AreEqual(30, player.CurrentOxygen, "Oxygen should not exceed its maximum capacity.");
    }
}
