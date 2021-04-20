using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locust : MonoBehaviour
{
    GameManager gameManager;
    LocustLandLocation occupiedLocustLandLocation;
    

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
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
