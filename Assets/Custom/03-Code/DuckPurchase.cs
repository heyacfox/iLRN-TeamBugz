using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DuckPurchase : MonoBehaviour
{
    public TMP_Text duckPurchasePrice;
    public float purchasePrice = 0.05f;
    public GameObject duckPrefab;
    public Transform duckSpawnLocation;
    GameManager gameManager;

    public void Start()
    {
        duckPurchasePrice.text = "Price:" + purchasePrice.ToString("00.00");
        gameManager = FindObjectOfType<GameManager>();
    }

    public void tryBuyDuck()
    {
        if (gameManager.currentMoney < purchasePrice)
        {
            //you can't buy a duck
        } else
        {
            Instantiate(duckPrefab, duckSpawnLocation.position, duckSpawnLocation.rotation);
            gameManager.currentMoney -= purchasePrice;

        }
        

    }
}
