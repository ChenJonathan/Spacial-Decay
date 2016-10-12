using UnityEngine;
using DanmakU;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the level, calling waves sequentially and handling dialogue and messages.
/// </summary>
public class LevelController : DanmakuGameController, IPausable
{
    // Field to spawn the bullets in
    [SerializeField]
    private DanmakuField field;
    public DanmakuField Field
    {
        get { return field; }
    }

    // The player prefab to be instantiated
    [SerializeField]
    private Player playerPrefab;
    private Player player;
    public Player Player
    {
        get { return player; }
    }

    // List of waves in order
    [SerializeField]
    private List<Wave> waves;
    private Wave currentWave; // Current wave
    public Wave Wave
    {
        get { return currentWave; }
    }
    private int waveCount; // Current wave number

    // Spawned on wave completion
    public GameObject waveMessage;

    /// <summary>
    /// Returns the only instance of the LevelController.
    /// </summary>
    /// <returns>The LevelController instance</returns>
    public static LevelController Singleton
    {
        get { return (LevelController)Instance; }
    }

    /// <summary>
    /// Returns whether or not the game is paused.
    /// </summary>
    /// <returns>Whether or not the game is paused</returns>
    public bool Paused
    {
        get;
        set;
    }

    /// <summary>
    /// Called when the LevelController is instantiated (before Start). Instantiates the player.
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        
        Vector2 spawnPos = Field.WorldPoint(Vector2.zero);
        player = (Player)Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        player.transform.parent = Field.transform;
    }

    /// <summary>
    /// Called when the LevelController is instantiated. Starts the first wave.
    /// </summary>
    public void Start()
    {
        waveCount = 0;
        StartWave();
    }

    /// <summary>
    /// Called periodically. Updates bullets.
    /// </summary>
    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Paused = !Paused;
        else if(Input.GetKeyDown(KeyCode.Tab))
            SceneManager.LoadScene("Level Select");

        if(!Paused)
        {
            base.Update();
        }
    }

    /// <summary>
    /// Instantiates the current wave.
    /// </summary>
    public void StartWave()
    {
        currentWave = Instantiate(waves[waveCount]);
        currentWave.transform.parent = transform;
    }

    /// <summary>
    /// Called when the current wave is completed. Shows the wave completion message.
    /// </summary>
    public void EndWave()
    {
        Destroy(currentWave.gameObject);
        waveCount++;

        StartCoroutine(ShowWaveMessage());
    }

    /// <summary>
    /// Coroutine to display the wave completion message. Starts the next wave when the message is finished.
    /// </summary>
    private IEnumerator ShowWaveMessage()
    {
        CanvasRenderer messageRender = waveMessage.GetComponent<CanvasRenderer>();
        waveMessage.SetActive(true);

        for(float a = 0f; a <= 1f; a += 0.02f)
        {
            messageRender.SetAlpha(a);
            yield return null;
        }
        yield return new WaitForSeconds(1);
        for(float a = 1f; a >= 0f; a -= 0.02f)
        {
            messageRender.SetAlpha(a);
            yield return null;
        }

        waveMessage.SetActive(false);
        if(waveCount == waves.Count)
        {
            SceneManager.LoadScene("Level Select");
        }
        else
        {
            StartWave();
        }
    }
}
