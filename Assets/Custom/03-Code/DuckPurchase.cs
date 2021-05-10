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
    public MeshRenderer cyllinder;
    public SkinnedMeshRenderer duck;
    public Material duckDefault;
    public Material duckCanBuy;

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

    public void Update()
    {
        string moneyText = $"£{gameManager.currentMoney}/£{purchasePrice.ToString()}";
        if (gameManager.currentMoney >= purchasePrice)
        {
            moneyText += "\nBUY DUCK NOW!";
            cyllinder.material.color = Color.yellow;
        } else
        {
            cyllinder.material.color = Color.black;
        }
        duckPurchasePrice.text = moneyText;
    }
}
