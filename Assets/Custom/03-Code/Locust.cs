using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locust : MonoBehaviour
{
    GameManager gameManager;
    LocustLandLocation occupiedLocustLandLocation;
    public LocustLandLocation targetLandLocation;
    Rigidbody myRigidbody;

    public float moveSpeed;
    bool useBasicMove = true;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        myRigidbody = GetComponent<Rigidbody>();
        moveSpeed = gameManager.globalParams.locustBasicMoveSpeed;
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
        if (occupiedLocustLandLocation != null)
        {
            occupiedLocustLandLocation.munchTimeLeft -= Time.deltaTime;
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
        }
    }
}
