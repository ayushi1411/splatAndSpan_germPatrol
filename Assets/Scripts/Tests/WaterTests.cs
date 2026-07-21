using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class WaterTests
{
    private GameObject playerObj;
    private PlayerController player;
    private PlayerWeapon weapon;
    private GameObject collectibleObj;

    [SetUp]
    public void SetUp()
    {
        playerObj = new GameObject("TestPlayer");
        playerObj.tag = "Player";
        player = playerObj.AddComponent<PlayerController>();
        weapon = playerObj.AddComponent<PlayerWeapon>();
        
        // Damage player for test
        typeof(PlayerController).GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(player, 80f);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObj);
        if (collectibleObj != null) Object.DestroyImmediate(collectibleObj);
    }

    private void InvokeOnCollect(Collectibles item)
    {
        MethodInfo method = typeof(Collectibles).GetMethod("OnCollect", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(item, new object[] { player, weapon });
    }

    [Test]
    public void TestWaterHealing()
    {
        collectibleObj = new GameObject("Water");
        Water water = collectibleObj.AddComponent<Water>();
        
        InvokeOnCollect(water);
        
        Assert.AreEqual(100f, player.CurrentHealth, "Water did not heal the player correctly.");
    }
}
