using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class BackpackUpgradeTests
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
    public void TestBackpackUpgradeCollection()
    {
        collectibleObj = new GameObject("Upgrade");
        BackpackUpgrade upgrade = collectibleObj.AddComponent<BackpackUpgrade>();
        
        InvokeOnCollect(upgrade);
        
        Assert.Greater(player.MaxCarbon, 60, "Backpack upgrade did not increase max carbon.");
        Assert.Greater(player.MaxOxygen, 30, "Backpack upgrade did not increase max oxygen.");
    }
}
