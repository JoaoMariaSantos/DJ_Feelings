using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintParticlesManager : MonoBehaviour
{
     [Header("References")]
    public AudioManager audioManager;
    private void OnEnable() {
        audioManager.PlaySound("Hint");
    }
}
