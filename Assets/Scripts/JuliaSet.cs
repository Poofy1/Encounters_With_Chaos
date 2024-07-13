using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JuliaSet : MonoBehaviour
{
    
    public RawImage rend;
    private Material mat;
    
    public Slider xSlider;
    public Slider ySlider;
    public Slider iterSlider;

    private bool isAnimating;
    
    void Start()
    {
        // Create a new instance of the material
        mat = new Material(rend.material);
        rend.material = mat;
    }
    
    
    public void ChangeX()
    {
        mat.SetFloat("_CurveScaleX", xSlider.value);
    }
    
    public void ChangeY()
    {
        mat.SetFloat("_CurveScaleY", ySlider.value);
    }
    
    public void ChangeIter()
    {
        mat.SetFloat("_Iterations", iterSlider.value);
    }
    
    public void Randomize()
    {
        if (!isAnimating)
        {
            StartCoroutine(AnimateRandomize());
        }
    }

    private IEnumerator AnimateRandomize()
    {
        isAnimating = true;

        // Disable slider interactions during animation
        xSlider.interactable = false;
        ySlider.interactable = false;

        // Store the current slider values
        float startA = xSlider.value;
        float startB = ySlider.value;

        // Generate random target values for the sliders
        float targetA = Random.Range(xSlider.minValue, .5f);
        float targetB = Random.Range(ySlider.minValue, ySlider.maxValue);

        float elapsedTime = 0f;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / 1);

            // Apply the custom interpolation function
            float interpolationFactor = Mathf.Sin(t * Mathf.PI * 0.5f);

            // Interpolate slider values smoothly
            xSlider.value = Mathf.Lerp(startA, targetA, interpolationFactor);
            ySlider.value = Mathf.Lerp(startB, targetB, interpolationFactor);

            // Update shader values
            ChangeX();
            ChangeY();

            yield return null;
        }

        // Ensure the final values are set precisely
        xSlider.value = targetA;
        ySlider.value = targetB;

        // Update shader values one last time
        ChangeX();
        ChangeY();

        // Re-enable slider interactions after animation
        xSlider.interactable = true;
        ySlider.interactable = true;

        isAnimating = false;
    }
}
