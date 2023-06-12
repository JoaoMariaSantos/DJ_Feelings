using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Noise;
using Instructions;

namespace Player
{
    public class Dashing : MonoBehaviour
    {
        [Header("References")]
        private Rigidbody rb;
        public NoiseManager noiseManager;
        private PlayerMovement playerMovement;
        PlayerControls controls;
        public AudioManager audioManager;

        [Header("Dashing")]
        private bool dashUnlocked = false;
        public float dashForce;
        public float referenceDashForce;
        public float dashUpwardForce;
        public float dashDuration;
        public bool dashReady;

        [Header("Reference")]
        public float dashCd;
        private float dashCdTimer;
        public InstructionsManager instructionsManager;

        [Header("Input")]
        public KeyCode dashKey = KeyCode.Space;

        private void Awake(){
            controls = new PlayerControls();
            controls.GamePlay.Dash.performed += ctx => Dash();
        }
        private void Start()
        {
            referenceDashForce = dashForce;
            rb = GetComponent<Rigidbody>();
            playerMovement = GetComponent<PlayerMovement>();            
        }

        private void Update()
        {
            if(playerMovement.grounded) dashReady = true;
            //if (Input.GetKeyDown(dashKey)) Dash();
        }

        private void Dash()
        {
            if (!dashReady || !dashUnlocked || playerMovement.grounded) return;

            dashForce = (noiseManager.GetArousalEdited() + 2f) * referenceDashForce;

            dashReady = false;

            playerMovement.dashing = true;

            Vector3 forceToApply = playerMovement.getMovementDirection() * dashForce + Vector3.up * dashUpwardForce;

            delayedForceToApply = forceToApply;
            Invoke(nameof(DelayedDashForce), 0.025f);

            float arousal = (noiseManager.GetArousalEdited() + 1) / 2 + 0.1f;
            audioManager.ChangeVolume("Dash", arousal);
            audioManager.PlaySound("Dash");
        }

        private Vector3 delayedForceToApply;

        private void DelayedDashForce()
        {
            rb.AddForce(delayedForceToApply, ForceMode.Impulse);

        }
        private void ResetDash()
        {
            playerMovement.dashing = false;
        }

        public void unlockDash(){
            dashUnlocked = true;
            instructionsManager.ShowInstruction("Dash");
        }
        private void OnEnable() {
            controls.GamePlay.Enable();
        }
    }
}
