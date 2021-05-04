using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GlobalParams : ScriptableObject
{
    LevelConfig currentLevelConfig;
    public LevelConfig startingLevelConfig;

    [Header("Day Variables")]
    public float preSwarmTime = 10f;

    [Header("Money Variables")]
    public float moneyPerLocust = 0.01f;

    [Header("WinConditions")]
    public float basicWin = 1.0f;
    public float mediumWin = 3.0f;
    public float goodWin = 6.0f;
    
    
    [Header("Locust Variables")]
    //ten seconds of munching per valid location on crop before land location is destroyed.
    //Crops do NOT recover after the locust has been removed
    public float locustMunchAtLocationMaxTime = 10f;
    public float locustBasicMoveSpeed = 5f;


    public float getCurrentLevelSpawnInterval()
    {
        return currentLevelConfig.spawnWaveEveryXSeconds;
    }

    public float getCurrentLevelSpawnPerWave()
    {
        return currentLevelConfig.bugsPerWave;
    }

    public float getCurrentLevelTotalWaves()
    {
        return currentLevelConfig.wavesToSpawn;
    }

    public float getCurrentLevelTotalTime()
    {
        return currentLevelConfig.totalLevelTimeLengthSeconds;
    }

    public void getNextLevel()
    {
        if (currentLevelConfig.nextLevel != null)
        {
            currentLevelConfig = currentLevelConfig.nextLevel;
        }
    }
    
    public void useStartingLevel()
    {
        currentLevelConfig = startingLevelConfig;
    }
}
