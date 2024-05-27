using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CantorSets : MonoBehaviour
{
    
    public RawImage rend;
    private Material mat => rend.material;
    
    public Slider widthSlider;
    public Slider iterSlider;
    

    void Start()
    {
        
    }
    
    
    public void ChangeWidth()
    {
        mat.SetFloat("_LineWidth", widthSlider.value / 20);
    }
    
    public void ChangeIter()
    {
        mat.SetFloat("_Iterations", iterSlider.value);
    }
}
