using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A visual display for music / effects volume in the options menu.
/// </summary>
public class VolumeIndicator : MenuButton
{
    [SerializeField]
    private bool music;

    private Image bgRenderer;
    private Image fgRenderer;
    private Image edgeRendererL;
    private Image edgeRendererR;
    private Text display;

    private bool dragging;

    /// <summary>
    /// Called on initialization (before Start). Handles initialization.
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        bgRenderer = GetComponent<Image>();
        fgRenderer = transform.GetChild(2).GetComponent<Image>();
        edgeRendererL = transform.GetChild(0).GetComponent<Image>();
        edgeRendererR = transform.GetChild(1).GetComponent<Image>();
        display = transform.GetChild(4).GetComponent<Text>();

        // Initialize volume
        float volume = (music ? GameController.Singleton.Audio.VolumeMusic : GameController.Singleton.Audio.VolumeEffects);
        fgRenderer.transform.localScale = new Vector3(volume, 1, 1);
        display.text = Mathf.RoundToInt(volume * 100).ToString();

        // Initialize bar color
        bgRenderer.color = new Color((1 - volume) / 2, volume / 2, 0);
        edgeRendererL.color = bgRenderer.color;
        edgeRendererR.color = bgRenderer.color;
        fgRenderer.color = new Color(1 - volume, volume, 0);
    }

    /// <summary>
    /// Updates the display.
    /// </summary>
    public void Update()
    {
        // Handle dragging
        if(dragging)
        {
            if(Input.GetMouseButtonUp(0))
                dragging = false;

            // Set volume
            float volume = Mathf.InverseLerp(0.381f, 0.691f, Input.mousePosition.x / Screen.width);
            fgRenderer.transform.localScale = new Vector3(volume, 1, 1);
            if(music)
                GameController.Singleton.Audio.VolumeMusic = volume;
            else
                GameController.Singleton.Audio.VolumeEffects = volume;
            display.text = Mathf.RoundToInt(volume * 100).ToString();

            // Set bar color
            bgRenderer.color = new Color((1 - volume) / 2, volume / 2, 0);
            edgeRendererL.color = bgRenderer.color;
            edgeRendererR.color = bgRenderer.color;
            fgRenderer.color = new Color(1 - volume, volume, 0);
        }
    }

    public void MouseDown()
    {
        dragging = true;

        // Set volume
        float volume = Mathf.InverseLerp(0.345f, 0.655f, Input.mousePosition.x / Screen.width);
        fgRenderer.transform.localScale = new Vector3(volume, 1, 1);
        if(music)
            GameController.Singleton.Audio.VolumeMusic = volume;
        else
            GameController.Singleton.Audio.VolumeEffects = volume;
    }
}