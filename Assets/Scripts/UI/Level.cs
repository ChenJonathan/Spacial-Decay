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
    [HideInInspector]
    public LineRenderer line; // Indicates the level that unlocked this level
    private ParticleSystem highlightEffect; // Indicates a newly unlocked level

    /// <summary> The amount of time taken for a level to fade in. </summary>
    private const float APPEAR_DURATION = 1.5f;

    /// <summary>
    /// Called when the object is instantiated. Handles initialization.
    /// </summary>
    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        highlightEffect = GetComponentInChildren<ParticleSystem>();
        highlightEffect.startColor = sprite.color;
        if (Scene == "") {
            Scene = gameObject.name;
        }
    }

    /// <summary>
    /// Makes the object gradually appear. Called upon entering level select if the level is newly unlocked.
    /// </summary>
    public void Appear()
    {
        StartCoroutine(Appear(APPEAR_DURATION));
    }

    /// <summary>
    /// Makes the most recent line to the level appear.
    /// </summary>
    public void LineAppear()
    {
        StartCoroutine(Appear(APPEAR_DURATION, true));
    }

    /// <summary>
    /// Coroutine to make the object appear. Also highlights the object with a particle effect afterwards.
    /// </summary>
    /// <param name="duration">How long the object should take to appear</param>
    private IEnumerator Appear(float duration, bool lineOnly = false)
    {
        Color color = sprite.color;
        color.a = 0;
        if (!lineOnly) {
            sprite.color = color;
        }
        if (line != null) {
            line.SetColors(color, color);
            line.enabled = true;
        }

        for(int i = 1; i <= 100; i++)
        {
            yield return new WaitForSeconds(duration / 100);

            // Increase alpha
            color.a = i / 100.0f;
            if (!lineOnly) {
                sprite.color = color;
            }
            if (line != null) {
                line.SetColors(color, color);
            }
        }

        Highlight();
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
        if(Input.GetMouseButtonDown(0))
        {
            if(!DifficultySelect.Instance.gameObject.activeSelf || DifficultySelect.Instance.Level != this)
                DifficultySelect.Instance.Activate(this);
            else
                DifficultySelect.Instance.Deactivate();
        }
    }
}