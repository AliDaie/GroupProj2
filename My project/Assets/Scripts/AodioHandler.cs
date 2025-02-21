using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    float[] audioSpectrum;
    float audioAmplitude;
    float smoothedAmplitude;
    [SerializeField] Material skyboxMaterial;
    [SerializeField] float smoothFactor = 0.1f;

    void Start()
    {
        audioSpectrum = new float[256];
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
        Color newColor = new Color(temp, temp, temp);
        skyboxMaterial.SetColor("_SkyTint", newColor); // changing skyTint property of skybox to sync its color with music amplitude

        Debug.Log("current skybox color: " + newColor);
    }
}

