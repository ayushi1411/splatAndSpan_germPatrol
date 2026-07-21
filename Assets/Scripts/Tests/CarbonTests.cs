using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class CarbonTests
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
        
        // Stats initialize to defaults.
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
    public void TestCarbonCollection()
    {
        collectibleObj = new GameObject("Carbon");
        Carbon carbon = collectibleObj.AddComponent<Carbon>();
        
        InvokeOnCollect(carbon);
        
        Assert.AreEqual(1, player.CurrentCarbon, "Carbon collectible did not add 1 carbon.");
    }
}
