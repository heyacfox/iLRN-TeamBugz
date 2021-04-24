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
    //For now start it on swarming
    public GameState gameState = GameState.Swarming;

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
        winText.text = "Defend";
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
        updateMoneyDisplay();
    }

    public Transform getRandomExitLocation()
    {
        return exitLocations[Random.Range(0, exitLocations.Count)];
    }


    private void Update()
    {
        if (gameState == GameState.MenuState)
        {
            //Do nothing right now? waiting for the user to do something?
        } else if (gameState == GameState.PreSwarming)
        {

        } else if (gameState == GameState.Swarming)
        {
            timeLeftInDay -= Time.deltaTime;
            if (timeLeftInDay <= 0f && !dayComplete)
            {
                endDay();
                return;
            }
            bugSpawnTimer -= Time.deltaTime;
            if (bugSpawnTimer <= 0)
            {
                //if there's more to munch
                if (cropManager.getRandomLandLocation() != null)
                {
                    spawnBugs();
                    bugSpawnTimer = globalParams.bugSpawnEveryXSeconds;
                }
                else
                {
                    bugSpawnTimer = 10000f;
                }

            }
        } else if (gameState == GameState.LocustsLeaving)
        {
            //watch until there are no bugs left?
            if (FindObjectsOfType<Locust>().Length <= 0)
            {
                //all the bugs have left. Show the player how much money they have from crops. 
                currentMoney += globalParams.moneyPerCropSaved * cropManager.cropsCurrent();
                //Run some sort of check.
                updateMoneyDisplay();
                if (currentMoney >= globalParams.moneyToWin)
                {
                    
                    winText.text = "You win!";
                } else
                {
                    winText.text = "You lose.";
                }

                gameState = GameState.EndDay;
            }
        } else if (gameState == GameState.EndDay)
        {
            //it's the end of the game. Allow them to restart the experience? Show them their result?
        }

        

        if (dayComplete)
        {
            
        }
    }

    private void updateMoneyDisplay()
    {
        moneyIndicatorText.text = $"Money: {currentMoney.ToString("0.00")}";
    }

    private void endDay()
    {
        List<Locust> allActiveLocust = new List<Locust>(FindObjectsOfType<Locust>());
        foreach (Locust l in allActiveLocust)
        {
            //l.moveSpeed *= 5;
            l.boid.Goal = getRandomExitLocation();
        }
        gameState = GameState.LocustsLeaving;
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
        updateMoneyDisplay();
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

//Menu state: Before the world has been entered.
//Pre-swarming: Player is in the environment, but the swarms haven't started spawning in yet
//Swarming: Swarms actively spawning in, 
//LocustsLeaving: Swarms no longer spawning in, and all bugs are leaving
//EndDay: All the locusts have left
public enum GameState
{
    MenuState,
    PreSwarming,
    Swarming,
    LocustsLeaving,
    EndDay
}
