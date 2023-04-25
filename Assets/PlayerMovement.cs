using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public GameObject playerObj;
    public float rotationSpeed;
    private float defaultMoveSpeed;
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    float horizontalInput;
    float verticalInput;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    //public Transform orientation;

    Vector3 moveDirection;

    Rigidbody rb;
    private void Awake()
    {
        defaultMoveSpeed = moveSpeed;
        rb = playerObj.GetComponent<Rigidbody>();
        //rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector2.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        if (grounded)
            rb.drag = groundDrag;
        else rb.drag = 0;

    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(KeyCode.LeftShift)){
            Debug.Log("shit key down");
            moveSpeed = defaultMoveSpeed * 3f;
        } 
        else moveSpeed = defaultMoveSpeed;

        Debug.Log(moveSpeed);

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = Vector3.forward * verticalInput + Vector3.right * horizontalInput;
        //moveDirection = oritentation.forward * verticalInput + oritentation.right * horizontalInput;

        //Vector3 rotationAxis = moveDirection + Vector3.right;
        Vector3 rotationAxis = Quaternion.Euler(0, 90, 0) * moveDirection;
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float flatVelMagniturde = flatVel.magnitude;

        //playerObj.transform.Rotate(rotationAxis * flatVelMagniturde * rotationSpeed * Time.deltaTime, Space.Self); //TODO fix

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else{
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            //float diff = flatVel.magnitude - moveSpeed;
            //float thisVelocity = flatVel.magnitude + diff * 0.05f;
            //Vector3 limitedVel = flatVel.normalized * thisVelocity;

            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
