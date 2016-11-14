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
    private Text newText;                           // A reference to the level's "NEW!" text

    public Sprite[] sprites;                        // Sprites to use for the level select

    private AudioSource audioSource;
    public AudioClip onHoverAudio;
    public AudioClip onClickAudio;

    [HideInInspector]
    public LineRenderer line;                       // Indicates the level that unlocked this level

    // Variables that control the look of the UI elements.

    private float scale = 0f;
    private float scaleTarg = 0f;
    private float scaleDrag = 0.25f;
    private readonly float scaleWhenFirstHovered = 1.3f;
    private readonly float scaleTargWhenDefault = 1.0f;
    private readonly float scaleTargWhenHovered = 1.15f;
    
    float expand = 0f;
    float expandTarg = 0f;
    float expandDrag = 0.33f;

    float nText = 0;
    float nTextTarg = 0;
    float nTextVel = 0;
    float nTextAccel = 0.1f;
    float nTextDrag = 0.9f;
    Color nTextCol = Color.yellow;

    /// <summary>
    /// Called when the object is instantiated. Handles initialization.
    /// </summary>
    public void Awake()
    {
        details = transform.FindChild("Details").GetComponent<Image>();
        center = transform.FindChild("Center").GetComponent<Image>();

        titleText = transform.FindChild("Details/TitleText").GetComponent<Text>();
        titleText.text = gameObject.name;
        
        ordinalText = transform.FindChild("Center/OrdinalText").GetComponent<Text>();
        ordinalText.text = transform.GetSiblingIndex().ToString();
        
        newText = transform.FindChild("Center/NewText").GetComponent<Text>();

        center.transform.localScale = Vector3.zero;
        details.transform.localScale = Vector3.zero;

        audioSource = GetComponent<AudioSource>();

        if (Scene == "")
        {
            Scene = gameObject.name;
        }
    }

    /// <summary>
    /// Makes the object gradually appear. Called upon entering level select if the level is newly unlocked.
    /// </summary>
    public void Appear()
    {
        StartCoroutine(Appear(0.5f));
    }

    /// <summary>
    /// Makes the most recent line to the level appear.
    /// </summary>
    public void LineAppear()
    {
        StartCoroutine(Appear(0.5f, true));
    }

    /// <summary>
    /// Coroutine to make the object appear.
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

        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForSeconds(duration / 30.0f);
            color.a = Mathf.Pow(i / 30.0f, 0.25f);

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
    /// Marks the level as a new level.
    /// </summary>
    public void Highlight()
    {
        nTextTarg = 1;
        nTextCol = Color.yellow;
    }

    /// <summary>
    /// A whole bunch of procedural animations.
    /// </summary>
    public void Update()
    {
        scale += (scaleTarg - scale) * scaleDrag;
        expand += (expandTarg - expand) * expandDrag;
        nTextVel += (nTextTarg - nText) * nTextAccel;
        nTextVel *= nTextDrag;
        nText += nTextVel;
        nTextCol = Color.Lerp(nTextCol, Color.white, 0.05f);

        if (expandTarg == 0 && nTextTarg == 1 && (Time.time % 3) < 0.05f)
        {
            nText = 1.25f;
            nTextCol = Color.yellow;
        }

        center.sprite = sprites[(int) expandTarg];
        center.transform.localScale = Vector3.one * scale;
        details.transform.localScale = Vector3.one * scale * expand;
        details.transform.localRotation = Quaternion.Euler(0, 0, (1 - expand) * 10);
        newText.transform.localScale = Vector3.one * 0.01f * nText;
        newText.color = nTextCol;

        scaleTarg = scaleTargWhenDefault;
        expandTarg = 0;
    }

    /// <summary>
    /// When the mouse first hovers over this level.
    /// </summary>
    private void OnMouseEnter()
    {
        scale = scaleWhenFirstHovered;

        if(nTextTarg == 1)
        {
            nText = 1.25f;
            nTextCol = Color.yellow;
        }

        audioSource.clip = onHoverAudio;
        audioSource.Play();
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
            audioSource.clip = onClickAudio;
            audioSource.Play();
            GameController.Singleton.LoadLevel(Scene);
        }
    }
}