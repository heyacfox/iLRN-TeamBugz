using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    [Header("Variables")]
    public float currentMoney = 0;
    public float timeLeftInDay;
    public float bugSpawnTimer;
    bool dayComplete;
    //For now start it on swarming
    public GameState gameState = GameState.Swarming;
    public LevelState levelState = LevelState.tutorialLevel;
    public TutorialStates tutorialState = TutorialStates.spawnIn;

    [Header("Links")]
    public TMP_Text moneyIndicatorText;
    public TMP_Text winText;
    public GlobalParams globalParams;
    public List<Transform> BugSpawnLocations;
    List<Transform> exitLocations;
    public Transform exitLocationsParent;
    public Transform temporaryLocationsParent;
    List<Transform> tempLocations;
    public Transform bugLocationsParent;
    public GameObject locustPrefab;

    public CropManager cropManager;

    public GameObject smokeParent;
    public GameObject duckParent;

    public GameObject creditsCanvas;

    [Header("AudioLinks")]
    public AudioSource playerAudioSource;
    public AudioClip tutorialHandShoo;
    public AudioClip tutorialPick1;
    public AudioClip tutorialPick2;
    public AudioClip tutorialBugsComing;
    public List<AudioClip> listOfPickingSounds;
    public List<AudioClip> listOfCropDestroyedSounds;
    public List<AudioClip> listOfBurnSounds;
    public List<AudioClip> listOfNewDuckSounds;
    public float pickSoundPlayPercentChance = 0.2f;
    public AudioMixer mainAudioMixer;

    [Header("AudioLinksPreswarms")]
    public AudioClip pinchOnlyClip;
    public AudioClip smokeClip;
    public AudioClip duck1Clip;
    public AudioClip duck2Clip;
    public AudioClip duck3Clip;
    public AudioClip loseGameClip;
    public AudioClip winLevel1Clip;
    public AudioClip winLevel2Clip;
    public AudioClip winLevel3Clip;
    public AudioClip preSwarmOnCreditsClip;
    public AudioClip startSuperHardMode;

    private int hardModeBugTracker = 1;
    private float hardModeBugSpawnEvery = 5f;



    private void Awake()
    {
        cropManager = FindObjectOfType<CropManager>();

        
        
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
        tempLocations = new List<Transform>();
        foreach (Transform c in temporaryLocationsParent)
        {
            tempLocations.Add(c);
        }
        updateMoneyDisplay();
        if (levelState == LevelState.tutorialLevel && tutorialState == TutorialStates.spawnIn)
        {
            narratorDialogueBegin(tutorialHandShoo);
            tutorialState = TutorialStates.hitWithHand;
        } else
        {
            globalParams.getNextLevel();
            timeLeftInDay = globalParams.getCurrentLevelTotalTime();
            bugSpawnTimer = globalParams.getCurrentLevelSpawnInterval();
        }
    }

    private void narratorDialogueBegin(AudioClip audioClip)
    {
        mainAudioMixer.FindSnapshot("FieldNarratorTalking").TransitionTo(0.5f);
        playerAudioSource.Stop();
        playerAudioSource.clip = audioClip;
        playerAudioSource.Play();
        Invoke("narratorDialogueEnd", audioClip.length);
    }

    private void narratorDialogueEnd()
    {
        mainAudioMixer.FindSnapshot("FieldSnapshot").TransitionTo(0.5f);
    }

    public Transform getRandomExitLocation()
    {
        return exitLocations[Random.Range(0, exitLocations.Count)];
    }
    public Transform getRandomTemporaryDestination()
    {
        return tempLocations[Random.Range(0, tempLocations.Count)];
    }

    public void cropEatenSound()
    {
        if (playerAudioSource != null && !playerAudioSource.isPlaying)
        {
            playerAudioSource.clip = listOfCropDestroyedSounds[Random.Range(0, listOfCropDestroyedSounds.Count)];
            playerAudioSource.Play();
        }
    }

    public void boughtDuckSound()
    {
        if (!playerAudioSource.isPlaying)
        {
            playerAudioSource.clip = listOfNewDuckSounds[Random.Range(0, listOfNewDuckSounds.Count)];
        playerAudioSource.Play();
        }
    }

    public void pinchedBugSound()
    {
        if (!playerAudioSource.isPlaying)
        {
            playerAudioSource.clip = listOfPickingSounds[Random.Range(0, listOfPickingSounds.Count)];
            playerAudioSource.Play();
        }
    }

    public void usedRubbishSound()
    {
            if (!playerAudioSource.isPlaying)
            {
                playerAudioSource.clip = listOfBurnSounds[Random.Range(0, listOfBurnSounds.Count)];
                playerAudioSource.Play();
            }
    }


    private void Update()
    {
        winText.text = $"GAMESTATE: {gameState.ToString()}| \nLEVELSTATE: {levelState.ToString()}| \nTUTSTATE: {tutorialState.ToString()}";
        if (gameState == GameState.MenuState)
        {
            //Do nothing right now? waiting for the user to do something?
        } else if (levelState == LevelState.tutorialLevel)
        {

        } else if (levelState == LevelState.credits)
        {

        }
            
                else
        {
            insideRealLevel();
        }
        
    }

    public void tutorialBugShooed()
    {
        if (levelState == LevelState.tutorialLevel && tutorialState == TutorialStates.hitWithHand)
        {
            narratorDialogueBegin(tutorialPick1);
            //if the player doesn't pick the bug
            InvokeRepeating("bugPinchReminder", tutorialPick1.length + 10f, 20f);
            tutorialState = TutorialStates.pinch;
        }
    }

    private void bugPinchReminder()
    {
        narratorDialogueBegin(tutorialPick2);
    }

    public void tutorialBugPicked()
    {
        if (levelState == LevelState.tutorialLevel &&tutorialState == TutorialStates.pinch)
        {
            CancelInvoke();
            //playerAudioSource.Stop();
            //playerAudioSource.clip = tutorialPick2;
            //playerAudioSource.Play();
            tutorialState = TutorialStates.phoneRing;
            //Invoke("tutorialPhoneInfoComplete", tutorialPick2.length);
            tutorialPhoneInfoComplete();
        }
        
    }

    public void tutorialPhoneInfoComplete()
    {
        if (levelState == LevelState.tutorialLevel && tutorialState == TutorialStates.phoneRing)
        {
            //playerAudioSource.Stop();
            //playerAudioSource.clip = tutorialPick2;
            //playerAudioSource.Play();
            /*
            if (gameState == GameState.Tutorial)
            {
                levelState = LevelState.pinchOnly;
            }
            */
            preSwarmBegins();
        }

    }

    public void swarmBegins()
    {
        gameState = GameState.Swarming;
        Invoke("swarmEnds", globalParams.getCurrentLevelTotalTime());
    }

    public void swarmEnds()
    {
        endDay();
        
        //(need to go to the next level)
        
    }

    private void playCorrectPreswarmAudio()
    {
        AudioClip clipToUse = pinchOnlyClip;
        if (levelState == LevelState.pinchOnly)
        {
            clipToUse = pinchOnlyClip;
        } else if (levelState == LevelState.smoke)
        {
            clipToUse = smokeClip;
        }
        else if (levelState == LevelState.ducks1)
        {
            clipToUse = duck1Clip;
        }
        else if (levelState == LevelState.ducks2)
        {
            clipToUse = duck2Clip;
        }
        else if (levelState == LevelState.ducks3)
        {
            clipToUse = duck3Clip;
        } else if (levelState == LevelState.credits)
        {
            return;
        }

        narratorDialogueBegin(clipToUse);
    }

    public void preSwarmBegins()
    {
        //I hate it I hate it I hate it I hate it I hate it but technically it will work to get
        //the next enum of the level
        cropManager.fakeFreeAllLocations();
        gameState = GameState.PreSwarming;
        levelState++;
        if (levelState == LevelState.credits)
        {
            checkWinState();
        } else
        {
            playCorrectPreswarmAudio();

            if (levelState != LevelState.pinchOnly && levelState != LevelState.hardMode)
            {
                //starter level is tutorial, which doesn't actuall have a config.
                globalParams.getNextLevel();
            } else
            {
                globalParams.useStartingLevel();
            }
            
            timeLeftInDay = globalParams.getCurrentLevelTotalTime();
            bugSpawnTimer = 0.0f;
            gameState = GameState.PreSwarming;

            if (levelState == LevelState.smoke)
            {
                smokeParent.SetActive(true);
            } else if (levelState == LevelState.ducks1)
            {
                duckParent.SetActive(true);
            }

            Invoke("swarmBegins", globalParams.preSwarmTime);
        }
        
    }

    private void insideRealLevel()
    {
        if (gameState == GameState.Swarming)
        {
            if (cropManager.cropsCurrent() <= 0)
            {
                checkWinState();
            }
            timeLeftInDay -= Time.deltaTime;
            if (timeLeftInDay <= 0f && !dayComplete)
            {
                endDay();
                return;
            }
            bugSpawnTimer -= Time.deltaTime;
            if (bugSpawnTimer <= 0)
            {
                spawnBugs();
                if (levelState != LevelState.hardMode)
                {
                    bugSpawnTimer = globalParams.getCurrentLevelSpawnInterval();
                }
                else
                {
                    bugSpawnTimer = hardModeBugSpawnEvery;
                }

                //always spawn bugs, regardless if there's not open spots.

                //if there's more to munch
                if (cropManager.getRandomLandLocation() != null)
                {
                    
                    
                }
                else
                {
                    //we're out of munch spots? You get to lose.
                    //checkWinState();
                }

            }
        }
        else if (gameState == GameState.LocustsLeaving)
        {
            //watch until there are no bugs left?
            if (FindObjectsOfType<Locust>().Length <= 0)
            {
                //all the bugs have left. Show the player how much money they have from crops. 
                //currentMoney += globalParams.moneyPerCropSaved * cropManager.cropsCurrent();
                //Run some sort of check.
                updateMoneyDisplay();

                gameState = GameState.EndDay;
            }
        }
        else if (gameState == GameState.EndDay)
        {
            preSwarmBegins();
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
            if (l.occupiedLocustLandLocation != null)
            {
                l.occupiedLocustLandLocation.isOccupied = false;
            }
        }
        gameState = GameState.LocustsLeaving;
    }

    private void spawnBugs()
    {
        //multiple bugs CAN spawn at the exact same location and I don't care about that.
        if (levelState != LevelState.hardMode)
        {
            for (int i = 0; i < globalParams.getCurrentLevelSpawnPerWave(); i++)
            {
                Instantiate(locustPrefab,
                    BugSpawnLocations[Random.Range(0, BugSpawnLocations.Count)].position,
                    Quaternion.identity);
            }
        } else
        {
            //hard mode, one more bug spawns every wave, until you lose. 
            //tell the player how many waves they lasted
            for (int i = 0; i < hardModeBugTracker; i++)
            {
                Instantiate(locustPrefab,
                    BugSpawnLocations[Random.Range(0, BugSpawnLocations.Count)].position,
                    Quaternion.identity);
            }
            hardModeBugTracker++;
            timeLeftInDay += 100;
        }
    }

    public void updateMoney(float moneyAmount)
    {
        currentMoney += moneyAmount;
        updateMoneyDisplay();
    }

    public void caughtLocust()
    {
        if (levelState != LevelState.tutorialLevel)
        {
            float randomCatchAudioChance = Random.Range(0, 1f);
            if (randomCatchAudioChance < pickSoundPlayPercentChance)
            {
                pinchedBugSound();
            }
        }
        updateMoney(globalParams.moneyPerLocust);
    }

    public void checkWinState()
    {
        
        if (cropManager.cropsCurrent() >= globalParams.goodWin)
        {
            narratorDialogueBegin(winLevel3Clip);
            creditsCanvas.SetActive(true);
            levelState = LevelState.credits;
        } else if (cropManager.cropsCurrent() >= globalParams.mediumWin)
        {
            narratorDialogueBegin(winLevel2Clip);
            creditsCanvas.SetActive(true);
            levelState = LevelState.credits;
        } else if (cropManager.cropsCurrent() >= globalParams.basicWin)
        {
            narratorDialogueBegin(winLevel1Clip);
            creditsCanvas.SetActive(true);
            levelState = LevelState.credits;
        } else
        {
            narratorDialogueBegin(loseGameClip);
            Debug.Log("YOU LOST");
            levelState = LevelState.credits;
            creditsCanvas.SetActive(true);
        }
        
    }

    public void hardModeTrigger()
    {
        /*
        if (levelState == LevelState.credits)
        {
            creditsCanvas.SetActive(false);
            globalParams.setLevelTo(globalParams.hardModeLevelConfig);
            preSwarmBegins();
        }
        */
    }
    
    

    //Code related to tutorial States

}

//Menu state: Before the world has been entered.
//Pre-swarming: Player is in the environment, but the swarms haven't started spawning in yet
//Swarming: Swarms actively spawning in, 
//LocustsLeaving: Swarms no longer spawning in, and all bugs are leaving
//EndDay: All the locusts have left
public enum GameState
{
    MenuState,
    Tutorial,
    PreSwarming,
    Swarming,
    //(Dialogue tag: Go on, shoo, get out of here!)
    LocustsLeaving,
    //(Dialogue: The character comments on how many crops are left)
    EndDay
}

public enum LevelState
{
    tutorialLevel,
    pinchOnly,
    smoke,
    ducks1,
    ducks2,
    ducks3,
    credits,
    hardMode,
    hardModeEnd

}

//

public enum TutorialStates
{
    //Initial State is actually a separate scene

    //INTRO SCENE, HOUSE TABLE WITH A PHONE, EVERYTHING ELSE IS BLACK. 

    //The phone will run until to press it with your hands. 

    //Lights come up on the scene

    spawnIn,
    //After spawn in is complete, 
    hitWithHand,
    pinch,
    phoneRing,


}
