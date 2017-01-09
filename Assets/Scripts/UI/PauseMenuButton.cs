using UnityEngine;

/// <summary>
/// Adds button functionality to the pause menu buttons.
/// </summary>
public class PauseMenuButton : MonoBehaviour
{
    public enum ButtonType { Continue, Restart, Exit }

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
    [SerializeField]
    private ButtonType buttonType;

    public void Update()
    {
        scale += (scaleTarg - scale) * scaleDrag;
        transform.localScale = Vector3.one * scale;
        scaleTarg = scaleTargWhenDefault;
    }

    public void OnMouseEnter()
    {
        GetComponentInParent<AudioSource>().PlayOneShot(onHoverEffect, GameController.Instance.Audio.VolumeEffects);
        scale = scaleWhenFirstHovered;
    }

    public void OnMouseOver()
    {
        scaleTarg = scaleTargWhenHovered;

        if(Input.GetMouseButtonDown(0) && GetComponent<SpriteRenderer>().color.a >= 0.5f)
        {
            GetComponentInParent<AudioSource>().PlayOneShot(onClickEffect, GameController.Instance.Audio.VolumeEffects);
            if (buttonType == ButtonType.Continue)
                transform.parent.GetComponent<MessagePauseMenu>().SetResume();
            else if (buttonType == ButtonType.Restart)
                transform.parent.GetComponent<MessagePauseMenu>().SetRestart();
            else
                transform.parent.GetComponent<MessagePauseMenu>().SetExit();
        }
    }
}