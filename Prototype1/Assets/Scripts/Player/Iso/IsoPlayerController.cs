using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoPlayerController : MonoBehaviour, IKickable
{
    [SerializeField] [Tooltip("The rigidbody used for movement")] private Rigidbody _rb;
    [SerializeField] [Tooltip("The player's movement speed")] private float _speed = 5;
    //[SerializeField][Tooltip("The player's turn speed")] private float _turnSpeed = 360;
    private Vector3 _input;
    private Camera cam;
    MainControls mc;
    [SerializeField] LayerMask groundMask;
    bool canUnstun;
    public Moveable moveable;
    [HideInInspector]
    public int attackState;
    [HideInInspector]
    public bool isDead;
    private void Start()
    {
        moveable = GetComponent<Moveable>();
        attackState = 0;
        isDead = false;
        cam = Camera.main;
        Helpers.UpdateMatrix();
    }

    private void Awake()
    {
        mc = new MainControls();
        mc.Enable();
        mc.Main.Move.performed += ctx => _input = new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
        mc.Main.Move.canceled += _ => _input = Vector3.zero;
    }

    private void Update()
    {
        if(!isDead && !moveable.isLaunched)
            Look();
    }

    private void FixedUpdate()
    {
        if (!moveable.isLaunched && !isDead && attackState == Helpers.NOTATTACKING)
            Move();
        else
            if(!moveable.isLaunched)
                _rb.velocity = Vector3.zero;
    }



  // The character rotates to move in the direction of the player's input
    private void Look()
    {
        //if (_input == Vector3.zero) return;

        //var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);

        if (attackState==Helpers.CHARGING)
        {
            lookAtMouse();
        }
        else if (_input != Vector3.zero && attackState==Helpers.NOTATTACKING)
        {
            transform.forward = _input.ToIso().normalized;
        }
    }

    public void lookAtMouse()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            var direction = position - transform.position;

            direction.y = 0;
            transform.forward = direction;
        }
    }

    public (bool success, Vector3 position) GetMousePosition()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            return (success: true, position: hitInfo.point);

        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }
    
    private void Move()
    {

        _rb.velocity = _input.ToIso().normalized * _speed + (Vector3.up * _rb.velocity.y);
    }

    public void Kicked()
    {

    }





}

// Automatically adjusts the player's movement to match the camera's rotation
public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);

    public static void UpdateMatrix()
    {
        _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0));
    }

    public const int NOTATTACKING = 0;
    public const int CHARGING = 1;
    public const int KICKING = 2;
    
}
