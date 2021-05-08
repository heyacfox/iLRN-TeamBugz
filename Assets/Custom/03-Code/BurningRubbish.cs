using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningRubbish : MonoBehaviour
{
    public float rubbishStock = 100f;
    public float rubbishMaxStock = 100f;
    public float rubbishBurnPerSecond = 3f;
    public float rubbishAccumulatePerSecond = 1f;
    public bool rubbishBurning;
    public ParticleSystem rubbishParticles;
    public Transform rubbishDisplay;
    public GameObject avoidanceCollider;
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        rubbishMaxStock = gameManager.globalParams.rubbishMax;
        rubbishBurnPerSecond = gameManager.globalParams.rubbishBurnPerSecond;
        rubbishAccumulatePerSecond = gameManager.globalParams.rubbishAccumulatePerSecond;
        rubbishStock = rubbishMaxStock;
    }

    public void rubbishSelected()
    {
        if (!rubbishBurning)
        {
            rubbishBurning = true;
            rubbishParticles.gameObject.SetActive(true);
            avoidanceCollider.SetActive(true);
            gameManager.usedRubbishSound();
        }
    }

    private void Update()
    {
        if (rubbishBurning)
        {
            rubbishStock -= rubbishBurnPerSecond * Time.deltaTime;
            if (rubbishStock <= 0)
            {
                rubbishStock = 0;
                rubbishParticles.gameObject.SetActive(false);
                avoidanceCollider.SetActive(false);
                rubbishBurning = false;
            }
        } else
        {
            rubbishStock += rubbishAccumulatePerSecond * Time.deltaTime;
            if (rubbishStock > rubbishMaxStock)
            {
                rubbishStock = rubbishMaxStock;
            }
        }
        
        float stockPercent = rubbishStock / rubbishMaxStock;
        rubbishDisplay.transform.localScale = new Vector3(stockPercent, stockPercent, stockPercent);
    }

}
