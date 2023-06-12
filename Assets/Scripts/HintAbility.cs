using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Artifact;
using Instructions;

namespace Player{
public class HintAbility : MonoBehaviour
{
    [Header("References")]
    public ArtifactManager artifactManager;
    public ParticleSystem particles;
    public Rigidbody rb;
    public InstructionsManager instructionsManager;
    PlayerControls controls;
    public AudioManager audioManager;

    [Header("Hint")]
    private bool usingHint = false;
    private Transform closestArtifactTransform;
    public float minDistance;
    private float currentDistance;
    public float distanceInc;
    private float currentHintAngle;
    private bool errorSoundReady = true;

    [Header("Settings")]
    public float hintCooldown;
    private bool hintUnlocked = false;
    private bool hintReady = true;

    [Header("Keybinds")]
    public KeyCode hintKey = KeyCode.Q;

    void Awake(){
        controls = new PlayerControls();
        controls.GamePlay.Hint.performed += ctx => useHint();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(usingHint){
            UsingHint();
        } else if(Input.GetKeyDown(hintKey)) {
            useHint();
        }
    }

    void useHint(){
        if(!hintUnlocked) return;
        if(!hintReady){
            if(errorSoundReady){
                errorSoundReady = false;
                audioManager.PlaySound("Error");
                Invoke(nameof(SetErrorSoundReady), 2);
            }
            return; 
        } 
        closestArtifactTransform = artifactManager.GetClosestEnabledArtifact();
        currentDistance = minDistance;

        Vector2 playerPosFlat = new Vector2(rb.transform.position.x, rb.transform.position.z);
        Vector2 artifactPosFlat = new Vector2(closestArtifactTransform.position.x, closestArtifactTransform.position.z);

        Vector2 direction = artifactPosFlat - playerPosFlat;
        currentHintAngle = Mathf.Atan2(direction.y, direction.x);

        particles.gameObject.SetActive(true);

        usingHint = true;
        hintReady = false;
        Invoke(nameof(ResetHint), hintCooldown);
    }

    void UsingHint(){
        if(particles.gameObject.activeSelf == false){
            usingHint = false;
            return;
        }

        currentDistance += distanceInc;

        particles.transform.position = new Vector3(rb.transform.position.x + Mathf.Cos(currentHintAngle) * currentDistance, rb.transform.position.y + 2, rb.transform.position.z + Mathf.Sin(currentHintAngle) * currentDistance);
    
        instructionsManager.InstructionDone("Hint");
    }

    void ResetHint(){
        hintReady = true;
    }

    void SetErrorSoundReady(){
        errorSoundReady = true;
    }


    public void unlockHint(){
        hintUnlocked = true;
        instructionsManager.ShowInstruction("Hint");
    }
    private void OnEnable() {
            controls.GamePlay.Enable();
        }
    }

}
