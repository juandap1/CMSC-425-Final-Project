using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform cam;
    public LayerMask whatIsGround;

    [Header("Movement Vars")]
    public float moveSpeed = 7f;
    public float sprintSpeed = 10f;
    public float groundDrag = 5f;
    public float playerHeight = 2f;
    public float jumpForce = 3f;
    public float jumpCooldown = 1f;

    [Header("Settings & Keybinds")]
    public float sens = 90f;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Allowed Movement")]
    public bool jump = true;
    public bool walk = true;

    float xRotation;
    float yRotation;
    float horizontalInput;
    float verticalInput;
    bool grounded;
    bool readyToJump = true;
    bool sprinting;

    Vector3 moveDirection;

    Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (walk || jump)
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        //Movement
        if (walk ||jump)
        {
            MovementInput();
            SpeedControl();
            if (grounded)
                rb.drag = groundDrag;
            else
                rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovementInput()
    {
        if (walk)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            sprinting = Input.GetKey(sprintKey);
        }

        if (Input.GetKey(jumpKey) && readyToJump && grounded && jump)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

    }

    void MovePlayer()
    {
        float speed = sprinting ? sprintSpeed : moveSpeed;
        moveDirection = cam.forward * verticalInput + cam.right * horizontalInput;
        if (grounded)
            rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
    }

    void SpeedControl()
    {
        float speedCap = sprinting ? sprintSpeed : moveSpeed;
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > speedCap)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
    }
}
