using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable, IKickable
{
    [SerializeField] [Tooltip("Movement speed of the player")] float movespeed;
    [SerializeField] [Tooltip("Vertical speed of your jump")] float jumpHeight;
    [SerializeField] [Tooltip("Mouse sensitivity")] float mouseSens;
    //[SerializeField] bool isFirstPerson = true;
    bool canJump;
    //bool crouched;
    Rigidbody rb;
    GameObject cam;
    MainControls mc;
    bool stunned;
    float stundelay = .2f;
    bool canUnstun;
    

    Vector3 movement;
    Vector2 mouseInput;

    private void Awake()
    {
        mc = new MainControls();
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Start is called before the first frame update
    void Start()
    {
        canUnstun = false;
        stunned = false;
        canJump = true;
        //crouched = false;
        rb = GetComponent<Rigidbody>();
        cam = transform.GetChild(0).gameObject;
        cam.transform.localRotation = Quaternion.identity;
    }

    private void OnEnable()
    {
        mc.Enable();
        mc.Main.Move.performed += ctx => movement = new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
        mc.Main.Move.canceled += _ => movement = Vector3.zero;
        mc.Main.Jump.performed += _ => Jump();
        mc.Main.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        mc.Main.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
    }

    private void OnDisable()
    {
        mc.Disable();
    }

    private void Update()
    {
        RotateCamera();
    }
    void FixedUpdate()
    {
        if (!stunned)
        {
            Move(movement);
        }
    }

    private void Move(Vector3 temp)
    {
        temp = Vector3.ClampMagnitude(temp, 1);
        rb.velocity = (temp.x * transform.right + temp.z * transform.forward) * movespeed + new Vector3(0,rb.velocity.y, 0);
    }

    private void Jump()
    {
        if(canJump && !stunned)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
            canJump = false;
        }
    }

    private void Crouch()
    {
        Debug.Log("Crouching");
    }

    private void RotateCamera()
    {
        transform.Rotate(Vector3.up, mouseInput.x * mouseSens * Time.deltaTime);
        cam.transform.Rotate(Vector3.right, mouseInput.y * -mouseSens * Time.deltaTime);
        float x = cam.transform.rotation.eulerAngles.x;
        if(x<=180)
        {
            cam.transform.localRotation = Quaternion.Euler(Mathf.Clamp(x, 0, 90f), 0, 0);
        }
        else
            cam.transform.localRotation = Quaternion.Euler(Mathf.Clamp(cam.transform.rotation.eulerAngles.x, 270f, 360f), 0, 0);
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            canJump = true; 
            if(stunned)
            {
            stunned = false;
            }
        }
       
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            canJump = false;
        }
    }

    public Vector3 GetMovement()
    {
        return movement;
    }

    public void Kicked()
    {
        Stunned();
    }


    private void Stunned()
    {
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

    }

}

