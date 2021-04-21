using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GlobalParams : ScriptableObject
{
    [Header("Day Variables")]
    public float timePerDay = 60f;

    [Header("Money Variables")]
    public float moneyPerLocust = 0.01f;
    public float moneyToWin = 0.01f;
    public float moneyPerCropSaved = 0.5f;
    
    
    [Header("Locust Variables")]
    //ten seconds of munching per valid location on crop before land location is destroyed.
    //Crops do NOT recover after the locust has been removed
    public float locustMunchAtLocationMaxTime = 10f;
    public float locustBasicMoveSpeed = 5f;

    [Header("Spawning Variables")]
    public float bugSpawnEveryXSeconds = 5f;
    public float bugSpawnAmountPerWave = 3f;
}
