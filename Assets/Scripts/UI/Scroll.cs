using UnityEngine;

/// <summary>
/// Scroll bars that scroll through the level select vertically when moused over.
/// </summary>
public class Scroll : MonoBehaviour
{
    [HideInInspector]
    public bool Zoom;
    [HideInInspector]
    public float CameraSpeed;
    public float CameraMinY;
    public float CameraMaxY;
    public float CameraMaxZ;

    public readonly float CAMERA_ACCELERATION = 0.3f;
    public readonly float CAMERA_SPEED_DECAY = 0.9f;

    /// <summary>
    /// Called periodically. Shifts the camera based on mouse position.
    /// </summary>
    public void FixedUpdate()
    {
        if(Zoom)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(transform.position.z, CameraMaxZ, 0.1f));
        }
        else if(transform.position.z != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(transform.position.z, 0, 0.1f));
            if(transform.position.z < 0.1f)
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
        else
        {
            // Calculating new camera speed
            float border = Screen.height / 6;
            if(Input.mousePosition.y > Screen.height - border)
            {
                float ratio = Mathf.InverseLerp(Screen.height - border, Screen.height, Input.mousePosition.y);
                CameraSpeed += ratio * ratio * CAMERA_ACCELERATION;
            }
            else if(Input.mousePosition.y < border + Screen.height / 10 && Input.mousePosition.y > Screen.height / 10)
            {
                float ratio = Mathf.InverseLerp(border + Screen.height / 10, Screen.height / 10, Input.mousePosition.y);
                CameraSpeed -= ratio * ratio * CAMERA_ACCELERATION;
            }

            CameraSpeed *= CAMERA_SPEED_DECAY;

            // Scroll the view
            ScrollTo(transform.position.y + CameraSpeed);
        }
    }

    public void ScrollTo(float y)
    {
        if (y > CameraMaxY)
        {
            y = CameraMaxY;
            CameraSpeed = 0;
        }

        else if (y < CameraMinY)
        {
            y = CameraMinY;
            CameraSpeed = 0;
        }

        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}