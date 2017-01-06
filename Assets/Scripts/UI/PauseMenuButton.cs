using UnityEngine;

/// <summary>
/// Adds button functionality to the pause menu buttons.
/// </summary>
public class PauseMenuButton : MonoBehaviour
{
    public bool ContinueLevel;

    private float scale = 0f;
    private float scaleTarg = 0f;
    private float scaleDrag = 0.25f;
    private readonly float scaleWhenFirstHovered = 1.3f;
    private readonly float scaleTargWhenDefault = 1.0f;
    private readonly float scaleTargWhenHovered = 1.15f;
    
    [SerializeField]
    private AudioClip onHoverEffect;
    [SerializeField]
    private AudioClip onClickEffect;

    void Update()
    {
        scale += (scaleTarg - scale) * scaleDrag;
        transform.localScale = Vector3.one * scale;
        scaleTarg = scaleTargWhenDefault;
    }

    private void OnMouseEnter()
    {
        AudioSource.PlayClipAtPoint(onHoverEffect, GameController.Instance.transform.position, GameController.Instance.Audio.VolumeEffects);
        scale = scaleWhenFirstHovered;
    }

    public void OnMouseOver()
    {
        scaleTarg = scaleTargWhenHovered;

        if(Input.GetMouseButtonDown(0) && GetComponent<SpriteRenderer>().color.a >= 0.5f)
        {
            AudioSource.PlayClipAtPoint(onClickEffect, GameController.Instance.transform.position, GameController.Instance.Audio.VolumeEffects);
            if (ContinueLevel)
                transform.parent.GetComponent<MessagePauseMenu>().SetResume();
            else
                transform.parent.GetComponent<MessagePauseMenu>().SetExit();
        }
    }
}