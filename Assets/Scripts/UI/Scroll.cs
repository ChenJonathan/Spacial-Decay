using UnityEngine;

/// <summary>
/// Scroll bars that scroll through the level select vertically when moused over.
/// </summary>
public class Scroll : MonoBehaviour
{
    private readonly float CAMERA_SPEED = 2;
    private readonly float CAMERA_MAX_Y = 57.6269100001f;

    /// <summary>
    /// Called periodically. Shifts the camera based on mouse position.
    /// </summary>
    public void Update()
    {
        float border = Screen.height / 4;
        float y = transform.position.y;
        if(Input.mousePosition.y > Screen.height - border)
        {
            float ratio = Mathf.InverseLerp(Screen.height - border, Screen.height, Input.mousePosition.y);
            y = Mathf.Clamp(transform.position.y + ratio * ratio * CAMERA_SPEED, -CAMERA_MAX_Y, CAMERA_MAX_Y);
        }
        else if(Input.mousePosition.y < border)
        {
            float ratio = Mathf.InverseLerp(border, 0, Input.mousePosition.y);
            y = Mathf.Clamp(transform.position.y - ratio * ratio * CAMERA_SPEED, -CAMERA_MAX_Y, CAMERA_MAX_Y);
        }
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}