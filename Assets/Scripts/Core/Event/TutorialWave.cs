using UnityEngine;
using DanmakU;
using System.Collections;

/// <summary>
/// The tutorial wave.
/// </summary>
public class TutorialWave : Wave
{
    public Transmission TransmissionPrefab;
    public Warning WarningPrefab;
    public Enemy EnemyPrefab;

    private Transmission transmission;

    public override void Start()
    {
        base.Start();

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
        yield return StartCoroutine(transmission.ShowContent("Welcome, pilot.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", 0.05f));

        // Move to location
        yield return StartCoroutine(transmission.ShowContent("Go here and stuff.", 0.05f, false));
        WarningData warning = new WarningData();
        warning.Prefab = WarningPrefab;
        warning.Duration = float.MaxValue;
        warning.FadeInDuration = 1;
        warning.Data.Location = new Vector2(-5, 0);
        warning.Data.Time = time;
        Warning runtime = SpawnWarning(warning);
        while(Vector2.Distance(LevelController.Singleton.Player.transform.position, runtime.transform.position) >= 1f)
        {
            yield return null;
        }
        Destroy(runtime);
        yield return StartCoroutine(transmission.ShowContent("Good job loser.", 0.05f));

        // Hitbox explanation - Polygon for enemy ships, center circle for bullets

        // Incoming bullets

        // Dash mechanic explanation - Dash charges and canceling

        // Incoming enemies

        // End
        yield return StartCoroutine(transmission.Disappear());
        LevelController.Singleton.EndEvent();
    }
}