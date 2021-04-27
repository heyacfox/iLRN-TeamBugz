using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locust : MonoBehaviour
{
    GameManager gameManager;
    LocustLandLocation occupiedLocustLandLocation;
    Rigidbody myRigidbody;

    public float moveSpeed;
    bool useBasicMove = true;
    public AudioClip squishSound;
    public AudioClip munchLoop;

    AudioSource audioSource;

    LocustState locustState;

    public Boid boid;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        myRigidbody = GetComponent<Rigidbody>();
        moveSpeed = gameManager.globalParams.locustBasicMoveSpeed;
        audioSource = GetComponent<AudioSource>();
        boid = GetComponent<Boid>();
        audioSource.volume = 0f;

    }

    private void Start()
    {
        pickNewLandLocation();
        
    }


    public void handTouchedLocust()
    {
        Debug.Log("touched locust");
        boid.Goal = gameManager.getRandomExitLocation();
        //gameManager.caughtLocust();
        //Destroy(this.gameObject);
    }

    public void getEaten()
    {
        Destroy(this.gameObject);
    }

    public void caughtLocust()
    {
        AudioSource.PlayClipAtPoint(squishSound, this.transform.position);
        if (occupiedLocustLandLocation !=null) occupiedLocustLandLocation.isOccupied = false;

        gameManager.caughtLocust();
        Destroy(this.gameObject);
    }

    public void occupyLocation(LocustLandLocation landLocation)
    {
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



    }

    public void Update()
    {
        audioSource.volume = 0f;
        
        if (locustState == LocustState.exiting)
        {
            
        }
        else if (locustState == LocustState.eating)
        {
            occupiedLocustLandLocation.munchTimeLeft -= Time.deltaTime;
            audioSource.volume = 1f;
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
        } else if (locustState == LocustState.flyingToTarget)
        {
            
        } else
        {
            pickNewLandLocation();
        }
    }

    private void pickNewLandLocation()
    {
        //If this returns absolutely nothing, what should I do?
        LocustLandLocation tempLocation = gameManager.cropManager.getRandomLandLocation();
        if (tempLocation == null)
        {
            Debug.Log("No more locations left");
            //Somehow, fly away? there's nothing left to munch?
            //moveSpeed *= 5;
            boid.Goal = gameManager.getRandomExitLocation();
            locustState = LocustState.exiting;
        }
        boid.Goal = tempLocation.transform;
        locustState = LocustState.flyingToTarget;
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
    flyingToTarget
}
