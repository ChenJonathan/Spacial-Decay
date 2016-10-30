using UnityEngine;
using System.Collections;

/// <summary>
/// A fragment of a level. Displays a message.
/// </summary>
public class Message : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(Run());
    }
    
    /// <summary>
    /// Coroutine to display the wave completion message. Starts the next wave when the message is finished.
    /// </summary>
    private IEnumerator Run()
    {
        CanvasRenderer messageRenderer = GetComponent<CanvasRenderer>();

        for(float a = 0f; a <= 1f; a += 0.02f)
        {
            messageRenderer.SetAlpha(a);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(1);
        for(float a = 1f; a >= 0f; a -= 0.02f)
        {
            messageRenderer.SetAlpha(a);
            yield return new WaitForSeconds(0.05f);
        }

        LevelController.Singleton.EndEvent();
    }
}