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
        transmission = Instantiate(TransmissionPrefab);
        StartCoroutine(Run());
    }

    public override void Update()
    {
        time += Time.deltaTime;
    }

    private IEnumerator Run()
    {
        Player.maxDashes = 0;
        ArrayList orbs = new ArrayList(LevelController.Singleton.Player.dashCounter.orbsCounterActive);
        orbs.AddRange(LevelController.Singleton.Player.dashCounter.orbsCounterInactive);
        foreach (GameObject o in orbs)
            o.GetComponent<CanvasRenderer>().SetAlpha(0);
        // Introduction
        yield return StartCoroutine(transmission.Appear());
        yield return StartCoroutine(transmission.ShowSpeaker("???"));
        yield return StartCoroutine(transmission.ShowContent("Welcome, pilot.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("In the coming days, you will be assisting with the alien threat in the recently discovered Lambda-3 sector. You are to engage any alien hostiles upon detection. Hestation may result in a disadvantageous position.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("Your ship is a standard issue fourth-generation fighter produced by SpaceZ Industries. As a combat vehicle, your ship is capable of withstading considerable punishment. It is not invunlerable, however, and you should exercise due caution. You can obtain an overview of your ship's integrity in the upper left corner of your HUD.", 0.05f));

        // Move to location
        yield return StartCoroutine(transmission.ShowContent("As a safety precaution, your ship's capacitor banks have been disabled as you do not have the proper training credentials. Please proceed to the maintanence bay momentarily so the restriction can be lifted.", 0.05f, false));
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
        Player.maxDashes = 3;
        foreach (GameObject o in orbs)
            o.GetComponent<CanvasRenderer>().SetAlpha(100);
        yield return StartCoroutine(transmission.ShowContent("ACCESS GRANTED...\nYou are authorized to access the ship's combat functions. You should now see your ship's charge in the upper left corner.", 0.05f));

        // Hitbox explanation - Polygon for enemy ships, center circle for bullets

        // Incoming bullets

        // Dash mechanic explanation - Dash charges and canceling

        // Incoming enemies

        // End
        yield return StartCoroutine(transmission.Disappear());
        LevelController.Singleton.EndEvent();
    }
}