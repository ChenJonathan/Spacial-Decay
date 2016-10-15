using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelIcon : MonoBehaviour
{
    public Sprite normal;
    public Sprite hovered;
    private Image image;

    bool isHovered = false;
    float rotX = 0f;
    float rotY = 0f;
    float rotDrag = 0.1f;
    float rotAmount = 1f;
    float size = 1f;
    float sizeHover = 0.2f;
    float sizeDrag = 0.25f;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        // Get mouse position in world space to detect if this object is being hovered over

        bool wasHovered = isHovered;
        Vector3 input = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
        Vector2 mouse = Camera.main.ScreenToWorldPoint(input);
        isHovered = (Vector2.Distance(transform.position, mouse) < 1.5f);

        // Fun rotation to the pointer

        float x = (mouse.x - transform.position.x) * rotAmount;
        float y = (mouse.y - transform.position.y) * rotAmount;
        rotX += (x - rotX) * rotDrag;
        rotY += (y - rotY) * rotDrag;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position, Vector3.up) * Quaternion.Euler(rotY, -rotX, 0);
        
        // Animations for hovering

        if (!wasHovered && isHovered)
            size = 1 + sizeHover * 2;
        size += ((isHovered ? 1 + sizeHover : 1) - size) * sizeDrag;
        transform.localScale = Vector3.one * size;
        image.sprite = (isHovered ? hovered : normal);
    }
}