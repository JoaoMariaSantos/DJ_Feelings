using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Credits{
public class EndScreenManager : MonoBehaviour
{
    private float currentOpacity = 0;
    public CanvasGroup canvasGroup;
    private bool screenEnabled = false;
    PlayerControls controls;
    public AudioManager audioManager;

    void Awake(){
        controls = new PlayerControls();
        controls.GamePlay.Jump.performed += ctx => disableEndScreen();
    }
    void Start(){
    }
    void Update()
    {
        Debug.Log(currentOpacity);
        if(screenEnabled){
            currentOpacity += Time.deltaTime * 0.3f;
            if(currentOpacity >= 3){
                screenEnabled = false;
            }
        } else {
            currentOpacity -= Time.deltaTime * 0.22f;
        }

        canvasGroup.alpha = currentOpacity;

        currentOpacity = Mathf.Clamp(currentOpacity, 0, 3);
    }

    public void gameEnded(){
        currentOpacity = 0;
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        enableEndScreen();
        Invoke(nameof(StartEndSound), 0.5f);
    }

    private void enableEndScreen(){
        screenEnabled = true;
    }

    private void disableEndScreen(){
        if(!screenEnabled) return;
        screenEnabled = false;
    }

    private void OnEnable(){
        controls.GamePlay.Enable();

    }

    private void OnDisable(){
        controls.GamePlay.Disable();
    }

    private void StartEndSound(){
        audioManager.PlaySound("EndScreen");
    }
}
}
