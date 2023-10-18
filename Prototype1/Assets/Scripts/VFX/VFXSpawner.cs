using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    #region private methods
                                                                        // where do we want variables? would assume on the other 
    /// <summary>                                                       // scripts where these are being called and not on here,
    /// Spawns an instance of a VFX prefab                              // but i can put them here if folks want but i can put
    /// </summary>                                                      //them here if folks want
    /// <param name="vfx">VFX prefab to spawn</param>
    /// <param name="spawnPos">VFX spawn transform</param>
    /// <returns></returns>
    private GameObject SpawnVFXMaster(GameObject vfx, Transform spawnPos)
    {
        GameObject vfxToSpawn = Instantiate(vfx, spawnPos);
        return vfxToSpawn;
    }

    /// <summary>
    /// Spawns an instance of a VFX prefab, destroys object after specified time
    /// </summary>
    /// <param name="vfx">VFX prefab to spawn</param>
    /// <param name="spawnPos">VFX spawn transform</param>
    /// <param name="lifetime">Lifetime of VFX, delay before destroying</param>
    /// <returns></returns>
    private GameObject SpawnVFXMasterTimed(GameObject vfx, Transform spawnPos, float lifetime)
    {
        GameObject vfxToSpawn = SpawnVFXMaster(vfx, spawnPos);
        DestroyVFXMaster(vfxToSpawn, lifetime);
        return vfxToSpawn;
    }

    /// <summary>
    /// Destroys given instance of a VFX prefab
    /// </summary>
    /// <param name="vfx">VFX prefab instance to destroy</param>
    /// <param name="lifetime">Lifetime of VFX, delay before destroying</param>
    private void DestroyVFXMaster(GameObject vfx, float lifetime)
    {
        Destroy(vfx, lifetime);
    }

    #endregion

    #region public methods main

    /// <summary>
    /// Spawns a VFX prefab
    /// </summary>
    /// <param name="vfx">VFX prefab to spawn</param>
    /// <param name="spawnPos"></param>
    public void SpawnVFX(GameObject vfx, Transform spawnPos)
    {
        SpawnVFXMaster(vfx, spawnPos);
    }

    /// <summary>
    /// Spawns a VFX prefab and applies a scale modifier
    /// </summary>
    /// <param name="vfx">VFX prefab to spawn</param>
    /// <param name="spawnPos">VFX spawn transform</param>
    /// <param name="scaleMod">Scale modifier</param>
    public void SpawnVFXScale(GameObject vfx, Transform spawnPos, float scaleMod)
    {
        GameObject vfxToSpawn = SpawnVFXMaster(vfx, spawnPos);
        vfxToSpawn.transform.localScale *= scaleMod;
    }

    /// <summary>
    /// Spawns a VFX prefab and applies a scale modifier randomly chosen between two values
    /// </summary>
    /// <param name="vfx">VFX prefab to spawn</param>
    /// <param name="spawnPos">VFX spawn transform</param>
    /// <param name="scaleModMin">Scale multiplier minimum value</param>
    /// <param name="scaleModMax">Scale multiplier maximum value</param>
    /// <returns></returns>
    public void SpawnVFXScaleRandom(GameObject vfx, Transform spawnPos,
                float scaleModMin, float scaleModMax)
    {
        float scaleMod = Random.Range(scaleModMin, scaleModMax);
        SpawnVFXScale(vfx, spawnPos, scaleMod);
    }

    #endregion

    #region public methods timed

    /// <summary>
    /// Spawns a VFX prefab
    /// </summary>
    /// <param name="vfx">VFX prefab to spawn</param>
    /// <param name="spawnPos">VFX spawn transform</param>
    /// <param name="lifetime">Lifetime of VFX, delay before destroying</param>
    public void SpawnVFX(GameObject vfx, Transform spawnPos, float lifetime)
    {
        SpawnVFXMasterTimed(vfx, spawnPos,lifetime);
    }

    /// <summary>
    /// Spawns a VFX prefab and applies a scale modifier
    /// </summary>
    /// <param name="vfx">VFX prefab to spawn</param>
    /// <param name="spawnPos">VFX spawn transform</param>
    /// <param name="scaleMod">Scale multipler</param>
    /// <param name="lifetime">Lifetime of VFX, delay before destroying</param>
    public void SpawnVFXScale(GameObject vfx, Transform spawnPos, float scaleMod, float lifetime)
    {
        GameObject vfxToSpawn = SpawnVFXMasterTimed(vfx, spawnPos, lifetime);
        vfxToSpawn.transform.localScale *= scaleMod;
    }

    /// <summary>
    /// Spawns a VFX prefab and applies a scale modifier randomly chosen between two values
    /// </summary>
    /// <param name="vfx">VFX prefab to spawn</param>
    /// <param name="spawnPos">VFX spawn transform</param>
    /// <param name="scaleModMin">Scale multiplier minimum value</param>
    /// <param name="scaleModMax">Scale multiplier maximum value</param>
    /// <param name="lifetime">Lifetime of VFX, delay before destroying</param>
    public void SpawnVFXScaleRandom(GameObject vfx, Transform spawnPos,
                float scaleModMin, float scaleModMax, float lifetime)
    {
        float scaleMod = Random.Range(scaleModMin, scaleModMax);
        SpawnVFXScale(vfx, spawnPos, scaleMod, lifetime);
    }
    #endregion
}
