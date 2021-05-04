using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DentedPixel;
using UnityEngine.UI;

public class CanvasFactShower : MonoBehaviour
{
    public List<string> factList;
    public int lastFact = -1;
    public int currentFact = -1;
    public Text factTextObject;
    public float timeForFade = 1.5f;

    public void Start()
    {
        LeanTween.alphaText(factTextObject.rectTransform, 0f, 0f);
        currentFact = Random.Range(0, factList.Count);
        fadeInNewFact();
        InvokeRepeating("pickNewFact", 15f, 15f);
    }

    public void pickNewFact()
    {
        while (currentFact == lastFact)
        {
            currentFact = Random.Range(0, factList.Count);
        }
        lastFact = currentFact;
        
        LeanTween.alphaText(factTextObject.rectTransform, 0f, timeForFade);
        Invoke("fadeInNewFact", timeForFade);
    }

    public void fadeInNewFact()
    {
        factTextObject.text = factList[currentFact];
        LeanTween.alphaText(factTextObject.rectTransform, 1f, timeForFade);
    }



}
