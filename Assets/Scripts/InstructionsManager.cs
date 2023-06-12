using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Player;

namespace Instructions{
public class InstructionsManager : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TMP_Text[] tips;
    private bool[] tipsTracker;

    [SerializeField]
    private List<int> tipsStackToShow = new List<int>();
    private int currentTipIndex = 0;

    [SerializeField]
    private float currentAlpha;

    [SerializeField]
    public float alphaInc;

    [SerializeField]
    private InstructionState state;
    private enum InstructionState
        {
            waiting,
            showing,
            supressing
        }

    void Start()
    {
        tipsTracker = new bool[tips.Length];
        for(int i = 0; i < tips.Length; i++){
            tipsTracker[i] = false;
        }
        state = InstructionState.waiting;

        Invoke(nameof(AddBasicInstructionsToStack), 2);
    }

    void Update()
    {
        if(state == InstructionState.showing) currentAlpha += alphaInc;
        else currentAlpha -= alphaInc;

        currentAlpha = Mathf.Clamp(currentAlpha, -1, 1.5f);
        canvasGroup.alpha = currentAlpha;

        if(state == InstructionState.supressing && currentAlpha <= -1){
            tips[currentTipIndex].gameObject.SetActive(false);
            tipsStackToShow.RemoveAt(0);
            state = InstructionState.waiting;
        }

        if(state == InstructionState.waiting && tipsStackToShow.Count > 0){
            currentTipIndex = tipsStackToShow[0];
            tips[currentTipIndex].gameObject.SetActive(true);
            state = InstructionState.showing;
        }
    }

    public void ShowInstruction(string instructionName){
        for(int i = 0; i < tips.Length; i ++){
            if(tips[i].gameObject.name.Equals(instructionName)){
                if(!tipsTracker[i]){
                    tipsTracker[i] = true;
                    tipsStackToShow.Add(i);
                }
                return;
            }
        }
    }

    public void InstructionDone(string instructionName){
        if(tipsStackToShow.Count < 1) return;

        if(tips[tipsStackToShow[0]].gameObject.name.Equals(instructionName)){
            state = InstructionState.supressing;
        }
    }

    private void AddBasicInstructionsToStack(){
        //Movement
        tipsStackToShow.Add(0);
        tipsTracker[0] = true;
        //Jump
        tipsStackToShow.Add(1);
        tipsTracker[1] = true;
        //Sprint
        tipsStackToShow.Add(2);
        tipsTracker[2] = true;
    }
}
}
