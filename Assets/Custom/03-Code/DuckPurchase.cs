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
    public AudioClip failBuyDuckClip;
    GameManager gameManager;

    public void Start()
    {
        
        gameManager = FindObjectOfType<GameManager>();
        purchasePrice = gameManager.globalParams.duckPurchaseCost;
        duckPurchasePrice.text = "Price:" + purchasePrice.ToString("$0.00");
    }

    public void tryBuyDuck()
    {
        if (gameManager.currentMoney < purchasePrice)
        {
            //you can't buy a duck
            GetComponent<AudioSource>().PlayOneShot(failBuyDuckClip);

        } else
        {
            Instantiate(duckPrefab, duckSpawnLocation.position, duckSpawnLocation.rotation);
            gameManager.updateMoney(-purchasePrice);
            gameManager.boughtDuckSound();

        }
        

    }
}
