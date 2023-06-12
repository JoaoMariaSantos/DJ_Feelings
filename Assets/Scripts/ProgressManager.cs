using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Noise;
using Artifact;
using Player;
using Terrain;
using Credits;

public class ProgressManager : MonoBehaviour
{
    [Header("Managers")]
    public NoiseManager noiseManager;
    public ArtifactManager artifactManager;
    public PlayerMovement playerMovement;
    public Dashing playerDash;
    public HintAbility hintAbility;
    public TerrainManager terrainManager;
    public AudioManager audioManager;
    public EndScreenManager endScreenManager;

    [Header("Progession Variables")]
    public List<int> neededDiamondsPerLevel;
    public List<float> feelingRadiusPerLevel;

    float currentCubeHeight;
    private int currentLevel = 0;

    void Awake()
    {
        SetDiamondProgression();
        SetNoiseMax();
    }

    void OnEnable()
    {
        ArtifactManager.OnRelicCollection += LevelingUp;
    }


    void OnDisable()
    {
        ArtifactManager.OnRelicCollection -= LevelingUp;
    }

    void LevelingUp()
    {
        float rippleEffectDuration = 6f;
        audioManager.PlaySound("LevelUp");
        terrainManager.StartRippleEffect(rippleEffectDuration);
        Invoke(nameof(LevelUp), rippleEffectDuration/2);
    }

    void LevelUp()
    {
        if (currentLevel + 1 >= neededDiamondsPerLevel.Count){
            artifactManager.EnableInfinityMode();
            endScreenManager.gameEnded();
            return;
        } 
        currentLevel++;
        artifactManager.ChangeCurrentLevel(currentLevel);
        if(currentLevel == 2) playerDash.unlockDash();
        else if(currentLevel == 1) hintAbility.unlockHint();
        SetNoiseMax();
    }

    void SetDiamondProgression()
    {
        artifactManager.SetNeededDiamonds(neededDiamondsPerLevel);
    }

    void SetNoiseMax()
    {
        noiseManager.SetMaxArousal(feelingRadiusPerLevel[currentLevel]);
        noiseManager.SetMaxValence(feelingRadiusPerLevel[currentLevel]);
    }
}
