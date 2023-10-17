using UnityEngine;

public class SpawnVFX : MonoBehaviour
{
    /// <summary>
    /// Spawns a VFX prefab and applies a scale modifier
    /// </summary>
    /// <param name="vfx">VFX prefab to spawn</param>
    /// <param name="spawnPos">VFX spawn Transform</param>
    /// <param name="scaleMod">Scale modifier</param>
    public void SpawnScaledVFX(GameObject vfx, Transform spawnPos, float scaleMod)
    {
        GameObject vfxToSpawn = Instantiate(vfx, spawnPos);
        vfxToSpawn.transform.localScale *= scaleMod;
    }

    /// <summary>
    /// Spawns a VFX prefab and applies a scale modifier randomly chosen between two values
    /// </summary>
    /// <param name="vfx">VFX prefab to spawn</param>
    /// <param name="spawnPos">VFX spawn Transform</param>
    /// <param name="scaleModMin">Scale modifier minimum value</param>
    /// <param name="scaleModMax">Scale modifier maximum value</param>
    /// <returns></returns>
    public void SpawnScaledVFXRandom(GameObject vfx, Transform spawnPos, float scaleModMin, float scaleModMax)
    {
        float scaleMod = Random.Range(scaleModMin, scaleModMax);

        SpawnScaledVFX(vfx, spawnPos, scaleMod);
    }
}
