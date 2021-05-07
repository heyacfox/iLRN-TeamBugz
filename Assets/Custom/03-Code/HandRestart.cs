using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandRestart : MonoBehaviour
{
    public bool m_isLeftPinkieStaying;
    OVRHand[] m_hands;
    public float totalLengthRequired = 2f;
    public float currentTime;

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
        } else
        {
            m_isLeftPinkieStaying = false;
        }
        if (m_isLeftPinkieStaying)
        {
            currentTime += Time.deltaTime;
            if (currentTime > totalLengthRequired)
            {
                loadIntroScene();
            }
        } else
        {
            currentTime = 0;
        }
    }

    private void loadIntroScene()
    {
        SceneManager.LoadScene("IntroScene");
    }
}
