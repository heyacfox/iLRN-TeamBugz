using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocustLandLocation : MonoBehaviour
{
    private Crop parentCrop;
    public bool isOccupied;
    public float munchTimeLeft;

    GameManager gameManager;
    


    private void Awake()
    {
        parentCrop = transform.GetComponentInParent<Crop>();
        gameManager = FindObjectOfType<GameManager>();
        munchTimeLeft = gameManager.globalParams.locustMunchAtLocationMaxTime;
    }

    public void OnDestroy()
    {
        parentCrop.removeLandLocation(this);
    }



}
