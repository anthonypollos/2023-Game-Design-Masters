using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using FMODUnity;


public enum GroundTypes
{
    DEFAULT, DIRT, STONE, WOOD, METAL
}
public class IsoPlayerController : MonoBehaviour, IKickable, ISlowable
{

    [SerializeField] [Tooltip("The rigidbody used for movement")] private Rigidbody _rb;
    [SerializeField] [Tooltip("The player's movement speed")] private float _speed = 5;
    [SerializeField] float groundedRayLength = 1.1f;
    [SerializeField] LayerMask groundedMask;
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
    public bool isStunned;
    private GameController gc;
    private int previousLayer;
    [SerializeField] GameObject lasso;

    [SerializeField] float dashRange, dashTime, dashCD;
    //[SerializeField] Image dashCDIndicator;
    //[SerializeField] private JukeBox jukebox;

    [SerializeField] float speedModWhenLassoOut;
    [SerializeField] float speedModWhenPulling;
    IsoAttackManager attackManager;
    Flammable flammable;

    [Header("Animator Variables")]
    [SerializeField] Animator anim; //assigned in inspector for now; can change

    List<float> slowMods;
    float[] slowModsArray;

    [Header("Sound Variables")]
    [SerializeField] GroundTypes currentGroundType = GroundTypes.DEFAULT;
    [SerializeField] private EventReference footsteps;
    [SerializeField] private EventReference footstepsDirt;
    [SerializeField] private EventReference footstepsWood;
    [SerializeField] private EventReference footstepsMetal;
    [SerializeField] private EventReference footstepsStone;
    [SerializeField] private EventReference dashing;
    [SerializeField] private EventReference dashing2;
    [SerializeField] private EventReference dashing3;

    Vector3 savedMousePos;

    private void Awake()
    {
        //jukebox.SetTransform(transform);
    }
    private void Start()
    {
        EnterSlowArea(0);
        isStunned = false;
        attackManager = GetComponent<IsoAttackManager>();
        flammable = GetComponent<Flammable>();
        moveable = GetComponent<Moveable>();
        attackState = 0;
        isDead = false;
        cam = Camera.main;
        Helpers.UpdateMatrix();
        canDash = true;
        gc = FindObjectOfType<GameController>();
        DeveloperConsole.instance.SetPlayer(gameObject, _rb);
    }

    private void OnEnable()
    {
        mc = ControlsContainer.instance.mainControls;
        mc.Main.Move.performed += OnMove;
        mc.Main.Move.canceled += OnMove;
        mc.Main.Aim.performed += OnAim;
        mc.Main.Aim.canceled += OnAim;
        mc.Main.Dash.performed += OnDash;
    }

    private void OnDisable()
    {
        mc.Main.Move.performed -= OnMove;
        mc.Main.Move.canceled -= OnMove;
        mc.Main.Aim.performed -= OnAim;
        mc.Main.Aim.canceled -= OnAim;
        mc.Main.Dash.performed -= OnDash;
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            Dash();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            _input = new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
        }
        if(ctx.canceled)
        {
            _input = Vector3.zero;
        }
    }

    private void OnAim(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            _aimInput = new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
        }
        if(ctx.canceled)
        {
            _aimInput = Vector3.zero;
        }
    }

    private void Update()
    {
        if (Time.timeScale != 0 && !isStunned)
        {
            if (!isDead && !moveable.isLaunched && attackState != Helpers.ATTACKING)
                Look();

            if (gameObject.layer == LayerMask.NameToLayer("PlayerDashing") && !moveable.isLaunched)
            {
                //Debug.Log("PlayerDash revert");
                gameObject.layer = previousLayer;
            }
        }
        if(isStunned && !moveable.isLaunched)
        {
            isStunned = false;
        }
    }

    private void FixedUpdate()
    {
        if (Time.timeScale != 0 && !isStunned)
        {
            if (!moveable.isLaunched && !isDead && attackState != Helpers.ATTACKING)
            {
                Look();
                Move();
            }
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
            LookAtMouse();
        }
        else if (_input != Vector3.zero && attackState==Helpers.NOTATTACKING)
        {
            transform.forward = _input.ToIso().normalized;
        }
        else if (attackState == Helpers.PULLING || attackState == Helpers.LASSOED)
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

    public void LookAtMouse(bool bypass = false)
    {
        if (!bypass && attackState == Helpers.LASSOING)
        {
            var direction = savedMousePos - transform.position;
            transform.forward = direction.normalized;
            return;
        }
        var (success, position) = GetMousePosition();
        if (success)
        {
            savedMousePos = position;
            var direction = position - transform.position;

            direction.y = 0;
            transform.forward = direction.normalized;
        }
    }

    private void Dash()
    {
        if(canDash && !moveable.isLaunched && !isDead && Time.timeScale != 0)
        {
            //Debug.Log("Transform.forward: " + transform.forward);
            if (flammable.isBurning)
            {
                flammable.StopDropAndRoll();
            }
            if (attackState == Helpers.LASSOING || attackState == Helpers.LASSOED || attackState == Helpers.PULLING)
            {
                attackManager.ForceRelease();
                gameObject.layer = LayerMask.NameToLayer("PlayerDashing");
                canDash = false;
                //jukebox.PlaySound(0);
                //PickDashSound(Random.Range(1, 4));
                if (_input == Vector3.zero)
                    moveable.Dash(transform.forward * dashRange, dashTime);
                else
                {
                    Debug.Log(_input.ToIso());
                    moveable.Dash(_input.ToIso().normalized * dashRange, dashTime);
                }
                anim.SetFloat("DashSpeed", 32f / (24 * dashTime));
                anim.SetTrigger("Dash");
                StartCoroutine(DashCD());
            }
            else if (attackState == Helpers.NOTATTACKING || _input == Vector3.zero)
            {
                if (gameObject.layer != LayerMask.NameToLayer("PlayerDashing"))
                    previousLayer = gameObject.layer;
                gameObject.layer = LayerMask.NameToLayer("PlayerDashing");
                canDash = false;
                //jukebox.PlaySound(0);
                PickDashSound(Random.Range(1, 4));
                moveable.Dash(transform.forward * dashRange, dashTime);
                anim.SetFloat("DashSpeed", 32f / (24 * dashTime));
                anim.SetTrigger("Dash");
                StartCoroutine(DashCD());
            }
        }
    }
    
    private IEnumerator DashCD()
    {
        
        for (float i =0; i<dashCD; i+=0.01f)
        {
            yield return new WaitForSeconds(0.01f);
            //dashCDIndicator.fillAmount = i / dashCD;
        }
        canDash = true;
        //dashCDIndicator.fillAmount = 1;
    }
    

    public (bool success, Vector3 position) GetMousePosition()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            //Debug.DrawRay(hitInfo.point, Vector3.down, Color.red);
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
        adjustedSpeed *= (1 - Mathf.Max(slowModsArray));
        if (attackState == Helpers.LASSOING || attackState == Helpers.LASSOED)
            adjustedSpeed *= speedModWhenLassoOut;
        if (attackState == Helpers.PULLING)
            adjustedSpeed *= speedModWhenPulling;

        Vector3 temp = _rb.velocity;
        temp.y = 0;
        //Debug.Log("Velocity magnitude: " + temp.magnitude);

        // Set forward/back movement float; will have to change
        if (temp.magnitude > 1)
            anim.SetFloat("YMov", 1);
        else
            anim.SetFloat("YMov", 0);
        Vector3 yVel = Vector3.up * Mathf.Clamp(_rb.velocity.y, Mathf.NegativeInfinity, 0);
        _rb.velocity = Vector3.zero;
        _rb.AddForce(_input.ToIso() * adjustedSpeed, ForceMode.VelocityChange);
        _rb.velocity += yVel;
        
        //if grounded & _input == Vector3.zero set velocity.y to 0
        if(_input == Vector3.zero && IsGrounded())
        {
            //Debug.Log("Is grounded: " + IsGrounded());
            _rb.useGravity = false;
            _rb.velocity = Vector3.zero;
        }
        else
        {
            _rb.useGravity = true;
        }

        

    }

    private bool IsGrounded()
    {
        ExtDebug.DrawBoxCastBox(transform.position, new Vector3(0.5f, 0f, 0.5f), Quaternion.identity, -transform.up, groundedRayLength, Color.red);
        bool temp =  Physics.BoxCast(transform.position, new Vector3(0.5f, 0f, 0.5f), -transform.up, Quaternion.identity, groundedRayLength, groundedMask);
        //Debug.Log("Is grounded: " + temp);
        return temp;
    }

    public void EnterSlowArea(float slowPercent)
    {
        if (slowMods == null)
        {
            slowMods = new List<float>();
        }
        slowMods.Add(slowPercent);
        slowModsArray = slowMods.ToArray();
    }
    public void ExitSlowArea(float slowPercent)
    {
        if (slowMods != null)
        {
            if (slowMods.Contains(slowPercent))
                slowMods.Remove(slowPercent);
            slowModsArray = slowMods.ToArray();
        }
    }

    public void Kicked()
    {
        isStunned = true;
        attackState = Helpers.NOTATTACKING;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GroundTypeContainer gtc = collision.gameObject.GetComponent<GroundTypeContainer>();
            if (gtc != null)
                currentGroundType = gtc.GetGroundType();
            else
                currentGroundType = GroundTypes.DEFAULT;
        }
    }

    public void PickDashSound(int selection)
    {
        print(selection);
        switch (selection)
        {
            case 3:
                AudioManager.instance.PlayOneShot(dashing3, this.transform.position);
                break;
            case 2:
                AudioManager.instance.PlayOneShot(dashing2, this.transform.position);
                break;
            default:
                AudioManager.instance.PlayOneShot(dashing, this.transform.position);
                break;
        }
    }
        public void Footsteps()
    {
        //jukebox.PlaySound(1);
        switch(currentGroundType)
        {
            case GroundTypes.DEFAULT:
                AudioManager.instance.PlayOneShot(footsteps, this.transform.position);
                break;
            case GroundTypes.DIRT:
                AudioManager.instance.PlayOneShot(footstepsDirt, this.transform.position);
                break;
            case GroundTypes.METAL:
                AudioManager.instance.PlayOneShot(footstepsMetal, this.transform.position);
                break;
            case GroundTypes.STONE:
                AudioManager.instance.PlayOneShot(footstepsStone, this.transform.position);
                break;
            case GroundTypes.WOOD:
                AudioManager.instance.PlayOneShot(footstepsWood, this.transform.position);
                break;


        }
    }

    public void ChangeSpeed(float speed)
    {
        _speed = speed;
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
    public const int THROWN = 5;
    
}
