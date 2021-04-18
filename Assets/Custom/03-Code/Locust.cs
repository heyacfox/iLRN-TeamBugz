using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locust : MonoBehaviour
{
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }


    public void handTouchedLocust()
    {
        Debug.Log("touched locust");
        gameManager.caughtLocust();
        Destroy(this.gameObject);
    }
}
