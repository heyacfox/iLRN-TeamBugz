using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericOnOff : MonoBehaviour
{
    public GameObject onOffObject;

    public void onOff()
    {
        onOffObject.SetActive(!onOffObject.activeSelf);
    }
}
