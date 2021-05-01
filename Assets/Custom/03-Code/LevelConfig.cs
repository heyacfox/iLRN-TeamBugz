using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelConfig : ScriptableObject
{
    public float wavesToSpawn = 5;
    public float bugsPerWave = 3;
    public float spawnWaveEveryXSeconds = 10;
    public float totalLevelTimeLengthSeconds = 60;
    public LevelConfig nextLevel;
}
