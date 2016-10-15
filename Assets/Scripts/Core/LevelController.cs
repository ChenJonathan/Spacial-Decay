using UnityEngine;
using DanmakU;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the level, calling events sequentially.
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

    // List of events in order
    [SerializeField]
    private List<GameObject> events;
    private GameObject currentEvent; // Current wave
    public GameObject Event
    {
        get { return currentEvent; }
    }
    private int eventCount; // Current event number

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
        eventCount = 0;
        StartEvent();
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
    /// Instantiates the current event.
    /// </summary>
    public void StartEvent()
    {
        currentEvent = Instantiate(events[eventCount]);
        currentEvent.transform.SetParent(transform);
    }

    /// <summary>
    /// Called when the current event is completed. Shows the wave completion message.
    /// </summary>
    public void EndEvent()
    {
        Destroy(currentEvent.gameObject);
        eventCount++;

        if(eventCount == events.Count)
        {
            SceneManager.LoadScene("Level Select");
        }
        else
        {
            StartEvent();
        }
    }
}
