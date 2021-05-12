using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightControl : MonoBehaviour
{
    // In seconds
    public float DayLength; // Includes sunrise/sunset
    public float NightLength;

    /// <summary>
    /// The percentage of day time it takes to transition to full day
    /// </summary>
    [Range(0, 0.5f)]
    public float SunrisePercent;
    /// <summary>
    /// The percentage of day time it takes to transition to full night
    /// </summary>
    [Range(0, 0.5f)]
    public float SunsetPercent;

    public Light DirectionalLight;

    // Used to fade out skybox so that it actually looks like night time
    public GameObject StarDome;
    public GameObject NightDome;

    public Color MiddayColor;
    public Color SunriseColor;
    public Color SunsetColor;
    public Color NightColor;

    private Coroutine runningCoroutine = null;

    public float currentTime = 0f;

    private List<LTDescr> tweens = new List<LTDescr>();

    private void Start()
    {
        // Set everything to night values
        DirectionalLight.color = NightColor;
        DirectionalLight.transform.eulerAngles = new Vector3(-60f, -30f, 0f);
        DirectionalLight.intensity = 0;
        LeanTween.alpha(StarDome, 1f, 0.01f);
        LeanTween.alpha(NightDome, 1f, 0.01f);
        ProceedFrom(currentTime);
        Invoke("Pause", 0.5f);
    }

    public void Pause()
    {
        tweens.ForEach((LTDescr tween) => { if (!LeanTween.isPaused(tween.id))
            {
                tween.pause();
                //Debug.Log($"Paused tween [{tween.id}]");
            }
        });
    }

    /// <summary>
    /// Resumes whatever tweens were paused previously.
    /// </summary>
    public void Resume()
    {
        tweens.ForEach((LTDescr tween) => { if (LeanTween.isPaused(tween.id)) 
            { 
                tween.resume();
                Debug.Log($"Paused tween [{tween.id}]");
            }
        });
    }

    public void ProceedFrom(float time)
    {
        Pause();
        tweens.Clear();
        currentTime = time % (DayLength + NightLength);

        float sunriseEnd = DayLength * SunrisePercent;
        float dayEnd = DayLength - (DayLength * SunsetPercent);
        if (currentTime < sunriseEnd)
        {
            Sunrise();
        }
        else if (currentTime < DayLength / 2)
        {
            StartDay();
        }
        else if (currentTime < dayEnd)
        {
            EndDay();
        }
        else if (currentTime < DayLength)
        {
            Sunset();
        }
        else
        {
            Night();
        }
    }

    private void Sunrise()
    {
        float initTime = currentTime;
        float time = (DayLength * SunrisePercent) - initTime;
        tweens.Add(LeanTween.rotateAround(DirectionalLight.gameObject, DirectionalLight.transform.right, 60f, time));
        tweens.Add(LeanTween.value(gameObject, (float val) => DirectionalLight.intensity = val, 0f, 1f, time));
        tweens.Add(LeanTween.value(gameObject, (Color val) => DirectionalLight.color = val, NightColor, SunriseColor, time));
        tweens.Add(LeanTween.alpha(StarDome, 0f, time));
        tweens.Add(LeanTween.alpha(NightDome, 0f, time));
        tweens.Add(LeanTween.value(gameObject, (float val) => currentTime = val, initTime, DayLength * SunrisePercent, time).setOnComplete(() => ProceedFrom(currentTime)));
    }

    private void StartDay()
    {
        float initTime = currentTime;
        float time = (DayLength / 2) - initTime;
        tweens.Add(LeanTween.rotateAround(DirectionalLight.gameObject, DirectionalLight.transform.right, 90f, time));
        tweens.Add(LeanTween.value(gameObject, (Color val) => DirectionalLight.color = val, SunriseColor, MiddayColor, time));
        tweens.Add(LeanTween.value(gameObject, (float val) => currentTime = val, initTime, DayLength / 2, time).setOnComplete(() => ProceedFrom(currentTime)));
    }

    private void EndDay()
    {
        float initTime = currentTime;
        float time = (DayLength - (DayLength * SunsetPercent)) - initTime;
        tweens.Add(LeanTween.rotateAround(DirectionalLight.gameObject, DirectionalLight.transform.right, 90f, time));
        tweens.Add(LeanTween.value(gameObject, (Color val) => DirectionalLight.color = val, MiddayColor, SunsetColor, time));
        tweens.Add(LeanTween.value(gameObject, (float val) => currentTime = val, initTime, DayLength - (DayLength * SunsetPercent), time).setOnComplete(() => ProceedFrom(currentTime)));
    }

    private void Sunset()
    {
        float initTime = currentTime;
        float time = DayLength - initTime;
        tweens.Add(LeanTween.rotateAround(DirectionalLight.gameObject, DirectionalLight.transform.right, 60f, time));
        tweens.Add(LeanTween.value(gameObject, (float val) => DirectionalLight.intensity = val, 1f, 0f, time));
        tweens.Add(LeanTween.value(gameObject, (Color val) => DirectionalLight.color = val, SunsetColor, NightColor, time));
        tweens.Add(LeanTween.alpha(StarDome, 1f, time));
        tweens.Add(LeanTween.alpha(NightDome, 1f, time));
        tweens.Add(LeanTween.value(gameObject, (float val) => currentTime = val, initTime, DayLength, time).setOnComplete(() => ProceedFrom(currentTime)));
    }

    private void Night()
    {
        float initTime = currentTime;
        float time = (DayLength + NightLength) - initTime;
        DirectionalLight.color = NightColor;
        DirectionalLight.transform.eulerAngles = new Vector3(-60f, -30f, 0f);
        DirectionalLight.intensity = 0;
        LeanTween.alpha(StarDome, 1f, 0.01f);
        LeanTween.alpha(NightDome, 1f, 0.01f);
        tweens.Add(LeanTween.value(gameObject, (float val) => currentTime = val, initTime, DayLength + NightLength, time).setOnComplete(() => ProceedFrom(currentTime)));
    }
}
