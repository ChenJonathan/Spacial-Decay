using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the endless level, spawning a new wave whenever one is finished.
/// </summary>
public class EndlessController : LevelController
{
    public Transmission TransmissionPrefab;

    // Prefabs
    private GameObject previousWave;
    private GameObject currentWave;

    // Runtime instances
    private Transmission transmission;

    /// <summary>
    /// Called when the LevelController is instantiated. Plays introduction dialogue and spawns the first wave.
    /// </summary>
    public override void StartEvent()
    {
        transmission = Instantiate(TransmissionPrefab);
        StartCoroutine(Introduction());
    }

    /// <summary>
    /// Called when a wave ends. Spawns the next wave.
    /// </summary>
    public override void EndEvent()
    {
        if(currentEvent != null)
            Destroy(currentEvent.gameObject);

        // Pick a new wave
        previousWave = currentWave;
        while(previousWave == currentWave)
        {
            currentWave = events[Random.Range(0, events.Count)];
        }

        // Instantiate the wave
        currentEvent = Instantiate(currentWave);
        currentEvent.transform.SetParent(transform);
    }

    /// <summary>
    /// Coroutine to play introduction dialogue and spawn the first wave.
    /// </summary>
    private IEnumerator Introduction()
    {
        yield return StartCoroutine(transmission.Appear());
        yield return StartCoroutine(transmission.ShowSpeaker("???"));
        yield return StartCoroutine(transmission.ShowContent("Welcome, pilot.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", 0.05f));
        yield return StartCoroutine(transmission.Disappear());
        EndEvent();
    }
}
