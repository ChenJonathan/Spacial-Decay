using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField]
    private int scrollSpeed = 2;
    [SerializeField]
    private float duration = 1;
    private float remaining;
    
    private MeshRenderer damageRenderer;

    public bool Paused
    {
        get;
        set;
    }

    void Start()
    {
        transform.Translate(new Vector2(Random.insideUnitCircle.x * 0.7f, 0));
        damageRenderer = gameObject.GetComponent<MeshRenderer>();
        remaining = duration;
    }

	void Update()
    {
        transform.position += new Vector3(0, scrollSpeed * Mathf.Lerp(0, remaining / duration, Time.deltaTime), 0); 
        Color c = damageRenderer.material.color;
        damageRenderer.material.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0, 1, remaining)); // Fade away effect
        
        if ((remaining-=Time.deltaTime) <= 0)
        {
            Destroy(gameObject);
        }
    }
}
