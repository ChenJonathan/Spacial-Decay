using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A level. Contains one or more waves, which are instantiated sequentially.
/// </summary>
public class Level : MonoBehaviour
{
    public string Scene; // Name of the corresponding scene
    public List<Level> Unlocks; // List of levels to unlock once this level is completed
    
    private SpriteRenderer sprite;
    private LineRenderer line; // Indicates the level that unlocked this level
    private ParticleSystem highlightEffect; // Indicates a newly unlocked level
    private bool clickable; // Whether the corresponding level will load on click or not

    /// <summary>
    /// Called when the object is instantiated. Handles initialization.
    /// </summary>
    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        highlightEffect = GetComponentInChildren<ParticleSystem>();
        highlightEffect.startColor = sprite.color;
        clickable = true;
    }

    /// <summary>
    /// Makes the object gradually appear. Called upon entering level select if the level is newly unlocked.
    /// </summary>
    public void Appear()
    {
        clickable = false;
        StartCoroutine(Appear(1.5f));
    }

    /// <summary>
    /// Coroutine to make the object appear. Also highlights the object with a particle effect afterwards.
    /// </summary>
    /// <param name="duration">How long the object should take to appear</param>
    private IEnumerator Appear(float duration)
    {
        Color color = sprite.color;
        color.a = 0;
        sprite.color = color;
        line.SetColors(color, color);

        for(int i = 1; i <= 100; i++)
        {
            yield return new WaitForSeconds(duration / 100);

            // Increase alpha
            color.a = i / 100.0f;
            sprite.color = color;
            line.SetColors(color, color);
        }

        Highlight();
        clickable = true;
    }

    /// <summary>
    /// Starts the particle effect to indicate that the level is newly unlocked.
    /// </summary>
    public void Highlight()
    {
        highlightEffect.Play();
    }

    /// <summary>
    /// Called when the mouse is on top of the object. Starts the corresponding level on click.
    /// </summary>
    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0) && clickable)
        {
            GameController.Singleton.LoadLevel(this);
        }
    }
}