using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Noise;
using Instructions;
using Credits;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public GameObject playerObj;
        private float defaultMoveSpeed;
        public float moveSpeed;
        private Vector2 movementInput;
        private Vector2 movementVector;
        public float desiredMoveSpeed;
        public float lastDesiredMoveSpeed;
        public float speedMultiplier;
        public float dashSpeed;
        public bool dashing;
        private bool sprinting;
        public bool jumping;
        public float groundDrag;

        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier;
        bool readyToJump = true;
        bool keepMomentum;

        public float pushSidewaysStrength;

        [Header("Noise")]

        public NoiseManager noiseManager;

        [Header("Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;

        float horizontalInput;
        float verticalInput;

        [Header("Ground Check")]
        public float playerHeight;
        public LayerMask whatIsGround;
        public bool grounded;

        [Header("References")]
        public AudioManager audioManager;
        public InstructionsManager instructionsManager;
        PlayerControls controls;

        //public Transform orientation;

        Vector3 moveDirection;

        Rigidbody rb;

        public MovementState state;
        public MovementState lastState;
        public enum MovementState
        {
            walking,
            sprinting,
            dashing,
            air
        }
        private void Awake()
        {
            controls = new PlayerControls();

            controls.GamePlay.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
            controls.GamePlay.Move.canceled += ctx => movementInput = Vector2.zero;
            controls.GamePlay.Jump.started += ctx => Jump();
            controls.GamePlay.Jump.canceled += ctx => ResetJump();
            controls.GamePlay.Sprint.started += ctx => Sprint(true);
            controls.GamePlay.Sprint.canceled += ctx => Sprint(false);

            defaultMoveSpeed = moveSpeed;
            rb = playerObj.GetComponent<Rigidbody>();
            //rb.freezeRotation = true;
        }

        private void Update()
        {
            grounded = Physics.Raycast(transform.position, Vector2.down, playerHeight * 0.5f + 0.2f, whatIsGround);

            MyInput();
            StateHandler();
            SpeedControl();


            if (grounded){
                rb.drag = groundDrag;
                dashing = false;
                jumping = false;
            }
            else {
                state = MovementState.air;
                rb.drag = 0;
            }

        }
        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void StateHandler(){
            float arousal = (noiseManager.GetArousalEdited() + 1f) * 0.35f + 0.2f;

            if(dashing){
                state = MovementState.dashing;
                desiredMoveSpeed = defaultMoveSpeed * speedMultiplier * arousal * dashSpeed;
                instructionsManager.InstructionDone("Dash");
            } 
            
            //Sprinting
            else if(sprinting && grounded){
                state = MovementState.sprinting;
                desiredMoveSpeed = defaultMoveSpeed * speedMultiplier * arousal + 0.5f;
                instructionsManager.InstructionDone("Sprint");
            } 
            
            //Walking
            else if(grounded){
                state = MovementState.walking;
                desiredMoveSpeed = defaultMoveSpeed * arousal;
            }

            //Jumping
            /* if(Input.GetKeyDown(jumpKey) && grounded){
                state = MovementState.air;
                //Jump();
            } */

            bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
            if(lastState == MovementState.dashing || lastState == MovementState.sprinting) keepMomentum = true;

            if(lastState == MovementState.air && state != lastState && lastState != MovementState.dashing){
                float volume = rb.velocity.magnitude * 0.002f;   
                Debug.Log(volume);
                volume = Mathf.Clamp(volume, 0.01f,0.3f);
                audioManager.ChangeVolume("Fall", volume);
                audioManager.PlaySound("Fall");
            } 

            if(desiredMoveSpeedHasChanged){
                if(keepMomentum){
                    moveSpeed += (desiredMoveSpeed - moveSpeed) * 0.02f;
                }
                else {
                    moveSpeed = desiredMoveSpeed;
                }
            }

            lastDesiredMoveSpeed = desiredMoveSpeed;
            lastState = state;
        }

        private void MyInput()
        {
            if(movementInput != Vector2.zero){
                movementVector = movementInput;
                return;
            } 

            movementVector.x = Input.GetAxisRaw("Horizontal");
            movementVector.y = Input.GetAxisRaw("Vertical");

            return;
        }

        private void MovePlayer()
        {
            if(movementVector.x == 0 && movementVector.y == 0) return;

            instructionsManager.InstructionDone("Movement");

            moveDirection = Vector3.forward * movementVector.y + Vector3.right * movementVector.x;

            Vector3 rotationAxis = Quaternion.Euler(0, 90, 0) * moveDirection;
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            float flatVelMagniturde = flatVel.magnitude;

            if (grounded)
            {
                float currentPushSidewaysAngle = pushSidewaysStrength * moveSpeed * (Mathf.PerlinNoise(Time.time * 0.6f, Time.time * 0.6f) * 2 - 1) * (noiseManager.GetArousalEdited() + 1);
                moveDirection = Quaternion.AngleAxis(currentPushSidewaysAngle, Vector3.up) * moveDirection;
                rb.AddForce(moveDirection.normalized * moveSpeed * 9f, ForceMode.Force);
            }
            else
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 9f * airMultiplier, ForceMode.Force);
            }
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                float diff = moveSpeed - flatVel.magnitude;
                float thisVelocity = flatVel.magnitude + diff * 0.01f;
                Vector3 limitedVel = flatVel.normalized * thisVelocity;

                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        private void Jump()
        {
            if(jumping || !readyToJump || !grounded) return;
            state = MovementState.air;
            readyToJump = false;
            jumping = true;

            float arousal = (noiseManager.GetArousalRaw() + 1) / 1.5f + 0.2f;
            float currentJumpForce = jumpForce * arousal + 1.5f;
            rb.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
            audioManager.ChangeVolume("Jump", arousal / 3);
            audioManager.ChangePitch("Jump", arousal - 0.1f);
            audioManager.PlaySound("Jump");
            instructionsManager.InstructionDone("Jump");
        }

        private void ResetJump(){
            readyToJump = true;
        }

        private void Sprint(bool state){
            if(state && !grounded) return;
            sprinting = state;
        }

        public GameObject GetCubeStandingOn()
        {
            Ray downRay = new Ray(transform.position, -Vector3.up);
            RaycastHit toGround;

            bool onGround = Physics.Raycast(downRay, out toGround, Mathf.Infinity, whatIsGround);

            GameObject hit = toGround.collider.gameObject;

            if(hit.name.Contains("ube")) return hit;
            
            return null;
        }

        public Vector2 GetPosFlat()
        {
            return new Vector2(rb.position.x, rb.position.z);
        }

        public float GetVelocity()
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            return flatVel.magnitude;
        }

        public Vector3 getMovementDirection(){
            return moveDirection;
        }

        private void OnEnable() {
            controls.GamePlay.Enable();
        }
}
}