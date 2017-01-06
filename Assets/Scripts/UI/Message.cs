using UnityEngine;
using System.Collections;

/// <summary>
/// A dialogue that appears and disappears. Subclasses should provide control flow.
/// </summary>
public abstract class Message : MonoBehaviour
{
    public SpriteRenderer Fade;
    public SpriteRenderer Background;
    public SpriteRenderer Text;
    
    public void Start()
    {
        GetComponent<AudioSource>().volume = GameController.Instance.Audio.VolumeEffects;
        LevelController.Singleton.TargetTimeScale = 0f;
        StartCoroutine(Run());
    }

    /// <summary>
    /// Control coroutine for the message.
    /// </summary>
    protected abstract IEnumerator Run();

    /// <summary>
    /// Coroutine to display the message.
    /// </summary>
    protected virtual IEnumerator Appear()
    {
        Vector3 backgroundScale = new Vector3(0, 0.05f, 1);
        Color fadeColor = Fade.color;
        Color textColor = Text.color;

        for(float x = backgroundScale.x; x <= 1f; x = Mathf.Lerp(x, 1.01f, 0.1f))
        {
            fadeColor.a = x / 2;
            Fade.color = fadeColor;

            backgroundScale.x = x;
            Background.transform.localScale = backgroundScale;

            yield return StartCoroutine(WaitForRealSeconds(0.005f));
            if(Input.GetKeyDown(KeyCode.Escape))
                yield break;
        }
        for(float y = backgroundScale.y; y <= 1f; y = Mathf.Lerp(y, 1.01f, 0.1f))
        {
            fadeColor.a = 0.5f + y / 2;
            Fade.color = fadeColor;

            backgroundScale.y = y;
            Background.transform.localScale = backgroundScale;

            yield return StartCoroutine(WaitForRealSeconds(0.005f));
            if(Input.GetKeyDown(KeyCode.Escape))
                yield break;
        }
        for(float a = textColor.a; a <= 1f; a += 0.05f)
        {
            textColor.a = a;
            Text.color = textColor;

            yield return StartCoroutine(WaitForRealSeconds(0.005f));
            if(Input.GetKeyDown(KeyCode.Escape))
                yield break;
        }
    }

    /// <summary>
    /// Coroutine to make the message disappear.
    /// </summary>
    protected virtual IEnumerator Disappear()
    {
        Color textColor = Text.color;
        Vector3 backgroundScale = Background.transform.localScale;

        for(float a = textColor.a; a >= 0f; a -= 0.05f)
        {
            textColor.a = a;
            Text.color = textColor;

            yield return StartCoroutine(WaitForRealSeconds(0.005f));
        }
        for(float y = backgroundScale.y; y >= 0.05f; y = Mathf.Lerp(y, 0.04f, 0.1f))
        {
            backgroundScale.y = y;
            Background.transform.localScale = backgroundScale;

            yield return StartCoroutine(WaitForRealSeconds(0.005f));
        }
        for(float x = backgroundScale.x; x >= 0f; x = Mathf.Lerp(x, -0.01f, 0.1f))
        {
            backgroundScale.x = x;
            Background.transform.localScale = backgroundScale;

            yield return StartCoroutine(WaitForRealSeconds(0.005f));
        }
    }

    protected static IEnumerator WaitForRealSeconds(float delay)
    {
        float start = Time.realtimeSinceStartup;
        while(Time.realtimeSinceStartup < start + delay)
        {
            yield return null;
        }
    }
}