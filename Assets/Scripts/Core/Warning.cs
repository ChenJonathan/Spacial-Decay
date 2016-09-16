using UnityEngine;

public class Warning : MonoBehaviour
{
    // Total duration of the warning, including fade-in / fade-out
    private float duration;
    public float Duration
    {
        get { return duration; }
        set { duration = value; }
    }
    // Amount of time the warning will spend fading in
    private float fadeInDuration;
    public float FadeInDuration
    {
        get { return fadeInDuration; }
        set { fadeInDuration = value; }
    }
    // Amount of time the warning will spend fading out
    private float fadeOutDuration;
    public float FadeOutDuration
    {
        get { return fadeOutDuration; }
        set { fadeOutDuration = value; }
    }

    private SpriteRenderer renderer2d;
    private float time = 0;

    public virtual void Start()
    {
        renderer2d = GetComponent<SpriteRenderer>();
        Color color = renderer2d.color;
        color.a = 0;
        renderer2d.color = color;
    }

    public virtual void Update()
    {
        time += Time.deltaTime;
        if(time > duration)
            Destroy(gameObject);
        else if(time > duration - fadeOutDuration)
        {
            Color color = renderer2d.color;
            color.a = Mathf.Lerp(1, 0, (time - (duration - fadeOutDuration)) / fadeOutDuration);
            renderer2d.color = color;
        }
        else if(time < fadeInDuration)
        {
            Color color = renderer2d.color;
            color.a = Mathf.Lerp(0, 1, time / fadeInDuration);
            renderer2d.color = color;
        }

    }
}