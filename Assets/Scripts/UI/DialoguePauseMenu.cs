using UnityEngine;
using System.Collections;

/// <summary>
/// A dialogue that is displayed when the level is paused.
/// </summary>
public class DialoguePauseMenu : Dialogue
{
    public SpriteRenderer Resume;
    public SpriteRenderer Exit;

    private bool resume = false;
    private bool exit = false;

    /// <summary>
    /// Control coroutine for the message.
    /// </summary>
    protected override IEnumerator Run()
    {
        Fade.color = new Color(Fade.color.r, Fade.color.g, Fade.color.b, 0f);
        Text.color = Resume.color = Exit.color = new Color(Text.color.r, Text.color.g, Text.color.b, 0f);

        yield return StartCoroutine(Appear());

        while(!resume && !exit)
        {
            if(!LevelController.Singleton.Paused || Input.GetKeyDown(KeyCode.Escape))
            {
                resume = true;
                exit = false;
            }
            yield return null;
        }

        yield return StartCoroutine(Disappear());

        LevelController.Singleton.Paused = false;
        if(resume)
            Destroy(gameObject);
        else
            GameController.Singleton.LoadLevelSelect(false);
    }

    /// <summary>
    /// Coroutine to display the message.
    /// </summary>
    protected override IEnumerator Appear()
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
        }
        for(float y = backgroundScale.y; y <= 1f; y = Mathf.Lerp(y, 1.01f, 0.1f))
        {
            fadeColor.a = 0.5f + y / 2;
            Fade.color = fadeColor;

            backgroundScale.y = y;
            Background.transform.localScale = backgroundScale;
            yield return StartCoroutine(WaitForRealSeconds(0.005f));
        }
        for(float a = textColor.a; a <= 1f; a += 0.02f)
        {
            textColor.a = a;
            Text.color = textColor;
            Resume.color = textColor;
            Exit.color = textColor;
            yield return StartCoroutine(WaitForRealSeconds(0.005f));
        }
    }

    /// <summary>
    /// Coroutine to make the message disappear.
    /// </summary>
    protected override IEnumerator Disappear()
    {
        Color textColor = Text.color;
        Vector3 backgroundScale = Background.transform.localScale;

        for(float a = textColor.a; a >= 0f; a -= 0.02f)
        {
            textColor.a = a;
            Text.color = textColor;
            Resume.color = textColor;
            Exit.color = textColor;
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

    public void SetResume()
    {
        if(!exit)
            resume = true;
    }

    public void SetExit()
    {
        if(!resume)
            exit = true;
    }
}