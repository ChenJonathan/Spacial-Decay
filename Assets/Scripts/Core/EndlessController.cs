using UnityEngine;
using System.Collections;
using System.IO;

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
        var level = Path.GetRandomFileName();
        yield return StartCoroutine(transmission.Appear());
        yield return StartCoroutine(transmission.ShowSpeaker("SIM_OVERLORD_AI"));
        yield return StartCoroutine(transmission.ShowContent("SIMULATION INITIALIZING... INITALIZED.\nLOADING tr_" + level + ".vgd... COMPLETE.\nENTERING SIMULATION...", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("Welcome to endless mode, pilot. In this simulated enviornment, you will take on wave after wave of hostiles until your inevitable defeat.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("All aspects of combat have been faithfully reproduced down to the last atom of your visor. Save, of course, for your gory death. Bon voyage.", 0.05f));
        yield return StartCoroutine(transmission.Disappear());
        EndEvent();
    }
}
