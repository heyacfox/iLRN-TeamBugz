using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandRestart : MonoBehaviour
{
    public bool m_isLeftPinkieStaying;
    OVRHand[] m_hands;
    private void Start()
    {
        

        m_hands = new OVRHand[]
        {
            GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/OVRHandPrefab").GetComponent<OVRHand>(),
            GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/OVRHandPrefab").GetComponent<OVRHand>()
        };
        m_isLeftPinkieStaying = false;
    }

    private void Update()
    {
        if (m_hands[0].GetFingerIsPinching(OVRHand.HandFinger.Pinky))
        {
            m_isLeftPinkieStaying = true;
            loadIntroScene();
        } else
        {
            m_isLeftPinkieStaying = false;
        }
    }

    private void loadIntroScene()
    {
        SceneManager.LoadScene("IntroScene");
    }
}
