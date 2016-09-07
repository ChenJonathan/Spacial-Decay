using UnityEngine;
using System.Collections;

public class HealthIndicator : MonoBehaviour {
    
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

    private bool active;
    
    void Awake()
    {
        health = 1.0f;

        bgRenderer = transform.parent.GetComponent<SpriteRenderer>();
        fgRenderer = GetComponent<SpriteRenderer>();
        edgeRendererL = transform.parent.GetChild(1).GetComponent<SpriteRenderer>();
        edgeRendererR = transform.parent.GetChild(2).GetComponent<SpriteRenderer>();
        
        SetVisibility(0);
    }
	
	void Update()
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

    public void Activate(float healthProportion)
    {
        health = healthProportion;
        SetVisibility(1);
        remaining = duration;
    }

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
