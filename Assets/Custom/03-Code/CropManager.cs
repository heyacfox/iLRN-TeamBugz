using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropManager : MonoBehaviour
{
    public List<Crop> allCrops;

    public void Awake()
    {
        allCrops = new List<Crop>(FindObjectsOfType<Crop>());
    }

    public LocustLandLocation getRandomLandLocation()
    {
        //pick a random crop
        //MUST BE UNOCCUPIED
        List<LocustLandLocation> allLandLocations = new List<LocustLandLocation>();
        foreach (Crop crop in allCrops)
        {
            allLandLocations.AddRange(crop.landLocations);
        }

        List<LocustLandLocation> unoccupiedLocations = allLandLocations.FindAll(l => !l.isOccupied);
        Debug.Log("Current Unoccupied locations: [" + unoccupiedLocations.Count.ToString() + "]");
        if (unoccupiedLocations.Count > 0)
        {
           
            return unoccupiedLocations[Random.Range(0, unoccupiedLocations.Count)];
        } else
        {
            return null;
        }
        
        
    }

    public void removeCrop(Crop crop)
    {
        //if you run out of crops...game over? Still play? What happens here?
        allCrops.Remove(crop);
    }
}
