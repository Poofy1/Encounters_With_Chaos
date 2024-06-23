using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CantorSets : MonoBehaviour
{
    public Renderer rend;
    private Material mat;
    
    public Slider iterSlider;
    

    void Start()
    {
        // Create a new instance of the material
        mat = new Material(rend.material);
        rend.material = mat;

        ChangeIter();
    }
    
    
    public void ChangeIter()
    {
        mat.SetFloat("_Iterations", iterSlider.value);
    }
}