using UnityEngine;

/// <summary>
/// Scroll bars that scroll through the level select vertically when moused over.
/// </summary>
public class Scroll : MonoBehaviour
{
    private float cameraSpeed;
    private float cameraMaxY;

    private readonly float CAMERA_ACCELERATION = 0.1f;
    private readonly float CAMERA_SPEED_DECAY = 0.2f;

    /// <summary>
    /// Initializes camera values.
    /// </summary>
    public void Start()
    {
        GetComponent<Camera>().fieldOfView = 2.0f * Mathf.Atan(419.84f / GetComponent<Camera>().aspect / 400f) * Mathf.Rad2Deg;
        cameraMaxY = 419.84f / 2f - 419.84f / GetComponent<Camera>().aspect / 2f;
        transform.position = new Vector3(transform.position.x, -cameraMaxY, transform.position.z);
    }

    /// <summary>
    /// Called periodically. Shifts the camera based on mouse position.
    /// </summary>
    public void FixedUpdate()
    {
        // Calculating new camera speed
        float border = Screen.height / 4;
        if(Input.mousePosition.y > Screen.height - border)
        {
            float ratio = Mathf.InverseLerp(Screen.height - border, Screen.height, Input.mousePosition.y);
            cameraSpeed += ratio * ratio * CAMERA_ACCELERATION;
        }
        else if(Input.mousePosition.y < border)
        {
            float ratio = Mathf.InverseLerp(border, 0, Input.mousePosition.y);
            cameraSpeed -= ratio * ratio * CAMERA_ACCELERATION;
        }
        else
        {
            cameraSpeed = Mathf.Lerp(cameraSpeed, 0, CAMERA_SPEED_DECAY);
        }

        // Making sure camera stays in bounds
        float y = transform.position.y + cameraSpeed;
        if(y > cameraMaxY)
        {
            y = cameraMaxY;
            cameraSpeed = 0;
        }
        else if(y < -cameraMaxY)
        {
            y = -cameraMaxY;
            cameraSpeed = 0;
        }
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}