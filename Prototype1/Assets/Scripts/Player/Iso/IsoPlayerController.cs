using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsoPlayerController : MonoBehaviour, IKickable
{
    [SerializeField] [Tooltip("The rigidbody used for movement")] private Rigidbody _rb;
    [SerializeField] [Tooltip("The player's movement speed")] private float _speed = 5;
    //[SerializeField][Tooltip("The player's turn speed")] private float _turnSpeed = 360;
    private Vector3 _input;
    public Vector3 _aimInput;
    private Camera cam;
    MainControls mc;
    [SerializeField] LayerMask groundMask;
    bool canUnstun;
    public Moveable moveable;
    [HideInInspector]
    public int attackState;
    [HideInInspector]
    public bool isDead;
    private bool canDash;
    private GameController gc;
    [SerializeField] GameObject lasso;

    [SerializeField] float dashRange, dashTime, dashCD;
    [SerializeField] Image dashCDIndicator;
    [SerializeField] private JukeBox jukebox;

    [SerializeField] float speedModWhenLassoOut;
    [SerializeField] float speedModWhenPulling;
    IsoAttackManager attackManager;

    [Header("Animator Variables")]
    [SerializeField] Animator anim; //assigned in inspector for now; can change
    [SerializeField] float kickSpeed, dashSpeed, tendrilThrowSpeed;

    private void Awake()
    {
        jukebox.SetTransform(transform);
    }
    private void Start()
    {
        attackManager = GetComponent<IsoAttackManager>();
        moveable = GetComponent<Moveable>();
        attackState = 0;
        isDead = false;
        cam = Camera.main;
        Helpers.UpdateMatrix();
        canDash = true;
        gc = FindObjectOfType<GameController>();
    }

    private void OnEnable()
    {
        mc = new MainControls();
        mc.Enable();
        mc.Main.Move.performed += ctx => _input = new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
        mc.Main.Move.canceled += _ => _input = Vector3.zero;
        mc.Main.Aim.performed += ctx => _aimInput = new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue <Vector2>().y);
        mc.Main.Dash.performed += _ => Dash();
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            if (!isDead && !moveable.isLaunched)
                Look();

            if (gameObject.layer == LayerMask.NameToLayer("PlayerDashing") && !moveable.isLaunched)
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
    }

    private void FixedUpdate()
    {
        if (Time.timeScale != 0)
        {
            if (!moveable.isLaunched && !isDead && attackState != Helpers.ATTACKING)
                Move();
            else
                if (!moveable.isLaunched)
                _rb.velocity = Vector3.zero + Vector3.up * _rb.velocity.y;
        }
    }



  // The character rotates to move in the direction of the player's input
    private void Look()
    {
        //if (_input == Vector3.zero) return;

        //var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);

        if (attackState==Helpers.LASSOING)
        {
            if (InputChecker.instance.IsController())
                LookAtAim();
            else
                LookAtMouse();

        }
        else if (_input != Vector3.zero && attackState==Helpers.NOTATTACKING)
        {
            transform.forward = _input.ToIso().normalized;
        }
        else if (attackState == Helpers.LASSOED)
        {
            LookAtLasso();
        }
    }

    public void LookAtLasso()
    {
        Vector3 dir = lasso.transform.position - transform.position;
        dir.y = 0;
        transform.forward = dir.normalized;
    }

    public void LookAtAim()
    {
        transform.forward = Helpers.ToIso(_aimInput.normalized);
    }

    public void LookAtMouse()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            var direction = position - transform.position;

            direction.y = 0;
            transform.forward = direction.normalized;
        }
    }

    private void Dash()
    {
        if(canDash && !moveable.isLaunched && !isDead && Time.timeScale != 0)
        {
            if( attackState == Helpers.NOTATTACKING || _input == Vector3.zero)
            {   
                gameObject.layer = LayerMask.NameToLayer("PlayerDashing");
                canDash = false;
                jukebox.PlaySound(0);
                moveable.Dash(transform.forward * dashRange, dashTime);
                anim.SetTrigger("dash");
                anim.SetTrigger("tendrilRelease");
                StartCoroutine(DashCD());
            }
            else if (attackState == Helpers.LASSOING || attackState == Helpers.LASSOED || attackState == Helpers.PULLING)
            {
                attackManager.ForceRelease();
                gameObject.layer = LayerMask.NameToLayer("PlayerDashing");
                canDash = false;
                jukebox.PlaySound(0);
                moveable.Dash(_input.ToIso().normalized * dashRange, dashTime);
                StartCoroutine(DashCD());
            }
        }
    }

    private IEnumerator DashCD()
    {
        
        for (float i =0; i<dashCD; i+=0.01f)
        {
            yield return new WaitForSeconds(0.01f);
            dashCDIndicator.fillAmount = i / dashCD;
        }
        canDash = true;
        dashCDIndicator.fillAmount = 1;
    }


    public (bool success, Vector3 position) GetMousePosition()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            Debug.DrawRay(hitInfo.point, Vector3.down, Color.red);
            return (success: true, position: hitInfo.point);

        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }
    
    private void Move()
    {
        float adjustedSpeed = _speed;
        if (attackState == Helpers.LASSOING || attackState == Helpers.LASSOED)
            adjustedSpeed *= speedModWhenLassoOut;
        if (attackState == Helpers.PULLING)
            adjustedSpeed *= speedModWhenPulling;
        _rb.velocity = _input.ToIso().normalized * adjustedSpeed + (Vector3.up * Mathf.Clamp(_rb.velocity.y, Mathf.NegativeInfinity, 0));

        // Set forward/back movement float; will have to change
        if (_rb.velocity.magnitude > 0.1)
            anim.SetFloat("yMov", 1);
        else
            anim.SetFloat("yMov", 0);

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
    public const int LASSOING = 1;
    public const int ATTACKING = 2;
    public const int LASSOED = 3;
    public const int PULLING = 4;
    
}
