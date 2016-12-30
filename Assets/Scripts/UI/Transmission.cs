using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

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
    
    public AudioClip onDialogueAudio;
    public AudioClip onContinueAudio;

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
    public virtual IEnumerator ShowSpeaker(string speaker)
    {
        Color speakerColor = Speaker.color;
        if(speakerColor.a != 0)
        {
            while(speakerColor.a != 0)
            {
                speakerColor.a = Mathf.MoveTowards(speakerColor.a, 0, Time.deltaTime * 1.5f);
                Speaker.color = speakerColor;
                yield return null;
            }
        }
        Speaker.text = speaker;
        while(speakerColor.a != 1)
        {
            speakerColor.a = Mathf.MoveTowards(speakerColor.a, 1, Time.deltaTime * 2f);
            Speaker.color = speakerColor;
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine to make the message display some content text.
    /// </summary>
    /// <param name="content">The message content</param>
    /// <param name="delay">Time in seconds for a letter to be shown</param>
    /// <param name="pause">Whether or not to prompt the user to press space after the message ends</param>
    public virtual IEnumerator ShowContent(string content, float delay, bool pause = true)
    {
        int index = 0;
        float time = 0;
        yield return null;

        Regex longStop = new Regex(@"[.;:?!]");
        Regex shortStop = new Regex(@",()");
        while(index < content.Length)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                index = content.Length;
                Content.text = content.Substring(0, index);
                yield return StartCoroutine(ShowContinue());
            }
            else
            {
                yield return null;
                time += Time.deltaTime;
                while(time > delay)
                {
                    time -= delay;
                    index = (index == content.Length) ? content.Length : index + 1;
                    if (index % 2 == 0)
                        AudioSource.PlayClipAtPoint(onDialogueAudio, GameController.Instance.transform.position, GameController.Instance.Audio.VolumeEffects);
                }
                Content.text = content.Substring(0, index);

                // Pause for punctuation
                if (index < content.Length && index > 0 && longStop.IsMatch(content[index - 1].ToString()))
                    yield return new WaitForSeconds(0.1f);
                else if (index < content.Length && index > 0 && shortStop.IsMatch(content[index - 1].ToString()))
                    yield return new WaitForSeconds(0.025f);

                if (index == content.Length && pause)
                    yield return StartCoroutine(ShowContinue());
            }
        }
    }

    /// <summary>
    /// Coroutine to make the continue text show.
    /// </summary>
    public virtual IEnumerator ShowContinue()
    {
        float targetAlpha = 1f;
        Color continueColor = Continue.color = new Color(1, 1, 1, 0);
        yield return null;
        while(!Input.GetKeyDown(KeyCode.Space))
        {
            continueColor.a = Mathf.MoveTowards(continueColor.a, targetAlpha, Time.deltaTime);
            Continue.color = continueColor;
            if(continueColor.a == targetAlpha)
                targetAlpha = 1 - targetAlpha;
            yield return null;
        }
        AudioSource.PlayClipAtPoint(onContinueAudio, GameController.Instance.transform.position, GameController.Instance.Audio.VolumeEffects);
        Continue.color = new Color(1, 1, 1, 0);
    }
}