using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Period3 : MonoBehaviour
{
    public float a = 3.8284f; // The parameter value for period-3 points
    public float x0 = 0.5f; // The initial value for the logistic map
    public int iterations = 100; // The number of iterations to perform
    public float lineLength = 10f; // The desired length of the line
    public float amplitude = 1f; // The amplitude of the visualization

    public Slider aSlider; // Reference to the slider for parameter 'a'
    public Slider x0Slider; // Reference to the slider for initial value 'x0'
    public Slider iterationsSlider; // Reference to the slider for 'iterations'

    private LineRenderer lineRenderer;
    private LineRenderer period3PointsRenderer;

    private void Start()
    {
        // Initialize the LineRenderer components
        lineRenderer = GetComponent<LineRenderer>();
        period3PointsRenderer = transform.Find("Period3Points").GetComponent<LineRenderer>();

        // Set the initial values of the sliders
        aSlider.value = a;
        x0Slider.value = x0;
        iterationsSlider.value = iterations;

        // Add listeners to the sliders' onValueChanged event
        aSlider.onValueChanged.AddListener(UpdateParameterA);
        x0Slider.onValueChanged.AddListener(UpdateInitialValue);
        iterationsSlider.onValueChanged.AddListener(UpdateIterations);

        // Build the initial visualization
        BuildVisualization();
    }

    private void BuildVisualization()
    {
        // Clear the LineRenderers
        lineRenderer.positionCount = 0;
        period3PointsRenderer.positionCount = 0;

        float x = x0;

        // Set the position count of the LineRenderer
        lineRenderer.positionCount = iterations;

        // Calculate the scale factor based on the desired line length and iterations
        float scaleFactor = lineLength / (iterations - 1);

        // Calculate and visualize the logistic map points
        for (int i = 0; i < iterations; i++)
        {
            x = a * x * (1 - x);
            Vector3 currentPoint = new Vector3(i * scaleFactor, x * amplitude, 0f);

            // Set the position of the LineRenderer
            lineRenderer.SetPosition(i, currentPoint);
        }

        // Calculate and visualize the period-3 points
        float[] period3Points = CalculatePeriod3Points(x);
        period3PointsRenderer.positionCount = period3Points.Length;

        for (int i = 0; i < period3Points.Length; i++)
        {
            Vector3 period3Point = new Vector3(i * scaleFactor, period3Points[i] * amplitude, 0f);
            period3PointsRenderer.SetPosition(i, period3Point);
        }
    }


    private float[] CalculatePeriod3Points(float x)
    {
        List<float> period3Points = new List<float>();
        float epsilon = 0.0001f; // Small value to check for convergence
        int maxIterations = 100; // Maximum number of iterations for convergence
    
        float x1 = a * x * (1 - x);
        float x2 = a * x1 * (1 - x1);
        float x3 = a * x2 * (1 - x2);

        // Check if x1, x2, and x3 form a period-3 cycle
        if (Mathf.Abs(x1 - a * x3 * (1 - x3)) < epsilon &&
            Mathf.Abs(x2 - a * x1 * (1 - x1)) < epsilon &&
            Mathf.Abs(x3 - a * x2 * (1 - x2)) < epsilon)
        {
            // Duplicate the period-3 points until the array has length equal to iterations
            while (period3Points.Count < iterations)
            {
                period3Points.Add(x1);
            
                if (period3Points.Count < iterations)
                    period3Points.Add(x2);
            
                if (period3Points.Count < iterations)
                    period3Points.Add(x3);
            }
        }

        // Convert the list of period-3 points to an array
        return period3Points.ToArray();
    }


    private void UpdateParameterA(float value)
    {
        a = value;
        BuildVisualization();
    }

    private void UpdateInitialValue(float value)
    {
        x0 = value;
        BuildVisualization();
    }

    private void UpdateIterations(float value)
    {
        iterations = (int)value;
        BuildVisualization();
    }
}