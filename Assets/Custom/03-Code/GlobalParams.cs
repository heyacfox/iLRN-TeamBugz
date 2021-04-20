using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GlobalParams : ScriptableObject
{
    public float moneyPerLocust = 0.01f;
    public float moneyToWin = 0.01f;
    //ten seconds of munching per valid location on crop before land location is destroyed.
    public float locustMunchAtLocationMaxTime = 10f;
    public float moneyPerCropSaved = 0.5f;
}
