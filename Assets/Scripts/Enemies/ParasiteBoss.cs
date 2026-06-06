using UnityEngine;

public class ParasiteBoss : MonoBehaviour
{
    public void TakeDamage(float damage, bool isSolvent)
    {
        Debug.Log($"[ParasiteBoss] Took {damage} damage (Is Solvent: {isSolvent})");
    }
}
