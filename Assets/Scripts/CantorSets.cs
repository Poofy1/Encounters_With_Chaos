using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CantorSets : MonoBehaviour
{
    public Renderer rend;
    private Material mat;
    
    public Slider iterSlider;
    public Slider lightSlider;
    public Slider curveSlider;
    public Slider speedSlider;
    
    private bool isAnimating;

    private float _LightState;
    private float _LightSpeed;

    void Start()
    {
        // Create a new instance of the material
        mat = new Material(rend.material);
        rend.material = mat;

        ChangeIter();
        ChangeLight();
        ChangeCurves();
        ChangeSpeed();
    }

    void Update()
    {
        _LightState += Time.deltaTime * _LightSpeed * .5f;
        mat.SetFloat("_LightState", _LightState);
    }
    
    
    public void ChangeIter()
    {
        mat.SetFloat("_Iterations", iterSlider.value);
    }
    
    public void ChangeLight()
    {
        mat.SetFloat("_LightDistance", lightSlider.value * 450 + 50);
    }
    
    public void ChangeCurves()
    {
        mat.SetFloat("_Roundness", curveSlider.value * 5);
    }
    
    public void ChangeSpeed()
    {
        _LightSpeed = speedSlider.value;
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
        lightSlider.interactable = false;
        curveSlider.interactable = false;
        speedSlider.interactable = false;

        // Store the current slider values
        float startA = lightSlider.value;
        float startB = curveSlider.value;
        float startC = speedSlider.value;

        // Generate random target values for the sliders
        float targetA = Random.Range(lightSlider.minValue, lightSlider.maxValue);
        float targetB = Random.Range(curveSlider.minValue, curveSlider.maxValue);
        float targetC = Random.Range(speedSlider.minValue, speedSlider.maxValue);

        float elapsedTime = 0f;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / 1);

            // Apply the custom interpolation function
            float interpolationFactor = Mathf.Sin(t * Mathf.PI * 0.5f);

            // Interpolate slider values smoothly
            lightSlider.value = Mathf.Lerp(startA, targetA, interpolationFactor);
            curveSlider.value = Mathf.Lerp(startB, targetB, interpolationFactor);
            speedSlider.value = Mathf.Lerp(startC, targetC, interpolationFactor);

            // Update shader values
            ChangeLight();
            ChangeCurves();
            ChangeSpeed();

            yield return null;
        }

        // Ensure the final values are set precisely
        lightSlider.value = targetA;
        curveSlider.value = targetB;
        speedSlider.value = targetC;

        // Update shader values one last time
        ChangeLight();
        ChangeCurves();
        ChangeSpeed();

        // Re-enable slider interactions after animation
        lightSlider.interactable = true;
        curveSlider.interactable = true;
        speedSlider.interactable = true;

        isAnimating = false;
        
        
    }
}