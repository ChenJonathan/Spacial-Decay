using UnityEngine;
using DanmakU;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// The overarching controller class that stores data about completed levels.
/// </summary>
public class GameController : Singleton<GameController>
{
    public Level StartLevel;
    public Probe ProbePrefab;
    /// <summary> Prefab for lines between levels. </summary>
    [SerializeField]
    [Tooltip("Prefab for lines between levels.")]
    private LineRenderer levelLinePrefab;
    [HideInInspector]
    public Level CurrentLevel;
    [HideInInspector]
    public int Difficulty;

    private List<Level> unlockedLevels;
    private List<Level> newLevels;

    // Current camera y-position in level select
    private float cameraY = -57.62691f;

    /// <summary>
    /// Returns the only instance of the GameController.
    /// </summary>
    /// <returns>The GameController instance</returns>
    public static GameController Singleton
    {
        get { return Instance; }
    }

    /// <summary>
    /// Called when the GameController is instantiated. Handles game initialization.
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        // Destroyed instances stop here
        if(Singleton != this)
        {
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        unlockedLevels = new List<Level>();
        unlockedLevels.Add(StartLevel);
        newLevels = new List<Level>();
        Level[] allLevels = GetComponentsInChildren<Level>();
        foreach (Level level in allLevels) {
            level.gameObject.SetActive(false);
        }
        StartLevel.gameObject.SetActive(true);
        StartLevel.Appear();
        SceneManager.sceneLoaded += OnLoad;
    }

    /// <summary>
    /// Called by Level objects to load a specific level.
    /// </summary>
    /// <param name="level">The level to load</param>
    public void LoadLevel(Level level)
    {
        cameraY = GameObject.FindGameObjectWithTag("MainCamera").transform.position.y;
        Singleton.CurrentLevel = level;
        newLevels.Remove(level);
        SceneManager.LoadScene(level.Scene);
    }

    /// <summary>
    /// Called whenever a scene is loaded. Activates and deactivates the Level objects and controls the level unlock animation.
    /// </summary>
    /// <param name="scene">The scene that was loaded</param>
    /// <param name="mode">How the scene was loaded</param>
    private void OnLoad(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1;

        if(scene.name.Equals("Level Select"))
        {
            // Set camera position
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            mainCamera.transform.position = new Vector3(0, cameraY, 0);

            // Re-enable previously unlocked levels
            foreach(Level level in unlockedLevels)
            {
                level.gameObject.SetActive(true);
                foreach(Probe probe in level.GetComponentsInChildren<Probe>())
                    Destroy(probe.gameObject);
            }

            // Highlight unplayed levels
            foreach(Level level in newLevels)
            {
                level.Highlight();
            }

            // Unlock new levels
            if(CurrentLevel != null)
            {
                foreach(Level level in CurrentLevel.Unlocks)
                {
                    if(!unlockedLevels.Contains(level))
                    {
                        unlockedLevels.Add(level);
                        newLevels.Add(level);
                    }

                    // Set line between the two levels
                    LineRenderer levelLine = GameObject.Instantiate(levelLinePrefab);
                    levelLine.transform.parent = level.transform;
                    levelLine.SetPosition(0, CurrentLevel.transform.position);
                    levelLine.SetPosition(1, level.transform.position);
                    levelLine.enabled = false;
                    level.line = levelLine;

                    // Send probe to new levels
                    Probe clone = ((Probe)Instantiate(ProbePrefab, CurrentLevel.transform.position, Quaternion.identity));
                    clone.SetDestination(level);
                    clone.transform.parent = CurrentLevel.transform;
                }
                CurrentLevel = null;
            }
        }
        else
        {
            // Disable unlocked levels
            foreach(Level level in unlockedLevels)
            {
                level.gameObject.SetActive(false);
            }
        }
    }
}