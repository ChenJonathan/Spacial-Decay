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

    private Image details;                          // A reference to the level's details
    private Image center;                           // A reference to the level's center image
    private Text titleText;                         // A reference to the level's title text
    private Text ordinalText;                       // A reference to the level's ordinal text

    public Sprite[] sprites;                        // Sprites to use for the level select

    [HideInInspector]
    public LineRenderer line;                       // Indicates the level that unlocked this level
    private ParticleSystem highlightEffect;         // Indicates a newly unlocked level
    private const float APPEAR_DURATION = 0.5f;     // The amount of time taken for a level to fade in.

    // Variables that control the look of the UI elements.

    private float scale = 0f;
    private float scaleTarg = 0f;
    private float scaleDrag = 0.25f;
    private readonly float scaleWhenFirstHovered = 1.3f;
    private readonly float scaleTargWhenDefault = 1.0f;
    private readonly float scaleTargWhenHovered = 1.15f;

    private float expand = 0f;
    private float expandTarg = 0f;
    private float expandDrag = 0.5f;

    /// <summary>
    /// Called when the object is instantiated. Handles initialization.
    /// </summary>
    public void Awake()
    {
        details = transform.Find("Details").GetComponent<Image>();
        center = transform.Find("Center").GetComponent<Image>();

        titleText = transform.Find("Details/TitleText").GetComponent<Text>();
        titleText.text = gameObject.name;
        
        ordinalText = transform.Find("Center/OrdinalText").GetComponent<Text>();
        ordinalText.text = transform.GetSiblingIndex().ToString();

        highlightEffect = transform.Find("HighlightEffect").GetComponent<ParticleSystem>();
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
        Color color = Color.white;
        color.a = 0;

        if (!lineOnly) {
            details.color = color;
            titleText.color = color;
            center.color = color;
            ordinalText.color = color;
        }
        if (line != null) {
            line.SetColors(color, color);
            line.enabled = true;
        }

        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(duration / 100.0f);
            color.a = i / 100.0f;

            if (!lineOnly) {
                details.color = color;
                titleText.color = color;
                center.color = color;
                ordinalText.color = color;
            }
            if (line != null) {
                line.SetColors(color, color);
            }
        }

        if (!lineOnly)
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

        center.sprite = sprites[(int) expandTarg];
        center.transform.localScale = Vector3.one * scale;
        details.transform.localScale = Vector3.one * scale * expand;

        scaleTarg = scaleTargWhenDefault;
        expandTarg = 0;
    }

    /// <summary>
    /// Called when the mouse is on top of the object. Starts the corresponding level on click.
    /// </summary>
    private void OnMouseOver()
    {
        scaleTarg = scaleTargWhenHovered;
        expandTarg = 1;

        if (Input.GetMouseButtonDown(0))
        {
            GameController.Singleton.LoadLevel(Scene);
        }
    }
}