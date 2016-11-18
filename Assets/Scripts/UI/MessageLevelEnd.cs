using UnityEngine;
using System.Collections;

/// <summary>
/// A dialogue that is displayed when the level ends, either as a victory or a loss.
/// </summary>
public class MessageLevelEnd : Message
{
    public float Duration;
    public bool Victory;

    /// <summary>
    /// Control coroutine for the message.
    /// </summary>
    protected override IEnumerator Run()
    {
        Background.transform.localScale = Vector3.zero;
        Fade.color = new Color(Fade.color.r, Fade.color.g, Fade.color.b, 0f);
        Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, 0f);

        // Ensure that game is not paused
        LevelController.Singleton.Paused = false;
        yield return new WaitUntil(() => FindObjectOfType<MessagePauseMenu>() == null);
        LevelController.Singleton.TargetTimeScale = 0f;

        yield return StartCoroutine(Appear());

        // Wait for a few seconds or player left click
        float time = 0;
        while(time < Duration)
        {
            if(Input.GetMouseButtonDown(0))
                time = Duration;
            else
                time += Time.unscaledDeltaTime;
            yield return null;
        }

        yield return StartCoroutine(Disappear());

        if(Victory)
            GameController.Singleton.LoadLevelSelect(true, LevelController.Singleton.LevelTime);
        else
            GameController.Singleton.LoadLevelSelect(false);
    }
}