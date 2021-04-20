using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Variables")]
    public float currentMoney = 0;
    public float timeLeftInDay;
    public float bugSpawnTimer;

    [Header("Links")]
    public TMP_Text moneyIndicatorText;
    public TMP_Text winText;
    public GlobalParams globalParams;
    public List<Transform> BugSpawnLocations;
    public Transform bugLocationsParent;
    public GameObject locustPrefab;

    public CropManager cropManager;


    private void Awake()
    {
        cropManager = FindObjectOfType<CropManager>();
        timeLeftInDay = globalParams.timePerDay;
        bugSpawnTimer = globalParams.bugSpawnEveryXSeconds;
        BugSpawnLocations = new List<Transform>();
        foreach (Transform c in bugLocationsParent)
        {
            BugSpawnLocations.Add(c);
        }
    }


    private void Update()
    {
        timeLeftInDay -= Time.deltaTime;
        if (timeLeftInDay <= 0f)
        {
            Debug.Log("Day over!!!");
        }
        bugSpawnTimer -= Time.deltaTime;
        if (bugSpawnTimer <= 0)
        {
            spawnBugs();
            bugSpawnTimer = globalParams.bugSpawnEveryXSeconds;
        }
    }

    private void spawnBugs()
    {
        //multiple bugs CAN spawn at the exact same location and I don't care about that.
        for (int i = 0; i < globalParams.bugSpawnAmountPerWave; i++)
        {
            Instantiate(locustPrefab, 
                BugSpawnLocations[Random.Range(0, BugSpawnLocations.Count)].position, 
                Quaternion.identity);
        }
    }

    public void updateMoney(float moneyAmount)
    {
        currentMoney += moneyAmount;
        moneyIndicatorText.text = currentMoney.ToString("###.##");
        checkWinState();
    }

    public void caughtLocust()
    {
        updateMoney(globalParams.moneyPerLocust);
    }

    public void checkWinState()
    {
        if (currentMoney >= globalParams.moneyToWin)
        {
            winText.text = "You Win!";
        }
    }
}
