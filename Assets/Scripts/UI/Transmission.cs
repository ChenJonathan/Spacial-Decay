using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// A conversation message that appears on the bottom of the screen.
/// </summary>
public class Transmission : MonoBehaviour
{
    public SpriteRenderer Background;
    public SpriteRenderer Portrait;
    public Text Speaker;
    public Text Content;
    public Text Continue;

    /// <summary>
    /// Coroutine to display the message.
    /// </summary>
    public virtual IEnumerator Appear()
    {
        Color color = Background.color = Portrait.color = Speaker.color = Content.color = Continue.color = new Color(1, 1, 1, 0);
        for(float a = color.a; a <= 1f; a += 0.02f)
        {
            color.a = a;
            Background.color = Portrait.color = Speaker.color = Content.color = color;
            yield return new WaitForSeconds(0.005f);
        }
    }

    /// <summary>
    /// Coroutine to make the message disappear.
    /// </summary>
    public virtual IEnumerator Disappear()
    {
        Color color = Background.color = Portrait.color = Speaker.color = Content.color = Color.white;
        Continue.color = new Color(1, 1, 1, 0);
        for(float a = color.a; a >= 0f; a -= 0.02f)
        {
            color.a = a;
            Background.color = Portrait.color = Speaker.color = Content.color = color;
            yield return new WaitForSeconds(0.005f);
        }
    }

    /// <summary>
    /// Coroutine to the message display a speaker.
    /// </summary>
<<<<<<< HEAD
    /// <param name="duration">Time in seconds for the message to be shown</param>
=======
>>>>>>> refs/remotes/origin/master
    public virtual IEnumerator ShowSpeaker(string speaker)
    {
        Color speakerColor = Speaker.color;
        if(speakerColor.a != 0)
        {
            while(speakerColor.a != 0)
            {
<<<<<<< HEAD
                speakerColor.a = Mathf.MoveTowards(speakerColor.a, 0, Time.deltaTime);
=======
                speakerColor.a = Mathf.MoveTowards(speakerColor.a, 0, Time.deltaTime * 1.5f);
>>>>>>> refs/remotes/origin/master
                Speaker.color = speakerColor;
                yield return null;
            }
        }
        Speaker.text = speaker;
        while(speakerColor.a != 1)
        {
<<<<<<< HEAD
            speakerColor.a = Mathf.MoveTowards(speakerColor.a, 1, Time.deltaTime);
=======
            speakerColor.a = Mathf.MoveTowards(speakerColor.a, 1, Time.deltaTime * 2f);
>>>>>>> refs/remotes/origin/master
            Speaker.color = speakerColor;
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine to make the message display some content text.
    /// </summary>
    /// <param name="content">The message content</param>
    /// <param name="delay">Time in seconds for a letter to be shown</param>
<<<<<<< HEAD
    public virtual IEnumerator ShowContent(string content, float delay)
    {
        int index = 0;
        float time = 0;
        Continue.color = new Color(1, 1, 1, 0);
=======
    /// <param name="pause">Whether or not to prompt the user to press space after the message ends</param>
    public virtual IEnumerator ShowContent(string content, float delay, bool pause = true)
    {
        int index = 0;
        float time = 0;
        yield return null;
>>>>>>> refs/remotes/origin/master
        while(index < content.Length)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                index = content.Length;
<<<<<<< HEAD
            }
            else
            {
=======
                Content.text = content.Substring(0, index);
                yield return StartCoroutine(ShowContinue());
            }
            else
            {
                yield return null;
>>>>>>> refs/remotes/origin/master
                time += Time.deltaTime;
                while(time > delay)
                {
                    time -= delay;
                    index = (index == content.Length) ? content.Length : index + 1;
                }
                Content.text = content.Substring(0, index);
<<<<<<< HEAD
                yield return null;
=======
                if(index == content.Length && pause)
                    yield return StartCoroutine(ShowContinue());
>>>>>>> refs/remotes/origin/master
            }
        }
    }

    /// <summary>
    /// Coroutine to make the continue text show.
    /// </summary>
<<<<<<< HEAD
    /// <param name="duration">Time in seconds for the message to be shown</param>
=======
>>>>>>> refs/remotes/origin/master
    public virtual IEnumerator ShowContinue()
    {
        float targetAlpha = 1f;
        Color continueColor = Continue.color = new Color(1, 1, 1, 0);
<<<<<<< HEAD
=======
        yield return null;
>>>>>>> refs/remotes/origin/master
        while(!Input.GetKeyDown(KeyCode.Space))
        {
            continueColor.a = Mathf.MoveTowards(continueColor.a, targetAlpha, Time.deltaTime);
            Continue.color = continueColor;
            if(continueColor.a == targetAlpha)
                targetAlpha = 1 - targetAlpha;
            yield return null;
        }
<<<<<<< HEAD
=======
        Continue.color = new Color(1, 1, 1, 0);
>>>>>>> refs/remotes/origin/master
    }
}