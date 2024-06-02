using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject[] scenes;
    public GameObject mainMenu;
    public Transform cameraAxis;
    public float rotationSpeed = 10f;

    private bool isRotating = false;
    private Quaternion initialRotation;

    private void Start()
    {
        mainMenu.SetActive(true);
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i].SetActive(false);
        }
        initialRotation = cameraAxis.rotation;
    }

    private void Update()
    {
        if (isRotating)
        {
            cameraAxis.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
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

    public void EnableCameraRotation()
    {
        isRotating = true;
    }

    public void DisableCameraRotation()
    {
        isRotating = false;
        cameraAxis.rotation = initialRotation;
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}