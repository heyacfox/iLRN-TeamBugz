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
    public GlobalParams globalParams;
    

    public void updateMoney(float moneyAmount)
    {
        currentMoney += moneyAmount;
        moneyIndicatorText.text = currentMoney.ToString("###.##");
        checkWinState();
    }

    public void caughtLocust()
    {
        updateMoney(globalParams.moneyPerLocust);
    }

    public void checkWinState()
    {
        if (currentMoney >= globalParams.moneyToWin)
        {
            winText.text = "You Win!";
        }
    }
}
