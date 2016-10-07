using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Scroll bars that scroll through the level select vertically when moused over.
/// </summary>
public class Scroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Scroll direction
    public bool Up;

    private Camera mainCamera;
    private bool mouseOver;
    private float currentCameraSpeed;

    private readonly float CAMERA_START_SPEED = 0.2f;
    private readonly float CAMERA_ACCELERATION = 0.4f;
    private readonly float CAMERA_MAX_Y = 57.6269100001f;

    /// <summary>
    /// Called on scroll bar instantiation. Handles initialization.
    /// </summary>
    public void Awake()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        mouseOver = false;
        currentCameraSpeed = CAMERA_START_SPEED;
    }

    /// <summary>
    /// Called periodically. Shifts the camera and increases the scroll speed over time if moused over.
    /// </summary>
    public void Update()
    {
        if(mouseOver)
        {
            mainCamera.transform.position = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.y + (Up ? currentCameraSpeed : -currentCameraSpeed));

            if(mainCamera.transform.position.y < -CAMERA_MAX_Y)
                mainCamera.transform.position = new Vector2(mainCamera.transform.position.x, -CAMERA_MAX_Y);
            else if(mainCamera.transform.position.y > CAMERA_MAX_Y)
                mainCamera.transform.position = new Vector2(mainCamera.transform.position.x, CAMERA_MAX_Y);

            currentCameraSpeed += CAMERA_ACCELERATION * Time.deltaTime;
        }
    }

    /// <summary>
    /// Called when the mouse first hovers over the scroll bar. Resets the scroll speed.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        currentCameraSpeed = CAMERA_START_SPEED;
    }

    /// <summary>
    /// Called when the mouse leaves the scroll bar.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }
}