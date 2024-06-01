using UnityEngine;

public class LorenzAttractor : MonoBehaviour
{
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

        for (int i = 0; i < numLines; i++)
        {
            GameObject line = new GameObject("Line " + i);
            line.transform.parent = transform;

            startPositions[i] = new Vector3(-1, Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            line.transform.localPosition = startPositions[i] * scale;
            
            trailRenderers[i] = line.AddComponent<TrailRenderer>();
            trailRenderers[i].time = numPoints * dt;
            trailRenderers[i].startWidth = trailWidth * 5f; // Set the head width to 3 times the thickness
            trailRenderers[i].endWidth = trailWidth; // Set the tail end width to the original thickness
            trailRenderers[i].material = new Material(Shader.Find("Sprites/Default"));

            // Create a new gradient for each line
            Gradient gradient = new Gradient();

            // Set the starting color based on the starting position
            Color startColor = new Color(
                Mathf.Clamp01(startPositions[i].x),
                Mathf.Clamp01(startPositions[i].y),
                Mathf.Clamp01(startPositions[i].z),
                1f
            );

            // Set the end color to be fully transparent
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

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
}