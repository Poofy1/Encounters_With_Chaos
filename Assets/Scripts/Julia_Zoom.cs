using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Julia_Zoom : MonoBehaviour
{
    public RawImage mapImage;
    public float zoomSpeed = 0.5f;
    public float minZoom = .1f;
    public float maxZoom = 1;

    private Vector2 mapSize;
    private float currentZoom = 1f;
    private bool isPanning = false;
    private Vector2 lastMousePosition;

    private bool IsPointerOverUIElement()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0 && results[0].gameObject != mapImage.gameObject;
    }
    
    private void Start()
    {
        mapSize = mapImage.rectTransform.rect.size;
    }
    
    private void OnEnable()
    {
        ResetPosition();
    }

    private void ResetPosition()
    {
        currentZoom = maxZoom;
        mapImage.uvRect = new Rect(Vector2.zero, Vector2.one);
    }

    private void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            currentZoom -= scrollInput * zoomSpeed * currentZoom;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            var mousePosition = Input.mousePosition;

            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            var normalizedMousePosition = new Vector2(
                mousePosition.x / screenSize.x,
                mousePosition.y / screenSize.y
            );

            Vector2 zoomCenter = mapImage.uvRect.position + normalizedMousePosition * mapImage.uvRect.size;
            Vector2 newSize = Vector2.one * currentZoom;
            Vector2 newPosition = zoomCenter - normalizedMousePosition * newSize;

            // Clamp the newPosition to ensure it stays within the bounds of the original image
            newPosition.x = Mathf.Clamp(newPosition.x, 0f, 1f - newSize.x);
            newPosition.y = Mathf.Clamp(newPosition.y, 0f, 1f - newSize.y);

            // Check if the zoom level is at the maximum zoom out
            if (currentZoom == maxZoom)
            {
                // Set the newPosition to (0, 0) to align with the top-left corner of the original image
                newPosition = Vector2.zero;
            }

            mapImage.uvRect = new Rect(
                newPosition,
                newSize
            );
        }

        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
        {
            isPanning = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector2 mouseDelta = (Vector2)Input.mousePosition - lastMousePosition;
            mouseDelta *= currentZoom;
            Vector2 uvDelta = new Vector2(
                mouseDelta.x / mapImage.rectTransform.rect.width,
                mouseDelta.y / mapImage.rectTransform.rect.height
            );

            Vector2 newPosition = mapImage.uvRect.position - uvDelta;

            // Clamp the newPosition to ensure it stays within the bounds of the original image
            newPosition.x = Mathf.Clamp(newPosition.x, 0f, 1f - mapImage.uvRect.width);
            newPosition.y = Mathf.Clamp(newPosition.y, 0f, 1f - mapImage.uvRect.height);

            mapImage.uvRect = new Rect(
                newPosition,
                mapImage.uvRect.size
            );

            lastMousePosition = Input.mousePosition;
        }
    }
}