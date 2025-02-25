using UnityEngine;

public class Bam : MonoBehaviour
{
    LineRenderer[] lineRenderer;
    static int numSphere = 500; 
    float time = 0f;
    Vector3[] initPos;
    Vector3[] startPosition, endPosition;
    float lerpFraction; // Lerp point between 0~1
    float t;

    [SerializeField] AudioSource audioSource;
    float[] audioSpectrum;
    float audioAmplitude;
    float smoothedAmplitude;
    [SerializeField] float smoothFactor = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        audioSpectrum = new float[256];
        // Assign proper types and sizes to the variables.
        lineRenderer = new LineRenderer[numSphere];
        initPos = new Vector3[numSphere]; // Start positions
        startPosition = new Vector3[numSphere]; 
        endPosition = new Vector3[numSphere]; 
        
        // Define target positions. Start = random, End = heart 
        for (int i =0; i < numSphere; i++){
            // Random start positions
            float r = 25f;
            startPosition[i] = new Vector3(r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f), r * Random.Range(-1f, 1f));        
            // Heart shape end position
            t = i* 2 * Mathf.PI / numSphere;
            endPosition[i] = new Vector3( 
                        5f*Mathf.Sqrt(2f) * Mathf.Sin(t) *  Mathf.Sin(t) *  Mathf.Sin(t),
                        5f* (- Mathf.Cos(t) * Mathf.Cos(t) * Mathf.Cos(t) - Mathf.Cos(t) * Mathf.Cos(t) + 2 *Mathf.Cos(t)) + 3f,
                        10f + Mathf.Sin(time));
        }
        // Let there be spheres..
        for (int i =0; i < numSphere; i++){
            float r = 10f; // radius of the circle
            // Position
            initPos[i] = startPosition[i];
            // Color
        
        }
        // Line
        for (int i = 1; i < numSphere; i++){
            //For creating line renderer object
            lineRenderer[i] = new GameObject("Line").AddComponent<LineRenderer>();
            lineRenderer[i].material = new Material(Shader.Find("Sprites/Default"));
            // lineRenderer[i].startColor = Color.HSVToRGB(Random.Range(0.7f, 1f), Random.Range(0.8f, 1f), Random.Range(0f, 1f));
            lineRenderer[i].endColor = Color.HSVToRGB(Random.Range(0f, 0.3f), Random.Range(0.8f, 0.94f), Random.Range(3f, 5f));
            // lineRenderer[i].startWidth = 0.3f;
            // lineRenderer[i].endWidth = 0.4f;
            // lineRenderer[i].positionCount = 2;
            // lineRenderer[i].useWorldSpace = true;    
                            
            //For drawing line in the world space, provide the x,y,z values
            lineRenderer[i].SetPosition(0, initPos[i]); //x,y and z position of the starting point of the line
            lineRenderer[i].SetPosition(1, initPos[i-1]); //x,y and z position of the end point of the line
        }

    }

    // Update is called once per frame
    void Update()
    {
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

        // Measure Time 
        time += Time.deltaTime; // Time.deltaTime = The interval in seconds from the last frame to the current one
        // what to update over time?
        for (int i =1; i < numSphere; i++){
            // Lerp : Linearly interpolates between two points.
            // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Vector3.Lerp.html
            // Vector3.Lerp(startPosition, endPosition, lerpFraction)
            
            // lerpFraction variable defines the point between startPosition and endPosition (0~1)
            // let it oscillate over time using sin function
            lerpFraction = Mathf.Sin(time) * 0.5f + 0.5f;

            // Lerp logic. Update position       
            lineRenderer[i].SetPosition(0, Vector3.Lerp(startPosition[i-1], endPosition[i-1], lerpFraction)); //x,y and z position of the starting point of the line
            lineRenderer[i].SetPosition(1, Vector3.Lerp(startPosition[i], endPosition[i], lerpFraction)); //x,y and z position of the end point of the line
            int randindex = (int)Random.Range(1,numSphere);
            // lineRenderer[i].SetPosition(1, Vector3.Lerp(startPosition[randindex], endPosition[randindex], lerpFraction)); //x,y and z position of the end point of the line

            // For now, start positions and end positions are fixed. But what if you change it over time?
            // startPosition[i]; endPosition[i];

            // Color Update over time
        }
    }
}
