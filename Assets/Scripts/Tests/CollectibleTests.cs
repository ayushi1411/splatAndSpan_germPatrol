using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class CollectibleTests
{
    private GameObject playerObj;
    private GameObject collectibleObj;
    private Collectibles collectible;

    [SetUp]
    public void SetUp()
    {
        playerObj = new GameObject("TestPlayer");
        collectibleObj = new GameObject("BaseCollectible");
        collectible = collectibleObj.AddComponent<Collectibles>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(collectibleObj);
    }

    [Test]
    public void TestStartSuctionSetsVariables()
    {
        // Test the base Collectible.cs logic directly
        collectible.StartSuction(playerObj.transform, 15f);
        
        // Use reflection to check private variables
        FieldInfo isPulledField = typeof(Collectibles).GetField("isBeingPulled", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo targetField = typeof(Collectibles).GetField("targetPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo speedField = typeof(Collectibles).GetField("pullSpeed", BindingFlags.NonPublic | BindingFlags.Instance);

        bool isPulled = (bool)isPulledField.GetValue(collectible);
        Transform target = (Transform)targetField.GetValue(collectible);
        float speed = (float)speedField.GetValue(collectible);

        Assert.IsTrue(isPulled, "StartSuction did not set isBeingPulled to true.");
        Assert.AreEqual(playerObj.transform, target, "StartSuction did not set the correct target player.");
        Assert.AreEqual(15f, speed, "StartSuction did not set the correct pull speed.");
    }
}
