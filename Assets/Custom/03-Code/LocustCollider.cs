using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocustCollider : MonoBehaviour
{
    public Locust linkedLocust;

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Locust collided with [" + collision.gameObject.name + "]");
        if (collision.gameObject.tag == "PlayerHands")
        {
            linkedLocust.handTouchedLocust();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LandLocation")
        {
            LocustLandLocation landLocation = other.gameObject.GetComponent<LocustLandLocation>();
            if (!landLocation.isOccupied)
            {
                linkedLocust.occupyLocation(landLocation);
            }
        }
    }
}
