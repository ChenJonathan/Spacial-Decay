using UnityEngine;
using DanmakU;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// The overarching controller class that stores data about completed levels.
/// </summary>
public class GameController : Singleton<GameController>
{
    public Probe ProbePrefab;
    public string StartLevel;
    [HideInInspector]
    public string CurrentLevel;
    [HideInInspector]
    public int Difficulty;

    [HideInInspector]
    public List<string> UnlockedLevels;
    [HideInInspector]
    public List<string> NewLevels;

    /// <summary> Prefab for lines between levels. </summary>
    [SerializeField]
    [Tooltip("Prefab for lines between levels.")]
    private LineRenderer levelLinePrefab;

    // Current camera y-position and max y-position in level select
    private float cameraY;
    private float cameraMaxY;

    /// <summary> Causes all levels to be unlocked at the start of the game. </summary>
    [SerializeField]
    [Tooltip("Causes all levels to be unlocked at the start of the game.")]
    private bool unlockAllLevels;

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

        // Set camera position
        Camera levelCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        levelCamera.fieldOfView = 2.0f * Mathf.Atan(419.84f / levelCamera.aspect / 400f) * Mathf.Rad2Deg;
        cameraMaxY = levelCamera.GetComponent<Scroll>().CameraMaxY = 419.84f / 2f - 419.84f / levelCamera.aspect / 2f;
        cameraY = -cameraMaxY;

        // Initialize levels
        DontDestroyOnLoad(gameObject);
        UnlockedLevels = new List<string>();
        NewLevels = new List<string>();
        if (unlockAllLevels)
        {
            foreach (Level level in GetAllLevels())
            {
                UnlockedLevels.Add(level.name);
            }
        }
        else
        {
            UnlockedLevels.Add(StartLevel);
            Level startLevelObject = GetLevel(StartLevel);
            startLevelObject.gameObject.SetActive(true);
            startLevelObject.Appear();
        }
        SceneManager.sceneLoaded += OnLoad;
    }

    /// <summary>
    /// Called by Level objects to load a specific level.
    /// </summary>
    /// <param name="level">The level to load</param>
    public void LoadLevel(string level)
    {
        cameraY = GameObject.FindGameObjectWithTag("MainCamera").transform.position.y;
        Singleton.CurrentLevel = level;
        NewLevels.Remove(level);
        SceneManager.LoadScene(level);
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
            Camera levelCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            levelCamera.GetComponent<Scroll>().CameraMaxY = cameraMaxY;
            levelCamera.transform.position = new Vector3(0, cameraY, 0);

            // Process levels
            foreach(Level level in GetAllLevels())
            {
                if(level.Scene.Equals(CurrentLevel))
                    NewLevels.Remove(CurrentLevel);

                if(UnlockedLevels.Contains(level.Scene))
                {
                    if(NewLevels.Contains(level.Scene))
                    {
                        // Highlight unplayed levels
                        level.Highlight();
                    }
                    else
                    {
                        // Re-enable lines
                        foreach(Level levelChild in level.Unlocks)
                        {
                            if(UnlockedLevels.Contains(levelChild.Scene) || level.Scene.Equals(CurrentLevel))
                            {
                                // Set line between the two levels
                                LineRenderer levelLine = GameObject.Instantiate(levelLinePrefab);
                                levelLine.transform.parent = level.transform;
                                levelLine.SetPosition(0, level.transform.position);
                                levelLine.SetPosition(1, levelChild.transform.position);

                                // Send probe to new levels
                                if(!UnlockedLevels.Contains(levelChild.Scene))
                                {
                                    levelLine.enabled = false;
                                    levelChild.line = levelLine;
                                    Probe clone = ((Probe)Instantiate(ProbePrefab, level.transform.position, Quaternion.identity));
                                    clone.SetDestination(levelChild);
                                    clone.transform.parent = level.transform;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Disable locked levels
                    level.gameObject.SetActive(false);
                }
            }
            CurrentLevel = null;
        }
    }

    /// <summary>
    /// Gets all levels in the game.
    /// </summary>
    /// <returns>All levels in the game.</returns>
    private Level[] GetAllLevels() {
        return GameObject.FindGameObjectWithTag("Levels").GetComponentsInChildren<Level>();
    }

    private Level GetLevel(string levelName)
    {
        GameObject levels = GameObject.FindGameObjectWithTag("Levels");
        foreach(Level level in levels.GetComponentsInChildren<Level>())
        {
            if(level.Scene.Equals(levelName))
                return level;
        }
        return null;
    }
}