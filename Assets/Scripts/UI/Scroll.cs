using UnityEngine;
using UnityEngine.UI;
using DanmakU;
using System.Collections;

public class Scroll : MonoBehaviour
{
    public bool Up;

    private Camera mainCamera;
    private float currentCameraSpeed;

    private readonly float CAMERA_START_SPEED = 0.2f;
    private readonly float CAMERA_ACCELERATION = 0.2f;
    private readonly float CAMERA_MAX_Y = 57.6269100001f;

    public void Awake()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        currentCameraSpeed = CAMERA_START_SPEED;
    }

    public void OnMouseEnter()
    {
        currentCameraSpeed = CAMERA_START_SPEED;
    }

    public void OnMouseOver()
    {
        mainCamera.transform.position = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.y + (Up ? currentCameraSpeed : -currentCameraSpeed));
        
        if(mainCamera.transform.position.y < -CAMERA_MAX_Y)
            mainCamera.transform.position = new Vector2(mainCamera.transform.position.x, -CAMERA_MAX_Y);
        else if(mainCamera.transform.position.y > CAMERA_MAX_Y)
            mainCamera.transform.position = new Vector2(mainCamera.transform.position.x, CAMERA_MAX_Y);

        currentCameraSpeed += CAMERA_ACCELERATION * Time.deltaTime;
    }
}