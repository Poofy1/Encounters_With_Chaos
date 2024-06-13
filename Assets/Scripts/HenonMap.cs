using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HenonMap : MonoBehaviour
{
    public RawImage rend;
    private Material mat;
    
    public Slider aSlider;
    public Slider bSlider;
    public Slider cSlider;
    public Slider dSlider;
    public Slider iterSlider;

    public float animationDuration = 1f; // Duration of the animation in seconds
    public float animationFalloff = 2f; // Exponential falloff factor for the animation

    private bool isAnimating = false;
    
    void Start()
    {
        // Create a new instance of the material
        mat = new Material(rend.material);
        rend.material = mat;
    }
    
    public void ChangeA()
    {
        mat.SetFloat("_A", aSlider.value);
    }
    
    public void ChangeB()
    {
        mat.SetFloat("_B", bSlider.value);
    }
    
    public void ChangeC()
    {
        mat.SetFloat("_C", cSlider.value);
    }
    
    public void ChangeD()
    {
        mat.SetFloat("_D", dSlider.value);
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
        aSlider.interactable = false;
        bSlider.interactable = false;
        cSlider.interactable = false;
        dSlider.interactable = false;

        // Store the current slider values
        float startA = aSlider.value;
        float startB = bSlider.value;
        float startC = cSlider.value;
        float startD = dSlider.value;

        // Generate random target values for the sliders
        float targetA = Random.Range(aSlider.minValue, aSlider.maxValue);
        float targetB = Random.Range(bSlider.minValue, bSlider.maxValue);
        float targetC = Random.Range(cSlider.minValue, cSlider.maxValue);
        float targetD = Random.Range(dSlider.minValue, dSlider.maxValue);

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);

            // Apply the custom interpolation function
            float interpolationFactor = Mathf.Sin(t * Mathf.PI * 0.5f);

            // Interpolate slider values smoothly
            aSlider.value = Mathf.Lerp(startA, targetA, interpolationFactor);
            bSlider.value = Mathf.Lerp(startB, targetB, interpolationFactor);
            cSlider.value = Mathf.Lerp(startC, targetC, interpolationFactor);
            dSlider.value = Mathf.Lerp(startD, targetD, interpolationFactor);

            // Update shader values
            ChangeA();
            ChangeB();
            ChangeC();
            ChangeD();

            yield return null;
        }

        // Ensure the final values are set precisely
        aSlider.value = targetA;
        bSlider.value = targetB;
        cSlider.value = targetC;
        dSlider.value = targetD;

        // Update shader values one last time
        ChangeA();
        ChangeB();
        ChangeC();
        ChangeD();

        // Re-enable slider interactions after animation
        aSlider.interactable = true;
        bSlider.interactable = true;
        cSlider.interactable = true;
        dSlider.interactable = true;

        isAnimating = false;
    }
}