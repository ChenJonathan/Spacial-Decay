using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A level. Contains one or more waves, which are instantiated sequentially.
/// </summary>
public class Level : MonoBehaviour
{
    public string Scene;                            // Name of the corresponding scene
    public List<Level> Unlocks;                     // List of levels to unlock once this level is completed

    private Image image;                            // A reference to the level image
    private Text ordinalText;                       // A reference to the level's ordinal text

    public Sprite[] sprites;
    [HideInInspector]
    public LineRenderer line;                       // Indicates the level that unlocked this level
    private ParticleSystem highlightEffect;         // Indicates a newly unlocked level
    private const float APPEAR_DURATION = 0.5f;     // The amount of time taken for a level to fade in.

    // Variables that control the look of the UI elements.

    float scale = 0f;
    float scaleWhenFirstHovered = 1.3f;
    float scaleTarg = 0f;
    float scaleTargWhenDefault = 1.0f;
    float scaleTargWhenHovered = 1.15f;
    float scaleDrag = 0.25f;

    float expand = 0f;
    float expandTarg = 0f;
    float expandDrag = 0.25f;

    /// <summary>
    /// Called when the object is instantiated. Handles initialization.
    /// </summary>
    public void Awake()
    {
        image = transform.Find("UIElements/Image").GetComponent<Image>();
        ordinalText = transform.Find("UIElements/Image/OrdinalText").GetComponent<Text>();
        highlightEffect = transform.Find("HighlightEffect").GetComponent<ParticleSystem>();
        image.
        ordinalText.text = transform.GetSiblingIndex().ToString();

        highlightEffect.startColor = image.color;
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
        Color color = image.color;
        color.a = 0;
        /*if (!lineOnly) {
            image.color = color;
        }*/
        if (line != null) {
            line.SetColors(color, color);
            line.enabled = true;
        }

        for(int i = 1; i <= 100; i++)
        {
            yield return new WaitForSeconds(duration / 100);

            // Increase alpha
            color.a = i / 100.0f;
            /*if (!lineOnly) {
                image.color = color;
            }*/
            if (line != null) {
                line.SetColors(color, color);
            }
        }

        if(!lineOnly)
            Highlight();
    }

    /// <summary>
    /// Starts the particle effect to indicate that the level is newly unlocked.
    /// </summary>
    public void Highlight()
    {
        highlightEffect.Play();
    }

    private void OnMouseEnter()
    {
        scale = scaleWhenFirstHovered;
    }

    void Update()
    {
        scale += (scaleTarg - scale) * scaleDrag;
        expand += (expandTarg - expand) * expandDrag;

        scaleTarg = scaleTargWhenDefault;
        expandTarg = 0;

        image.transform.parent.localScale = Vector3.one * scale;
    }

    /// <summary>
    /// Called when the mouse is on top of the object. Starts the corresponding level on click.
    /// </summary>
    private void OnMouseOver()
    {
        scaleTarg = scaleTargWhenHovered;
        expandTarg = 1;

        if(Input.GetMouseButtonDown(0))
        {
            GameController.Singleton.LoadLevel(Scene);
            if (!DifficultySelect.Instance.gameObject.activeSelf || DifficultySelect.Instance.Level != this)
                DifficultySelect.Instance.Activate(this);
            else
                DifficultySelect.Instance.Deactivate();
        }
    }
}