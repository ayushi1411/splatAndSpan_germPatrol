using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class BioMatterTests
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
        
        player.currentBioMatter = 0;
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
    public void TestBioMatterCollection()
    {
        collectibleObj = new GameObject("BioMatter");
        BioMatter bio = collectibleObj.AddComponent<BioMatter>();
        
        InvokeOnCollect(bio);
        
        Assert.AreEqual(10, player.currentBioMatter, "BioMatter did not add the correct score value.");
    }
}
