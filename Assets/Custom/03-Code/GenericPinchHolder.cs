using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericPinchHolder : MonoBehaviour
{

    public UnityEvent onRightPinchHeld;
    public UnityEvent onLeftPinchHeld;
    public OVRHand.HandFinger fingerToHold;
    public float holdMaxTime = 2f;
    // Start is called before the first frame update


    public bool m_isRightPinchStaying;
    public bool m_isLeftPinchStaying;
    OVRHand[] m_hands;
    float rightCurrentHoldTime = 0.0f;
    float leftCurrentHoldTime = 0.0f;

    private void Start()
    {


        m_hands = new OVRHand[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/OVRHandPrefab").GetComponent<OVRHand>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/OVRHandPrefab").GetComponent<OVRHand>()
        };
        m_isRightPinchStaying = false;
        m_isLeftPinchStaying = false;
    }

    private void Update()
    {
        if (m_hands[0].GetFingerIsPinching(fingerToHold))
        {
            m_isLeftPinchStaying = true;
        } else
        {
            m_isLeftPinchStaying = false;
        }
        if (m_hands[1].GetFingerIsPinching(fingerToHold))
        {
            m_isRightPinchStaying = true;
        } else
        {
            m_isLeftPinchStaying = false;
        }

        if (m_isRightPinchStaying)
        {
            rightCurrentHoldTime += Time.deltaTime;
            if (rightCurrentHoldTime > holdMaxTime)
            {
                if (onRightPinchHeld.GetPersistentEventCount() > 0) onRightPinchHeld.Invoke();
                rightCurrentHoldTime = 0;
            }
        }
        else
        {
            rightCurrentHoldTime = 0;
        }

        if (m_isLeftPinchStaying)
        {
            leftCurrentHoldTime += Time.deltaTime;
            if (leftCurrentHoldTime > holdMaxTime)
            {
                if (onLeftPinchHeld.GetPersistentEventCount() > 0) onLeftPinchHeld.Invoke();
                leftCurrentHoldTime = 0;
            }
        }
        else
        {
            leftCurrentHoldTime = 0;
        }
    }
}
