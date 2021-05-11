using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericHandEnter : MonoBehaviour
{
    public UnityEvent onHandEnter;
    public float actionDelayTime;
    bool isDelayed = false;


    private void OnTriggerEnter(Collider other)
    {
        if (!isDelayed)
        {
            if (other.gameObject.tag == "PlayerHands")
            {
                if (onHandEnter.GetPersistentEventCount() > 0) onHandEnter.Invoke();
                isDelayed = true;
                Invoke("removeDelay", actionDelayTime);
            }
        }
    }

    void removeDelay()
    {
        isDelayed = false;
    }
}
