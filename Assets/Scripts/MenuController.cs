using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    public GameObject[] scenes;
    public GameObject mainMenu;
    public Transform cameraAxis;
    public float rotationSpeed = 10f;
    public float manualRotationSpeed = 0.5f;

    private bool isRotating = false;
    private Quaternion initialRotation;
    private bool isDragging = false;
    private Vector3 lastMousePosition;

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
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                
                // Convert delta to rotation axes
                // Note the positive delta.x here for correct horizontal rotation
                Vector3 rotationAxis = (cameraAxis.right * -delta.y + cameraAxis.up * delta.x).normalized;
                float rotationAmount = delta.magnitude * manualRotationSpeed;

                // Apply rotation
                cameraAxis.Rotate(rotationAxis, rotationAmount, Space.World);

                lastMousePosition = Input.mousePosition;
            }
            else
            {
                // Automatic rotation
                cameraAxis.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            }
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
        isDragging = false;
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
    
    private bool IsPointerOverUIElement()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}