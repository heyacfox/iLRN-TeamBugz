using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwapManager : MonoBehaviour
{
    public GameObject introParent;
    public GameObject fieldParent;
    // Start is called before the first frame update
    public void swapToField()
    {
        introParent.SetActive(false);
        fieldParent.SetActive(true);
    }

    public void swapToIntro()
    {
        fieldParent.SetActive(false);
        introParent.SetActive(true);
    }
}
