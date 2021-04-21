using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locust : MonoBehaviour
{
    GameManager gameManager;
    LocustLandLocation occupiedLocustLandLocation;
    public LocustLandLocation targetLandLocation;
    public Transform targetExitLocation;
    Rigidbody myRigidbody;

    public float moveSpeed;
    bool useBasicMove = true;
    public AudioClip squishSound;
    public AudioClip munchLoop;

    AudioSource audioSource;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        myRigidbody = GetComponent<Rigidbody>();
        moveSpeed = gameManager.globalParams.locustBasicMoveSpeed;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
    }

    private void Start()
    {
        pickNewLandLocation();
    }


    public void handTouchedLocust()
    {
        Debug.Log("touched locust");
        //gameManager.caughtLocust();
        //Destroy(this.gameObject);
    }

    public void caughtLocust()
    {
        AudioSource.PlayClipAtPoint(squishSound, this.transform.position);
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



    }

    public void Update()
    {
        audioSource.volume = 0f;
        if (targetExitLocation != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetExitLocation.transform.position, Time.deltaTime * moveSpeed);
        }
        else if (occupiedLocustLandLocation != null)
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
        } else if (targetLandLocation != null && useBasicMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetLandLocation.transform.position, Time.deltaTime * moveSpeed);
        } else
        {
            pickNewLandLocation();
        }
    }

    private void pickNewLandLocation()
    {
        //If this returns absolutely nothing, what should I do?
        targetLandLocation = gameManager.cropManager.getRandomLandLocation();
        if (targetLandLocation == null)
        {
            Debug.Log("No more locations left");
            //Somehow, fly away? there's nothing left to munch?
            moveSpeed *= 5;
            targetExitLocation = gameManager.getRandomExitLocation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Locust collided with [" + collision.gameObject.name + "]");
        if (collision.gameObject.tag == "PlayerHands")
        {
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
