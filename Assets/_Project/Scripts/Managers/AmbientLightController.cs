using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientLightController : MonoBehaviour
{
    public static AmbientLightController _instance;
    bool increaseLightRunning;
    bool decreaseLightRunning;
    IEnumerator increaseLightCoroutine;
    IEnumerator decreaseLightCoroutine;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void ChangeLightning(float lightIntensity)
    {
        if (RenderSettings.ambientIntensity < lightIntensity)
        {
            if (decreaseLightRunning)
            {
                //StopCoroutine(decreaseLightCoroutine);
                StopAllCoroutines();
                decreaseLightRunning = false;
            }
            increaseLightCoroutine = IncreaseLightningOverTime(lightIntensity);
            StartCoroutine(increaseLightCoroutine);
        }
        if (RenderSettings.ambientIntensity > lightIntensity)
        {
            if (increaseLightRunning)
            {
                //StopCoroutine(increaseLightCoroutine);
                StopAllCoroutines();
                increaseLightRunning = false;
            }
            decreaseLightCoroutine = DecreaseLightningOverTime(lightIntensity);
            StartCoroutine(decreaseLightCoroutine);
        }
    }

    IEnumerator IncreaseLightningOverTime(float lightIntensity)
    {
        increaseLightRunning = true;
        for (float i = RenderSettings.ambientIntensity; i <= lightIntensity; i += .05f)
        {
            RenderSettings.ambientIntensity = i;
            yield return new WaitForSeconds(.2f);
        }
    }

    IEnumerator DecreaseLightningOverTime(float lightIntensity)
    {
        decreaseLightRunning = true;
        for (float i = RenderSettings.ambientIntensity; i >= lightIntensity; i -= .05f)
        {
            RenderSettings.ambientIntensity = i;
            yield return new WaitForSeconds(.2f);
        }
    }
}
