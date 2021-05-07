using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Locust : MonoBehaviour
{
    GameManager gameManager;
    public LocustLandLocation occupiedLocustLandLocation;
    Rigidbody myRigidbody;

    public float moveSpeed;
    bool useBasicMove = true;
    public AudioClip squishSound;
    public AudioClip munchLoop;
    public AudioClip flyLoop;
    public AudioClip handHitSound;
    public bool HandHitDelayed;

    AudioSource audioSource;

    LocustState locustState;

    public Boid boid;

    public UnityEvent onLocustHitWithHand;
    public UnityEvent onLocustPicked;
    public UnityEvent onEatenByDuck;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        myRigidbody = GetComponent<Rigidbody>();
        moveSpeed = gameManager.globalParams.locustBasicMoveSpeed;
        audioSource = GetComponent<AudioSource>();
        boid = GetComponent<Boid>();
        startFlySound();

    }

    private void startFlySound()
    {
        audioSource.clip = flyLoop;
        audioSource.time = Random.Range(0, audioSource.clip.length);
        audioSource.Play();
    }

    private void startEatingSound()
    {
        audioSource.clip = munchLoop;
        audioSource.time = Random.Range(0, audioSource.clip.length);
        audioSource.Play();
    }

    private void Start()
    {
        pickNewLandLocation();
        
    }


    public void handTouchedLocust()
    {
        Debug.Log("touched locust");
        if (!HandHitDelayed)
        {
            HandHitDelayed = true;
            audioSource.PlayOneShot(handHitSound);
            Invoke("removeHandHitAudioDelay", 1f);
        }
        if (onLocustHitWithHand.GetPersistentEventCount() > 0) onLocustHitWithHand.Invoke();
        //DON"T EXIT, but do get off the crop and go away for a little bit.
        boid.Goal = gameManager.getRandomTemporaryDestination();
        if (occupiedLocustLandLocation != null) occupiedLocustLandLocation.isOccupied = false;
        startFlySound();
        locustState = LocustState.flyingToTargetNonFood;
        //gameManager.caughtLocust();
        //Destroy(this.gameObject);
    }

    void removeHandHitAudioDelay()
    {
        HandHitDelayed = false;
    }

    public void getEaten()
    {
        if (onEatenByDuck.GetPersistentEventCount() > 0) onEatenByDuck.Invoke();
        if (occupiedLocustLandLocation != null) occupiedLocustLandLocation.isOccupied = false;
        Destroy(this.gameObject);
    }

    public void caughtLocust()
    {
        //you can't catch locusts until you finish trying to hit them first.
        if (gameManager.tutorialState != TutorialStates.hitWithHand &&
            gameManager.tutorialState != TutorialStates.spawnIn)
        {
            if (onLocustPicked.GetPersistentEventCount() > 0) onLocustPicked.Invoke();
            AudioSource.PlayClipAtPoint(squishSound, this.transform.position);
            if (occupiedLocustLandLocation != null) occupiedLocustLandLocation.isOccupied = false;

            gameManager.caughtLocust();
            Destroy(this.gameObject);
        }
    }

    public void occupyLocation(LocustLandLocation landLocation)
    {
        //If you ALREADY HAVE ONE
        //STOP HAVING IT
        if (occupiedLocustLandLocation != null) occupiedLocustLandLocation.isOccupied = false;
        landLocation.isOccupied = true;
        occupiedLocustLandLocation = landLocation;
        //begin the munch
        //if you get pinched, you need to set the land location as unoccupied. 
        //If the player HITS with the hand, then the locust flies away
        //If the player PINCHES with the hand, then they capture the locust.
        //Lock locust to the position. No longer allowed to move.
        transform.position = landLocation.transform.position;
        transform.rotation = landLocation.transform.rotation;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        locustState = LocustState.eating;
        startEatingSound();



    }

    public void Update()
    {
        if (locustState == LocustState.exiting)
        {
            //If you are eating when everyone else is leaving, stop being a boid.
            if (occupiedLocustLandLocation != null)
            {
                gameObject.layer = LayerMask.NameToLayer("Default");
            }
            
        }
        else if (locustState == LocustState.eating)
        {
            if (gameManager.levelState != LevelState.tutorialLevel)
            {
                occupiedLocustLandLocation.munchTimeLeft -= Time.deltaTime;
            }
            if (occupiedLocustLandLocation.munchTimeLeft <= 0)
            {
                
                //Destroy the munch location somehow?
                //certainly stop occupying it.
                Destroy(occupiedLocustLandLocation.gameObject);
                occupiedLocustLandLocation = null;

                //be free
                myRigidbody.constraints = RigidbodyConstraints.None;
                //find a new place to go
                pickNewLandLocation();
            }
        } else if (locustState == LocustState.flyingToTargetNonFood && gameManager.gameState != GameState.LocustsLeaving)
        {
            //If you reached the temporary destination after the smacking occurred, now go find something to eat
            if (Vector3.Distance(boid.Goal.position, transform.position) < 3)
            {
                pickNewLandLocation();
            }
        } else
        {
            pickNewLandLocation();
        }
    }

    private void pickNewLandLocation()
    {
        if (occupiedLocustLandLocation != null) occupiedLocustLandLocation.isOccupied = false;
        //If this returns absolutely nothing, what should I do?
        LocustLandLocation tempLocation = gameManager.cropManager.getRandomLandLocation();
        
        if (tempLocation == null)
        {
            Debug.Log("No more locations left");
            //Somehow, fly away? there's nothing left to munch?
            //moveSpeed *= 5;
            boid.Goal = gameManager.getRandomExitLocation();
            locustState = LocustState.exiting;
            return;
        }
        boid.Goal = tempLocation.transform;
        locustState = LocustState.flyingToTargetFood;
        transform.LookAt(tempLocation.transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Locust collided with [" + collision.gameObject.name + "]");
        if (collision.gameObject.tag == "PlayerHands")
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            handTouchedLocust();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LandLocation")
        {
            LocustLandLocation landLocation = other.gameObject.GetComponent<LocustLandLocation>();
            if (!landLocation.isOccupied)
            {
                
                occupyLocation(landLocation);
            }
        } else if (other.gameObject.tag == "LocustExit")
        {
            Destroy(this.gameObject);
        }
    }
}

public enum LocustState
{
    entering,
    exiting,
    eating,
    flyingToTargetNonFood,
    flyingToTargetFood
}
