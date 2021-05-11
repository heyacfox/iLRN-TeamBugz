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

    private void Start()
    {
        // Set everything to night values
        DirectionalLight.color = NightColor;
        DirectionalLight.transform.eulerAngles = new Vector3(-60f, -30f, 0f);
        DirectionalLight.intensity = 0;
        LeanTween.alpha(StarDome, 1f, 0.01f);
        LeanTween.alpha(NightDome, 1f, 0.01f);
        StartCoroutine(DayCycle());
    }

    private IEnumerator DayCycle()
    {
        float sunriseTime = DayLength * SunrisePercent;
        float sunsetTime = DayLength * SunsetPercent;
        float dayTime = DayLength - sunriseTime - sunsetTime;
        // Sunrise
        LeanTween.rotateAround(DirectionalLight.gameObject, DirectionalLight.transform.right, 60f, sunriseTime);
        LeanTween.value(gameObject, (float val) => DirectionalLight.intensity = val, 0f, 1f, sunriseTime);
        LeanTween.value(gameObject, (Color val) => DirectionalLight.color = val, NightColor, SunriseColor, sunriseTime);
        LeanTween.alpha(StarDome, 0f, sunriseTime);
        LeanTween.alpha(NightDome, 0f, sunriseTime);
        yield return new WaitForSeconds(sunriseTime);

        // Daytime
        LeanTween.rotateAround(DirectionalLight.gameObject, DirectionalLight.transform.right, 180f, dayTime);
        LeanTween.value(gameObject, (Color val) => DirectionalLight.color = val, SunriseColor, MiddayColor, dayTime / 2);
        yield return new WaitForSeconds(dayTime / 2);
        LeanTween.value(gameObject, (Color val) => DirectionalLight.color = val, MiddayColor, SunsetColor, dayTime / 2);
        yield return new WaitForSeconds(dayTime / 2);

        // Sunset
        LeanTween.rotateAround(DirectionalLight.gameObject, DirectionalLight.transform.right, 60f, sunsetTime);
        LeanTween.value(gameObject, (float val) => DirectionalLight.intensity = val, 1f, 0f, sunsetTime);
        LeanTween.value(gameObject, (Color val) => DirectionalLight.color = val, SunsetColor, NightColor, sunsetTime);
        LeanTween.alpha(StarDome, 1f, sunsetTime);
        LeanTween.alpha(NightDome, 1f, sunsetTime);
        yield return new WaitForSeconds(sunsetTime);

        StartCoroutine(NightCycle());
    }

    private IEnumerator NightCycle()
    {
        DirectionalLight.color = NightColor;
        DirectionalLight.transform.eulerAngles = new Vector3(-60f, -30f, 0f);
        DirectionalLight.intensity = 0;
        LeanTween.alpha(StarDome, 1f, 0.01f);
        LeanTween.alpha(NightDome, 1f, 0.01f);
        yield return new WaitForSeconds(NightLength);

        StartCoroutine(DayCycle());
    }
}
