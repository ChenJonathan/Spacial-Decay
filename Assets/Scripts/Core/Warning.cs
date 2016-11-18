using UnityEngine;

/// <summary>
/// A sprite that warns the player of incoming enemies or bullets.
/// </summary>
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

    // Renders the warning sprite
    private SpriteRenderer renderer2d;

    // Time elapsed since the warning was instantiated
    private float time = 0;

    /// <summary>
    /// Called when the warning is instantiated. Handles initialization.
    /// </summary>
    public virtual void Start()
    {
        renderer2d = GetComponent<SpriteRenderer>();
        Color color = renderer2d.color;
        color.a = (fadeInDuration <= 0) ? 1 : 0;
        renderer2d.color = color;
    }

    /// <summary>
    /// Called periodically. Handles fading in, fading out, and destroying the warning when time is up.
    /// </summary>
    /// <param name="warning">The warning prefab to spawn</param>
    /// <returns>The warning that was spawned</returns>
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