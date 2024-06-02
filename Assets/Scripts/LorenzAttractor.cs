using UnityEngine;
using UnityEngine.UI;

public class LorenzAttractor : MonoBehaviour
{
    public Slider sigSlider;
    public Slider betaSlider;
    
    public float rho = 28.0f;
    public float sigma = 10.0f;
    public float beta = 8.0f / 3.0f;
    public float dt = 0.01f;
    public int numPoints = 1000;
    public float trailWidth = 0.1f;
    public float speed = 1.0f;
    [Range(0.1f, 10.0f)]
    public float scale = 10f;
    public int numLines = 1;

    private Vector3[] startPositions;
    private TrailRenderer[] trailRenderers;
    
    private Vector3[] currentPositions;
    Vector3 offset = new Vector3(300, 0, 0);
    
    private void Start()
    {
        Respawn();
    }

    public void Respawn()
    {
        // Destroy existing line game objects and their trail renderers
        if (trailRenderers != null)
        {
            for (int i = 0; i < trailRenderers.Length; i++)
            {
                if (trailRenderers[i] != null)
                {
                    Destroy(trailRenderers[i].gameObject);
                }
            }
        }

        startPositions = new Vector3[numLines];
        trailRenderers = new TrailRenderer[numLines];

        // Choose a random color scheme
        Color[] colorScheme = GetRandomColorScheme();

        for (int i = 0; i < numLines; i++)
        {
            GameObject line = new GameObject("Line " + i);
            line.transform.parent = transform;

            startPositions[i] = new Vector3(0, Random.Range(-1f, 1f), 15);
            line.transform.localPosition = startPositions[i] * scale;
            
            trailRenderers[i] = line.AddComponent<TrailRenderer>();
            trailRenderers[i].time = numPoints * dt;
            trailRenderers[i].startWidth = trailWidth * 5f; // Set the head width to 3 times the thickness
            trailRenderers[i].endWidth = trailWidth; // Set the tail end width to the original thickness
            trailRenderers[i].material = new Material(Shader.Find("Sprites/Default"));

            // Determine the color based on the random y position
            float t = (startPositions[i].y + 1) / 2;
            Color startColor = Color.Lerp(colorScheme[0], colorScheme[1], t);
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

            // Create a new gradient for each line
            Gradient gradient = new Gradient();

            // Set the color keys of the gradient
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(startColor, 0f);
            colorKeys[1] = new GradientColorKey(endColor, 1f);

            // Set the alpha keys of the gradient
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(0f, 1f);

            // Assign the color and alpha keys to the gradient
            gradient.SetKeys(colorKeys, alphaKeys);

            // Assign the manually created gradient to the trail renderer
            trailRenderers[i].colorGradient = gradient;
        }
        
        currentPositions = new Vector3[numLines];
        for (int i = 0; i < numLines; i++)
        {
            currentPositions[i] = startPositions[i];
        }
    }

    private Color[] GetRandomColorScheme()
    {
        Color[] colorSchemes = new Color[]
        {
            new Color(1f, 0f, 0f),  // Red
            new Color(0f, 1f, 0f),  // Green
            new Color(0f, 0f, 1f),  // Blue
            new Color(1f, 1f, 0f),  // Yellow
            new Color(1f, 0f, 1f),  // Magenta
            new Color(0f, 1f, 1f)   // Cyan
        };

        int index = Random.Range(0, colorSchemes.Length);
        return new Color[] { colorSchemes[index], Color.white };
    }

    private void Update()
    {
        for (int i = 0; i < numLines; i++)
        {
            Vector3 currentPosition = currentPositions[i];

            for (int j = 0; j < numPoints; j++)
            {
                float dx = sigma * (currentPosition.y - currentPosition.x) * dt;
                float dy = (currentPosition.x * (rho - currentPosition.z) - currentPosition.y) * dt;
                float dz = (currentPosition.x * currentPosition.y - beta * currentPosition.z) * dt;

                currentPosition += new Vector3(dx, dy, dz) * speed * Time.deltaTime;
            }

            
            currentPositions[i] = currentPosition;
            trailRenderers[i].transform.position = currentPosition * scale + offset;
        }
    }
    
    
    public void ChangeSig()
    {
        sigma = sigSlider.value;
    }
    
    public void ChangeBeta()
    {
        beta = betaSlider.value;
    }
    
}