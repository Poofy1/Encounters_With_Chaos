using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject[] scenes;
    public GameObject mainMenu;

    private void Start()
    {
        mainMenu.SetActive(true);
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i].SetActive(false);
        }
    }

    public void SceneSelect(int scene)
    {
        mainMenu.SetActive(false);
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i].SetActive(i == scene);
        }
    }
    
    public void ReturnMain()
    {
        mainMenu.SetActive(true);
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i].SetActive(false);
        }
    }
}
