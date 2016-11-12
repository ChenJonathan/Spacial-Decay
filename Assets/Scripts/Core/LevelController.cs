using UnityEngine;
using DanmakU;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the level, calling events sequentially.
/// </summary>
public class LevelController : DanmakuGameController
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
    
    private Rect viewportRect; // Camera viewport dimensions required to maintain 16:9 aspect ratio
    public Rect ViewportRect
    {
        get { return viewportRect; }
    }

    /// <summary>
    /// Returns the only instance of the LevelController.
    /// </summary>
    /// <returns>The LevelController instance</returns>
    public static LevelController Singleton
    {
        get { return (LevelController)Instance; }
    }

    // Time scale constantly approaches this value
    public float TargetTimeScale = 1;

    // Total time
    [HideInInspector]
    public float LevelTime = 0;

    /// <summary>
    /// Called when the LevelController is instantiated (before Start). Instantiates the player.
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        // Calculate viewport
        float scaleHeight = ((float)Screen.width / (float)Screen.height) / (16.0f / 9.0f);
        if(scaleHeight < 1.0f)
        {
            viewportRect.width = 1.0f;
            viewportRect.height = scaleHeight;
            viewportRect.x = 0;
            viewportRect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            viewportRect.width = scaleWidth;
            viewportRect.height = 1.0f;
            viewportRect.x = (1.0f - scaleWidth) / 2.0f;
            viewportRect.y = 0;
        }

        // Update cameras
        foreach(Camera camera in GameObject.FindObjectsOfType<Camera>())
        {
            if(!camera.tag.Equals("Background"))
                camera.rect = viewportRect;
        }

        // Spawn player
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
    /// Called periodically. Updates bullets and time scale.
    /// </summary>
    public override void Update()
    {
        base.Update();
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, TargetTimeScale, Time.unscaledDeltaTime);
        LevelTime += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Escape))
            TargetTimeScale = Time.timeScale == 0 ? 1 : 0;

        // TODO Remove these
        if(Input.GetKeyDown(KeyCode.Tab))
            GameController.Singleton.LoadLevelSelect(true, LevelTime);
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            EndEvent();
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
    /// Called when the current event is completed.
    /// </summary>
    public void EndEvent()
    {
        Destroy(currentEvent.gameObject);
        eventCount++;

        if(eventCount == events.Count)
        {
            GameController.Singleton.LoadLevelSelect(true, LevelTime);
        }
        else
        {
            TargetTimeScale = 1;
            StartEvent();
        }
    }
}
