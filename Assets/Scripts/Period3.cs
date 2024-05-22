using UnityEngine;
using UnityEngine.UI;

public class Period3 : MonoBehaviour
{
    public float a = 3.83f; // The parameter value for the logistic map
    public float x0 = 0.5f; // The initial value for the logistic map
    public int iterations = 100; // The number of iterations to perform
    public float lineLength = 10f; // The desired length of the line
    public float amplitude = 1f; // The amplitude of the visualization

    public Slider aSlider; // Reference to the slider for parameter 'a'
    public Slider x0Slider; // Reference to the slider for initial value 'x0'
    public Slider iterationsSlider; // Reference to the slider for 'iterations'

    private LineRenderer lineRenderer;

    private void Start()
    {
        // Initialize the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();

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
        // Clear the LineRenderer
        lineRenderer.positionCount = 0;

        float x = x0;

        // Set the position count of the LineRenderer
        lineRenderer.positionCount = iterations;

        // Calculate the scale factor based on the desired line length and iterations
        float scaleFactor = lineLength / (iterations - 1);

        // Calculate and visualize the Period-3 points
        for (int i = 0; i < iterations; i++)
        {
            x = a * x * (1 - x);
            Vector3 currentPoint = new Vector3(i * scaleFactor, x * amplitude, 0f);

            // Set the position of the LineRenderer
            lineRenderer.SetPosition(i, currentPoint);
        }
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