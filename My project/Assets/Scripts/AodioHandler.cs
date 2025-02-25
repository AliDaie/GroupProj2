using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    float[] audioSpectrum;
    float audioAmplitude;
    float smoothedAmplitude;
    [SerializeField] Material skyboxMaterial;
    [SerializeField] float smoothFactor = 0.1f;

    [Header("Place any objects that should be visible from the start here (also place any objects with a light here)")]
    [SerializeField] List<GameObject> reactiveGameObjects = new List<GameObject>();

    [Header("Controls general size of objects:")]
    [SerializeField] float scaleFloat = 30;
    [Header("Controls distribution size of objects:")]
    [SerializeField] float scaleMultiplier = 2;
    [SerializeField] Color objectColor = Color.white;

    [Header("Place any objects that will be enabled for the climax here")]
    [SerializeField] List<GameObject> objectsEnabledAtClimax = new List<GameObject>();

    [Header("time passed for climax:")]
    [SerializeField] float climaxTime = 10;
    float climaxTimer = 0;

    [Header("Controls general size of climax objects:")]
    [SerializeField] float climaxScaleFloat = 30;
    [Header("Controls distribution size of climax objects:")]
    [SerializeField] float climaxScaleMultiplier = 2;
    [SerializeField] Color climaxObjectColor = Color.white;

    void Start()
    {
        audioSpectrum = new float[256];

        foreach (GameObject obj in objectsEnabledAtClimax)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        //storing music amplitude into audioAmplitude variable
        audioSource.GetSpectrumData(audioSpectrum, 0, FFTWindow.Rectangular);

        audioAmplitude = 0f;
        for (int i = 0; i < audioSpectrum.Length; i++)
        {
            audioAmplitude += audioSpectrum[i];
        }

        audioAmplitude /= audioSpectrum.Length;

        // smoothFactor used to make the color sync changes each frame less jarring
        smoothedAmplitude = Mathf.Lerp(smoothedAmplitude, audioAmplitude, smoothFactor);


        float temp = smoothedAmplitude * 100f + 0.4f;
        Color skyboxColor = new Color(temp, temp, temp);
        Color newColor = new Color(temp, temp, temp) * objectColor;
        Color newColor2 = new Color(temp, temp, temp) * climaxObjectColor;
        skyboxMaterial.SetColor("_SkyTint", skyboxColor); // changing skyTint property of skybox to sync its color with music amplitude

        float temp2 = temp;
        temp = Mathf.Pow(temp, scaleMultiplier);
        // for reactive materials
        foreach (GameObject obj in reactiveGameObjects)
        {
            MeshRenderer mR = obj.GetComponent<MeshRenderer>();
            if (mR != null)
                mR.material.color = newColor;
            obj.transform.localScale = new Vector3(temp / scaleFloat, temp / scaleFloat, temp / scaleFloat);

            //for light color changing
            Light lightComponent = obj.GetComponent<Light>();
            if (lightComponent == null) lightComponent = obj.GetComponentInChildren<Light>();

            if (lightComponent != null)
                lightComponent.color = newColor;
        }

        climaxTimer += Time.deltaTime;

        temp2 = Mathf.Pow(temp2, climaxScaleFloat);
        if (climaxTimer > climaxTime)
        {
            foreach (GameObject obj in objectsEnabledAtClimax)
            {
                if (!obj.activeInHierarchy) obj.SetActive(true);

                MeshRenderer mR = obj.GetComponent<MeshRenderer>();
                if (mR != null)
                    mR.material.color = newColor2;
                obj.transform.localScale = new Vector3(temp2 / climaxScaleFloat, temp2 / climaxScaleFloat, temp2 / climaxScaleFloat);

                //for light color changing
                Light lightComponent = obj.GetComponent<Light>();
                if (lightComponent == null) lightComponent = obj.GetComponentInChildren<Light>();

                if (lightComponent != null)
                    lightComponent.color = newColor2;
            }
        }
    }
}