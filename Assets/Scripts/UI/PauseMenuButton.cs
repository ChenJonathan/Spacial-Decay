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
<<<<<<< HEAD
    
=======

    public AudioClip onHoverEffect;
    public AudioClip onClickEffect;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

>>>>>>> refs/remotes/origin/master
    void Update()
    {
        scale += (scaleTarg - scale) * scaleDrag;
        transform.localScale = Vector3.one * scale;
        scaleTarg = scaleTargWhenDefault;
    }

    private void OnMouseEnter()
    {
<<<<<<< HEAD
=======
        audioSource.PlayOneShot(onHoverEffect);
>>>>>>> refs/remotes/origin/master
        scale = scaleWhenFirstHovered;
    }

    public void OnMouseOver()
    {
        scaleTarg = scaleTargWhenHovered;

        if(Input.GetMouseButtonDown(0) && GetComponent<SpriteRenderer>().color.a >= 0.5f)
        {
<<<<<<< HEAD
            if(ContinueLevel)
=======
            audioSource.PlayOneShot(onClickEffect);
            if (ContinueLevel)
>>>>>>> refs/remotes/origin/master
                transform.parent.GetComponent<MessagePauseMenu>().SetResume();
            else
                transform.parent.GetComponent<MessagePauseMenu>().SetExit();
        }
    }
}