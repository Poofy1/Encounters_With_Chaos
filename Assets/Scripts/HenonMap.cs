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
    public Slider iterSlider;
    
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
    
    public void ChangeIter()
    {
        mat.SetFloat("_Iterations", iterSlider.value);
    }
}
