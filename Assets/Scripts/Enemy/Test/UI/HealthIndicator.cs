using UnityEngine;

/// <summary>
/// A visual display for enemy health. Only appears briefly when the enemy is damaged.
/// </summary>
public class HealthIndicator : MonoBehaviour
{
    [SerializeField]
    private int duration = 3;
    [SerializeField]
    private int fadeSpeed = 3;
    private float remaining;

    private SpriteRenderer bgRenderer;
    private SpriteRenderer fgRenderer;
    private SpriteRenderer edgeRendererL;
    private SpriteRenderer edgeRendererR;

    private float health;

    /// <summary>
    /// Called on initialization (before Start). Handles initialization.
    /// </summary>
    public void Awake()
    {
        health = 1.0f;

        bgRenderer = transform.parent.GetComponent<SpriteRenderer>();
        fgRenderer = GetComponent<SpriteRenderer>();
        edgeRendererL = transform.parent.GetChild(1).GetComponent<SpriteRenderer>();
        edgeRendererR = transform.parent.GetChild(2).GetComponent<SpriteRenderer>();
        
        SetVisibility(0);
    }

    /// <summary>
    /// Updates the display by fading it out over time.
    /// </summary>
    public void Update()
    {
        transform.parent.rotation = Quaternion.identity;
        transform.parent.position = transform.parent.parent.position + new Vector3(0, -1.5f, 0);
        transform.parent.GetChild(1).transform.localScale = transform.parent.GetChild(2).transform.localScale = new Vector3(1 / transform.parent.localScale.x, 1, 1);

        transform.localScale = new Vector3(health, 1, 1);

        if ((remaining -= Time.deltaTime) < 0)
        {
            SetVisibility(Mathf.Lerp(bgRenderer.color.a, 0, -fadeSpeed * remaining));
        }
        else
        {
            bgRenderer.color = new Color((1 - health) / 2, health / 2, 0);
            edgeRendererL.color = bgRenderer.color;
            edgeRendererR.color = bgRenderer.color;
            fgRenderer.color = new Color(1 - health, health, 0);
        }
    }

    /// <summary>
    /// Makes the display fully visible.
    /// </summary>
    /// <param name="healthProportion">Fraction of health remaining</param>
    public void Activate(float healthProportion)
    {
        health = healthProportion;
        SetVisibility(1);
        remaining = duration;
    }

    /// <summary>
    /// Sets the visibility of the display.
    /// </summary>
    /// <param name="value">How visible the display should be</param>
    private void SetVisibility(float value)
    {
        Color bgColor = bgRenderer.color;
        bgRenderer.color = new Color(bgColor.r, bgColor.g, bgColor.b, value);
        edgeRendererL.color = new Color(bgColor.r, bgColor.g, bgColor.b, value);
        edgeRendererR.color = new Color(bgColor.r, bgColor.g, bgColor.b, value);
        Color fgColor = fgRenderer.color;
        fgRenderer.color = new Color(fgColor.r, fgColor.g, fgColor.b, value);
    }
}
