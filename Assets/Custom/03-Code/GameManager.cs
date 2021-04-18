using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Variables")]
    public float currentMoney = 0;

    [Header("Links")]
    public TMP_Text moneyIndicatorText;
    public TMP_Text winText;

    [Header("Global Params")]
    public float moneyPerLocust = 0.01f;
    public float moneyToWin = 0.01f;

    

    public void updateMoney(float moneyAmount)
    {
        currentMoney += moneyAmount;
        moneyIndicatorText.text = currentMoney.ToString("###.##");
        checkWinState();
    }

    public void caughtLocust()
    {
        updateMoney(moneyPerLocust);
    }

    public void checkWinState()
    {
        if (currentMoney >= moneyToWin)
        {
            winText.text = "You Win!";
        }
    }
}
