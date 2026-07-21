using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class OxygenTests
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
        
        player.maxOxygen = 100;
        player.currentOxygen = 0;
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
    public void TestOxygenCollection()
    {
        collectibleObj = new GameObject("Oxygen");
        Oxygen oxygen = collectibleObj.AddComponent<Oxygen>();
        
        InvokeOnCollect(oxygen);
        
        Assert.AreEqual(1, player.currentOxygen, "Oxygen collectible did not add 1 oxygen.");
    }
}
