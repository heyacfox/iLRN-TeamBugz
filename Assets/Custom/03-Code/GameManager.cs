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
    bool dayComplete;

    [Header("Links")]
    public TMP_Text moneyIndicatorText;
    public TMP_Text winText;
    public GlobalParams globalParams;
    public List<Transform> BugSpawnLocations;
    List<Transform> exitLocations;
    public Transform exitLocationsParent;
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
        exitLocations = new List<Transform>();
        foreach(Transform c in exitLocationsParent)
        {
            exitLocations.Add(c);
        }
    }

    public Transform getRandomExitLocation()
    {
        return exitLocations[Random.Range(0, exitLocations.Count)];
    }


    private void Update()
    {
        timeLeftInDay -= Time.deltaTime;
        if (timeLeftInDay <= 0f && !dayComplete)
        {
            Debug.Log("Day over!!!");
            bugSpawnTimer = 100000f;
            List<Locust> allActiveLocust = new List<Locust>(FindObjectsOfType<Locust>());
            foreach(Locust l in allActiveLocust)
            {
                l.moveSpeed *= 5;
                l.targetExitLocation = getRandomExitLocation();
            }
            dayComplete = true;
        }
        bugSpawnTimer -= Time.deltaTime;
        if (bugSpawnTimer <= 0)
        {
            //if there's more to munch
            if (cropManager.getRandomLandLocation() != null)
            {
                spawnBugs();
                bugSpawnTimer = globalParams.bugSpawnEveryXSeconds;
            } else
            {
                bugSpawnTimer = 10000f;
            }
            
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
