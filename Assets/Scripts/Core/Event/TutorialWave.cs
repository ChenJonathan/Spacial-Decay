using UnityEngine;
using DanmakU;
using System.Collections;

/// <summary>
/// The tutorial wave.
/// </summary>
public class TutorialWave : Wave
{
    public Transmission TransmissionPrefab;

    private Transmission transmission;

    public override void Start()
    {
        StartCoroutine(Run());
    }

    public override void Update()
    {
        time += Time.deltaTime;
    }

    private IEnumerator Run()
    {
        transmission = Instantiate(TransmissionPrefab);

        // Introduction
        yield return StartCoroutine(transmission.Appear());
        yield return StartCoroutine(transmission.ShowSpeaker("???"));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(transmission.ShowContent("Welcome, pilot.", 0.1f));
        yield return StartCoroutine(transmission.ShowContinue());
        yield return new WaitForSeconds(1f);

        // Move to location

        // Hitbox explanation

        // Incoming bullets

        // Dash mechanic explanation

        // Incoming enemies

        // End
        yield return StartCoroutine(transmission.Disappear());
        LevelController.Singleton.EndEvent();
    }
}