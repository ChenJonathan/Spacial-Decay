using UnityEngine;
using DanmakU;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the level, calling events sequentially.
/// </summary>
public class LevelController : DanmakuGameController, IPausable
{
    // The player prefab to be instantiated
    [SerializeField]
    private Player playerPrefab;
    private Player player;
    public Player Player
    {
        get { return player; }
    }

    // Field to spawn the bullets in
    [SerializeField]
    private DanmakuField field;
    public DanmakuField Field
    {
        get { return field; }
    }

    // List of events in order
    [SerializeField]
    protected List<GameObject> events;
    protected GameObject currentEvent; // Current event
    public GameObject Event
    {
        get { return currentEvent; }
    }
    private int eventCount = 0; // Current event number
    
    private Rect viewportRect; // Camera viewport dimensions required to maintain 16:9 aspect ratio
    public Rect ViewportRect
    {
        get { return viewportRect; }
    }

    // Prefabs
    [SerializeField]
    private MessageLevelEnd levelCompleteMessage;
    [SerializeField]
    private MessageLevelEnd levelFailedMessage;
    [SerializeField]
    private MessagePauseMenu pauseMenuMessage;

    private Message currentMessage;

    /// <summary>
    /// Returns the only instance of the LevelController.
    /// </summary>
    /// <returns>The LevelController instance</returns>
    public static LevelController Singleton
    {
        get { return (LevelController)Instance; }
    }

    // Whether the game is paused or not
    private bool paused;
    [HideInInspector]
    public bool Paused
    {
        get { return paused; }
        set
        {
            paused = value;
            if(paused)
                TargetTimeScale = 0;
            else
                TargetTimeScale = 1;
        }
    }

    // Total time
    [HideInInspector]
    public float LevelTime = 0;

    // Time scale constantly approaches this value
    [HideInInspector]
    public float TargetTimeScale = 1;

    /// <summary> Makes the player invincible permanently. </summary>
    [SerializeField]
    [Tooltip("Makes the player invincible permanently.")]
    public bool PermanentInvincible;

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
        StartEvent();
    }

    /// <summary>
    /// Called periodically. Updates bullets and time scale.
    /// </summary>
    public override void Update()
    {
        base.Update();
        LevelTime += Time.deltaTime;

        // Pausing things
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, TargetTimeScale, Time.unscaledDeltaTime);
        if(Input.GetKeyDown(KeyCode.Escape) && !Paused && FindObjectOfType<Message>() == null)
        {
            if(!(currentMessage is MessageLevelEnd))
            {
                Paused = true;
                currentMessage = Instantiate(pauseMenuMessage);
            }
        }
    }

    /// <summary>
    /// Instantiates the current event.
    /// </summary>
    public virtual void StartEvent()
    {
        currentEvent = Instantiate(events[eventCount]);
        currentEvent.transform.SetParent(transform);
    }

    /// <summary>
    /// Called when the current event is completed.
    /// </summary>
    public virtual void EndEvent()
    {
        Destroy(currentEvent.gameObject);
        eventCount++;

        if(eventCount == events.Count)
            EndLevel(true);
        else
            StartEvent();
    }

    /// <summary>
    /// Called when the current level is completed.
    /// </summary>
    public virtual void EndLevel(bool victory)
    {
        if(currentMessage)
            Destroy(currentMessage.gameObject);

        currentMessage = Instantiate(victory ? levelCompleteMessage : levelFailedMessage);
    }
}
