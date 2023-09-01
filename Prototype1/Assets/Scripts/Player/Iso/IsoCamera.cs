using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoCamera : MonoBehaviour
{
    public Transform _player;
    public float _smoothing = 5f;
    Vector3 _offset;
    
    // Start is called before the first frame update
    void Start()
    {
        _offset = transform.position - _player.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetCamPos = _player.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, _smoothing * Time.deltaTime);
    }
}
