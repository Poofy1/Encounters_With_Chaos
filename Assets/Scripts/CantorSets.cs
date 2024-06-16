using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CantorSets : MonoBehaviour
{
    public Renderer rend;
    private Material mat;
    
    public Slider widthSlider;
    public Slider iterSlider;
    
    private Camera cam;
    private bool isPerspective;

    void Start()
    {
        // Create a new instance of the material
        mat = new Material(rend.material);
        rend.material = mat;
        
        cam = Camera.main;
        isPerspective = false;

        ChangeIter();
    }
    
    void OnEnable()
    {
        // Switch to perspective mode when the script is enabled
        if (cam != null && !isPerspective)
        {
            cam.orthographic = false;
            isPerspective = true;
        }
    }
    
    void OnDisable()
    {
        // Switch back to orthographic mode when the script is disabled
        if (cam != null && isPerspective)
        {
            cam.orthographic = true;
            isPerspective = false;
        }
    }
    
    
    public void ChangeIter()
    {
        mat.SetFloat("_Iterations", iterSlider.value);
    }
}