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
    public Toggle animToggle;
    public Button randButton;

    public float animationDuration = 1f; // Duration of the animation in seconds
    public float animationFalloff = 2f; // Exponential falloff factor for the animation

    private bool isAnimating = false;
    private Coroutine animationCoroutine;
    
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
        animToggle.interactable = false;

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
        animToggle.interactable = true;

        isAnimating = false;
    }
    
    
    
    
    

    public void ToggleAnimation()
    {
        isAnimating = animToggle.isOn;

        if (isAnimating)
        {
            // Disable slider interactions except for iterations
            aSlider.interactable = false;
            bSlider.interactable = false;
            cSlider.interactable = false;
            dSlider.interactable = false;
            randButton.interactable = false;

            // Start the animation coroutine
            animationCoroutine = StartCoroutine(AnimateSliders());
        }
        else
        {
            // Enable slider interactions
            aSlider.interactable = true;
            bSlider.interactable = true;
            cSlider.interactable = true;
            dSlider.interactable = true;
            randButton.interactable = true;

            // Stop the animation coroutine if it's running
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
        }
    }
    
    private float[] targets = new float[4];
    private IEnumerator AnimateSliders()
    {
        targets = new float[4];
        targets[0] = Random.Range(aSlider.minValue, aSlider.maxValue);
        targets[1] = Random.Range(bSlider.minValue, bSlider.maxValue);
        targets[2] = Random.Range(cSlider.minValue, cSlider.maxValue);
        targets[3] = Random.Range(dSlider.minValue, dSlider.maxValue);

        while (isAnimating)
        {
            // Generate new targets and speeds for sliders that have reached their targets
            if (Mathf.Approximately(aSlider.value, targets[0]))
                SetNewTarget(0, aSlider);
            if (Mathf.Approximately(bSlider.value, targets[1]))
                SetNewTarget(1, bSlider);
            if (Mathf.Approximately(cSlider.value, targets[2]))
                SetNewTarget(2, cSlider);
            if (Mathf.Approximately(dSlider.value, targets[3]))
                SetNewTarget(3, dSlider);

            float speed = .2f * Time.deltaTime;

            // Move sliders towards their targets
            aSlider.value = Mathf.MoveTowards(aSlider.value, targets[0], speed);
            bSlider.value = Mathf.MoveTowards(bSlider.value, targets[1], speed);
            cSlider.value = Mathf.MoveTowards(cSlider.value, targets[2], speed);
            dSlider.value = Mathf.MoveTowards(dSlider.value, targets[3], speed);

            // Update shader values
            ChangeA();
            ChangeB();
            ChangeC();
            ChangeD();

            yield return null;
        }
    }

    private void SetNewTarget(int index, Slider slider)
    {
        targets[index] = Random.Range(slider.minValue, slider.maxValue);
    }
}