using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericHandEnter : MonoBehaviour
{
    public UnityEvent onHandEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerHands")
        {
            if (onHandEnter.GetPersistentEventCount() > 0) onHandEnter.Invoke();
        }
    }
}
