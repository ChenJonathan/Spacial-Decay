using UnityEngine;
using DanmakU;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// The overarching controller class that stores data about completed levels.
/// </summary>
public class GameController : Singleton<GameController>
{
    public string StartLevel;

    [HideInInspector]
    public string CurrentLevel = "";
    [HideInInspector]
    public int Difficulty = 2;

    public struct LevelData
    {
        public bool Unlocked;
        public bool Complete;
        public Difficulty DifficultyBeaten;
        public float TimeBeaten;
    }
    
    [HideInInspector]
    public Dictionary<string, LevelData> Levels;

    /// <summary> Prefab for lines between levels. </summary>
    [SerializeField]
    [Tooltip("Prefab for lines between levels.")]
    private LineRenderer LevelLinePrefab;
    /// <summary> Prefab for indicator that moves to unlocked levels. </summary>
    [SerializeField]
    [Tooltip("Prefab for indicator that moves to unlocked levels.")]
    private Probe ProbePrefab;

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

        // Set camera and camera FOV
        Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        cam.fieldOfView = 2.0f * Mathf.Atan(419.84f / cam.aspect / 400f) * Mathf.Rad2Deg;
        //cameraMaxY = levelCamera.GetComponent<Scroll>().CameraMaxY = 419.84f / 2f - 419.84f / levelCamera.aspect / 2f;
        //cameraY = -cameraMaxY;

        // Initialize levels
        DontDestroyOnLoad(gameObject);
        Levels = new Dictionary<string, LevelData>();
        foreach(Level level in GetAllLevels())
        {
            Levels[level.name] = new LevelData();
            if(unlockAllLevels)
            {
                LevelData levelData = Levels[level.name];
                levelData.Unlocked = true;
                Levels[level.name] = levelData;
            }
        }
        if(!unlockAllLevels)
        {
            LevelData levelData = Levels[StartLevel];
            levelData.Unlocked = true;
            Levels[StartLevel] = levelData;
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
        Singleton.CurrentLevel = level;
        SceneManager.LoadScene(level);
    }

    /// <summary>
    /// Called by the LevelController to return to the level select.
    /// </summary>
    /// <param name="levelData">Information about the completed level</param>
    public void LoadLevelSelect(bool victory, float time = 0)
    {
        LevelData levelData = new LevelData();
        levelData.Unlocked = true;
        levelData.Complete = victory;
        levelData.DifficultyBeaten = (Difficulty)Difficulty;
        levelData.TimeBeaten = time;
        Levels[CurrentLevel] = levelData;
        SceneManager.LoadScene("Level Select");
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
            // Set camera position and bounds
            float minY = Mathf.Infinity;
            float maxY = -Mathf.Infinity;
            float curY = 0;
            Queue<Level> visibleLevels = new Queue<Level>();
            visibleLevels.Enqueue(GetLevel(StartLevel));
            while(visibleLevels.Count > 0)
            {
                Level level = visibleLevels.Dequeue();
                float y = level.transform.position.y;
                if(y < minY)
                    minY = y;
                if(y > maxY)
                    maxY = y;
                if(level.Scene.Equals(CurrentLevel))
                    curY = y;
                if(CurrentLevel != "")
                {
                    foreach(Level levelChild in level.Unlocks)
                    {
                        if(Levels[levelChild.Scene].Unlocked || level.Scene.Equals(CurrentLevel))
                            visibleLevels.Enqueue(levelChild);
                    }
                }
            }

            Scroll camScroll = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Scroll>();
            camScroll.CameraMinY = minY;
            camScroll.CameraMaxY = maxY;
            camScroll.ScrollTo(curY);

            // Process levels
            foreach(Level level in GetAllLevels())
            {
                if(Levels[level.Scene].Unlocked)
                {
                    if(!Levels[level.Scene].Complete)
                    {
                        // Highlight unplayed levels
                        level.Highlight();
                    }
                    else
                    {
                        // Set values
                        if(Levels[level.Scene].Complete)
                        {
                            Text details = level.transform.Find("Details/DetailText").GetComponent<Text>();
                            details.text = "Difficulty Beaten\n<color=#ffff00ff>" + Levels[level.Scene].DifficultyBeaten.ToString()
                                     + "</color>\nTime Beaten\n<color=#ffff00ff>" + Levels[level.Scene].TimeBeaten.ToString("F2") + " seconds</color>";
                        }

                        // Re-enable lines
                        foreach(Level levelChild in level.Unlocks)
                        {
                            if(Levels[levelChild.Scene].Unlocked || level.Scene.Equals(CurrentLevel))
                            {
                                // Set line between the two levels
                                LineRenderer levelLine = GameObject.Instantiate(LevelLinePrefab);
                                levelLine.transform.parent = level.transform;
                                levelLine.SetPosition(0, level.transform.position);
                                levelLine.SetPosition(1, levelChild.transform.position);

                                // Send probe to new levels
                                if(!(Levels[levelChild.Scene].Unlocked))
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
    /// Gets a level in the game.
    /// </summary>
    /// <returns>A level in the game.</returns>
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

    /// <summary>
    /// Gets all levels in the game.
    /// </summary>
    /// <returns>All levels in the game.</returns>
    private Level[] GetAllLevels() {
        return GameObject.FindGameObjectWithTag("Levels").GetComponentsInChildren<Level>();
    }
}