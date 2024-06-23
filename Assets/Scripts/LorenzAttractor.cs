using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LorenzAttractor : MonoBehaviour
{
    public Slider sigSlider;
    public Slider betaSlider;
    public Slider rhoSlider;
    
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
    
    public float spawnZ = 10f; // Constant Z value for spawning
    public int maxTrails = 10; // Maximum number of trails
    private List<TrailRenderer> dynamicTrails = new List<TrailRenderer>();
    private List<Vector3> dynamicPositions = new List<Vector3>();
    private Camera mainCamera;
    
    
    
    private Vector3[] currentPositions;
    Vector3 offset = new Vector3(300, 0, 0);
    
    private void Start()
    {
        mainCamera = Camera.main;
        
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

        ClearDynamicTrails();

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
        
        
        // Check for mouse input
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement()) // 0 is left mouse button
        {
            SpawnTrailAtMousePosition();
        }

        // Update dynamic trails
        for (int i = dynamicTrails.Count - 1; i >= 0; i--)
        {
            if (dynamicTrails[i] == null)
            {
                dynamicTrails.RemoveAt(i);
                continue;
            }

            Vector3 currentPosition = dynamicPositions[i];
            
            for (int j = 0; j < numPoints; j++)
            {
                float dx = sigma * (currentPosition.y - currentPosition.x) * dt;
                float dy = (currentPosition.x * (rho - currentPosition.z) - currentPosition.y) * dt;
                float dz = (currentPosition.x * currentPosition.y - beta * currentPosition.z) * dt;

                currentPosition += new Vector3(dx, dy, dz) * speed * Time.deltaTime;
            }
            
            dynamicPositions[i] = currentPosition;
            dynamicTrails[i].transform.position = currentPosition * scale + offset;
        }
        
    }
    
    private bool IsPointerOverUIElement()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    
    private void SpawnTrailAtMousePosition()
    {
        if (dynamicTrails.Count >= maxTrails)
        {
            Destroy(dynamicTrails[0].gameObject);
            dynamicTrails.RemoveAt(0);
            dynamicPositions.RemoveAt(0);
        }

        // Create a plane that represents the current view of the Lorenz attractor
        Plane attractorPlane = new Plane(mainCamera.transform.forward, offset);

        // Cast a ray from the mouse position into the scene
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        float enter = 0.0f;
        if (attractorPlane.Raycast(ray, out enter))
        {
            // Get the point where the ray intersects the plane
            Vector3 hitPoint = ray.GetPoint(enter);

            // Adjust for scale and offset
            Vector3 localPosition = (hitPoint - offset) / scale;

            GameObject newLine = new GameObject("Dynamic Trail");
            newLine.transform.parent = transform;

            TrailRenderer newTrail = newLine.AddComponent<TrailRenderer>();
            newTrail.time = numPoints * dt;
            newTrail.startWidth = trailWidth * 5f;
            newTrail.endWidth = trailWidth;
            newTrail.material = new Material(Shader.Find("Sprites/Default"));

            Color trailColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(trailColor, 0.0f), new GradientColorKey(trailColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            newTrail.colorGradient = gradient;

            newLine.transform.position = localPosition * scale + offset;
            dynamicTrails.Add(newTrail);
            dynamicPositions.Add(localPosition);
        }
    }
    
    public void ClearDynamicTrails()
    {
        foreach (var trail in dynamicTrails)
        {
            if (trail != null)
            {
                Destroy(trail.gameObject);
            }
        }
        dynamicTrails.Clear();
        dynamicPositions.Clear();
    }
    
    
    public void ChangeSig()
    {
        sigma = sigSlider.value;
    }
    
    public void ChangeBeta()
    {
        beta = betaSlider.value;
    }
    
    public void ChangeRho()
    {
        rho = rhoSlider.value;
    }
    
    public void RandomizeParameters()
    {
        // Randomly select values for sigma, beta, and rho within their respective ranges
        sigSlider.value = Random.Range(sigSlider.minValue, sigSlider.maxValue);
        betaSlider.value = Random.Range(betaSlider.minValue, betaSlider.maxValue);
        rhoSlider.value = Random.Range(rhoSlider.minValue, rhoSlider.maxValue);

        // Update the corresponding values in the script
        ChangeSig();
        ChangeBeta();
        ChangeRho();

        Respawn();
    }
    
}