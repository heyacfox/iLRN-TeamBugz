using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public List<LocustLandLocation> landLocations;
    CropManager cropManager;

    // Start is called before the first frame update
    void Awake()
    {
        landLocations = new List<LocustLandLocation>(GetComponentsInChildren<LocustLandLocation>());
        cropManager = FindObjectOfType<CropManager>();

    }

    public LocustLandLocation returnRandomActiveLandLocation()
    {
        List<LocustLandLocation> unoccupiedLocations = landLocations.FindAll(l => !l.isOccupied);
        if (unoccupiedLocations.Count > 0)
        {
            return unoccupiedLocations[Random.Range(0, landLocations.Count)];
        } else
        {
            //ouch no oh no why am I doing this.
            return null;
        }
        
    }

    //if you no longer have any land locations; you die.
    public void removeLandLocation(LocustLandLocation landLocation)
    {
        landLocations.Remove(landLocation);
        if (landLocations.Count <= 0)
        {
            cropManager.removeCrop(this);
        }
        //make it smaller as it's getting munched?
        transform.localScale = new Vector3(transform.localScale.x / 2f, transform.localScale.y, transform.localScale.z / 2f);
    }

    
}
