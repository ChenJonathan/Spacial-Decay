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
    public Warning EnemyWarningPrefab;
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

        // Move to location
        yield return StartCoroutine(transmission.ShowContent("As a safety precaution, your ship's attack capabilities have been disabled as you do not have the proper training credentials. Please proceed to the command center so the restriction can be lifted.", 0.05f, false));
        WarningData warning = new WarningData();
        warning.Prefab = WarningPrefab;
        warning.Duration = float.MaxValue;
        warning.FadeInDuration = 1;
        warning.Data.Location = new Vector2(-6, 0);
        warning.Data.Time = time;
        Warning runtime = SpawnWarning(warning);
        while(Vector2.Distance(LevelController.Singleton.Player.transform.position, runtime.transform.position) >= 1f)
        {
            yield return null;
        }
        Destroy(runtime.gameObject);
        foreach (GameObject o in orbs)
            o.GetComponent<CanvasRenderer>().SetAlpha(100);
        yield return StartCoroutine(transmission.ShowContent("ACCESS GRANTED...\nYou are authorized to access the ship's combat functions. You should now see your ship's charge in the upper left corner.", 0.05f));

        // Move to location 2
        yield return StartCoroutine(transmission.ShowContent("Please proceed to the maintanence bay to complete the upgrade process.", 0.05f));
        warning.Data.Location = new Vector2(6, 0);
        runtime = SpawnWarning(warning);
        while (Vector2.Distance(LevelController.Singleton.Player.transform.position, runtime.transform.position) >= 1f)
        {
            yield return null;
        }
        Destroy(runtime.gameObject);
        Player.maxDashes = 3;
        yield return StartCoroutine(transmission.ShowContent("UPGRADE COMPLETE...\nYour ship is now outfitted with its offensive package. Your HUD should reflect this momentarily.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("Your ship's capacitor banks are capable of storing enough charge for up to three warp jumps. The last century of warfare has led High Command to conclude that any targeted projectile is too easily mitigated by vast improvements in ship mobility. As a result, warp jumps represent the current standard of both offensive and defensive strategy.", 0.05f));

        // Dash mechanic explanation - Dash charges and canceling
        yield return StartCoroutine(transmission.ShowContent("Naturally, warp jumps are mathematically complex. To help you handle the n-dimensional calculations, a core-cortex link will automatically kick in; many pilots describe it as a sensation of \"slo-mo.\"", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("Plot warp jumps using your HUD by selecting a target destination. You may cancel a jump through an alternate click input, but as the engines have already been overloaded, the ship will still fire the engines without consuming extra charge. This will result in a sub-warp velocity.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("To illustrate the potent offensive power of a warp jump, a target ship will be provided.\nUse a warp jump to destroy the ship. Your HUD will automatically warn you of the incoming threat. Do not panic. The target is decomissioned and for training purposes only.\nAim carefully, and accelerate through the target.", 0.05f, false));
        WarningData enemyWarning = new WarningData();
        enemyWarning.Prefab = EnemyWarningPrefab;
        enemyWarning.Duration = 3;
        enemyWarning.FadeInDuration = 1;
        enemyWarning.FadeOutDuration = 1;
        enemyWarning.Data.Location = new Vector2(16, 0);
        enemyWarning.Data.Time = time;
        runtime = SpawnWarning(enemyWarning);
        yield return new WaitForSeconds(3);
        Enemy enemy1 = (Enemy)Instantiate(EnemyPrefab, new Vector3(20, 0), Quaternion.identity);
        while (enemy1)
        {
            yield return null;
        }

        yield return StartCoroutine(transmission.ShowContent("Excellent. Multidimensional warp calculations typically pose a serious challenge for new pilots.", 0.05f));
        // Hitbox explanation - Polygon for enemy ships
        yield return StartCoroutine(transmission.ShowContent("While warping, massive energies twist the fabric of space around you and disintegrate anything in a linear path - igoring the non-Euclidian geometry of such convoluted spacetime, of course. As a result, you may freely pass through both steel and plasma, ignoring collisions with enemy ships or enemy projectiles.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("Your ship is a standard issue fourth-generation fighter produced by SpaceZ Industries. As a combat vehicle, your ship is capable of withstading considerable punishment. It is not invunlerable, however, and you should exercise due caution. You can obtain an overview of your ship's integrity in the upper left corner of your HUD.", 0.05f));

        yield return StartCoroutine(transmission.ShowContent("Colliding directly with other ships will inflict considerable strain on the hull. Policy directs that new pilots learn the feeling of a hull-to-hull impact in order to respond effectively in the future. Take hull damage from the training ship to proceed.", 0.05f, false));
        runtime = SpawnWarning(enemyWarning);
        yield return new WaitForSeconds(3);
        Enemy enemy2 = (Enemy)Instantiate(EnemyPrefab, new Vector3(20, 0), Quaternion.identity);
        int currentHP = LevelController.Singleton.Player.Lives;
        while (LevelController.Singleton.Player.Lives == currentHP)
        {
            if (!enemy2)
                enemy2 = (Enemy)Instantiate(EnemyPrefab, new Vector3(20, 0), Quaternion.identity);
            yield return null;
        }
        enemy2.Die();

        // Hitbox explanation - Center circle for bullets
        yield return StartCoroutine(transmission.ShowContent("It is important to note that most plasma-based weapons concentrate matter in the core of the visible structure as a result of the charged ions. Generally, your ship's hull will be able to dissipate the outer portion of plasma projectiles; the inner core, however, will inflict considerable damage.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("A number of plasma-based projectiles will approach your ship. You may experiment with your ship's tolerance of plasma density. Once you have taken a critical hit to your hull, the projectiles will stop.", 0.05f, false));
        runtime = SpawnWarning(enemyWarning);
        yield return new WaitForSeconds(3);
        TrainingEnemy enemy3 = (TrainingEnemy)Instantiate(EnemyPrefab, new Vector3(32, 0), Quaternion.identity);
        enemy3.enableFire = true;
        currentHP = LevelController.Singleton.Player.Lives;
        while (LevelController.Singleton.Player.Lives == currentHP)
        {
            yield return null;
        }
        enemy3.Die();

        // End
        yield return StartCoroutine(transmission.ShowContent("This concludes basic pilot training. You have been certified for all swaths of civilian space and military space between clearence levels 1-A and 1-C.\nAs you accumulate experience as a pilot, you will be sent deeper into hostile space near the front lines.", 0.05f));
        yield return StartCoroutine(transmission.ShowContent("Good luck, pilot. Nobody can hear you scream in space, but it will certainly carry over the comms.", 0.05f));
        yield return StartCoroutine(transmission.Disappear());
        LevelController.Singleton.EndEvent();
    }
}