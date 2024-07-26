using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IsoCamera : MonoBehaviour
{
    private Transform player;
    public float _smoothing = 5f;
    Vector3 _offset;

    List<Transparency> currentFade;

    //This will ALWAYS be the player
    private Transform playerStatic;

    // Start is called before the first frame update
    void Start()
    {
        playerStatic = GameObject.FindGameObjectWithTag("Player").transform;
        player = playerStatic;
        currentFade = new List<Transparency>();
        _offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        Vector3 targetCamPos = player.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, _smoothing * Time.deltaTime);
    }

    //Get the offset from the player
    public Vector3 GetOffset()
    {
        return _offset;
    }

    /// <summary>
    /// Change the camera target to newTarget
    /// </summary>
    /// <param name="newTarget"></param>
    public void ChangeTarget(GameObject newTarget)
    {
        player = newTarget.transform;
    }

    /// <summary>
    /// Revert the camera target back to the player
    /// </summary>
    public void RevertTarget()
    {
        player = playerStatic;
    }

    /// <summary>
    /// Tell if the player is the current camera target
    /// </summary>
    /// <returns></returns>
    public bool isTargetPlayer()
    {
        return (player == playerStatic);
    }
}
