using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoPlayerController : MonoBehaviour
{
    [SerializeField] [Tooltip("The rigidbody used for movement")] private Rigidbody _rb;
    [SerializeField] [Tooltip("The player's movement speed")] private float _speed = 5;
    [SerializeField][Tooltip("The player's turn speed")] private float _turnSpeed = 360;
    private Vector3 _input;

    private void Update()
    {
        GatherInput();
        Look();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

  // The character rotates to move in the direction of the player's input
    private void Look()
    {
        if (_input == Vector3.zero) return;

        var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    }
    
    private void Move()
    {
        _rb.MovePosition(transform.position + transform.forward * _input.normalized.magnitude * _speed * Time.deltaTime);
    }
}

// Automatically adjusts the player's movement to match the camera's rotation
public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 35, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
