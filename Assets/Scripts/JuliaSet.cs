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
}
