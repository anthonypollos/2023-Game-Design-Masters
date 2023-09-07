using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoPlayerController : MonoBehaviour, IKickable, IDamageable
{
    [SerializeField] [Tooltip("The rigidbody used for movement")] private Rigidbody _rb;
    [SerializeField] [Tooltip("The player's movement speed")] private float _speed = 5;
    [SerializeField][Tooltip("The player's turn speed")] private float _turnSpeed = 360;
    private Vector3 _input;
    private Camera cam;
    MainControls mc;
    [SerializeField] LayerMask groundMask;
    bool stunned;
    float stundelay = .2f;
    bool canUnstun;
    private void Start()
    {
        cam = Camera.main;
        stunned = false;
        canUnstun = false;
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
        Look();
    }

    private void FixedUpdate()
    {
        if(!stunned)
            Move();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            
            if (stunned && canUnstun)
            {
                stunned = false;
            }
        }

    }

    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

  // The character rotates to move in the direction of the player's input
    private void Look()
    {
        //if (_input == Vector3.zero) return;

        //var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);

        var (success, position) = GetMousePosition();
        if(success)
        {
            var direction = position - transform.position;

            direction.y = 0;
            transform.forward = direction;
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
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

        _rb.velocity = _input.ToIso().normalized * _speed;
    }

    public void Kicked()
    {
        Stunned();
    }


    private void Stunned()
    {
        Debug.Log("stunned");
        canUnstun = false;
        stunned = true;
        StartCoroutine(UnStunDelay());

    }

    IEnumerator UnStunDelay()
    {
        yield return new WaitForSeconds(stundelay);
        canUnstun = true;
    }

    public void TakeDamage(int dmg)
    {
        return;    
    }


}

// Automatically adjusts the player's movement to match the camera's rotation
public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 35, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
