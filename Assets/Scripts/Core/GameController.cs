using UnityEngine;
using DanmakU;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// The overarching controller class that stores data about completed levels.
/// </summary>
public class GameController : Singleton<GameController>, IPausable
{
    public Level StartLevel;
    [HideInInspector]
    public Level CurrentLevel;
    public Probe ProbePrefab;

    private List<Level> unlockedLevels;
    private List<Level> newLevels;
    
    /// <summary>
    /// Returns the only instance of the GameController.
    /// </summary>
    /// <returns>The GameController instance</returns>
    public static GameController Singleton
    {
        get { return Instance; }
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
    /// Called when the GameController is instantiated. Handles game initialization.
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        // Destroyed instances stop here
        if(Singleton == this)
        {
            DontDestroyOnLoad(gameObject);
            unlockedLevels = new List<Level>();
            unlockedLevels.Add(StartLevel);
            newLevels = new List<Level>();
            StartLevel.gameObject.SetActive(true);
            StartLevel.Appear();
            SceneManager.sceneLoaded += OnLoad;
        }
    }

    /// <summary>
    /// Called by Level objects to load a specific level.
    /// </summary>
    /// <param name="level">The level to load</param>
    public void LoadLevel(Level level)
    {
        GameController.Singleton.CurrentLevel = level;
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
            if(scene.name.Equals("Level Select"))
        {
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

                        // Set line between the two levels
                        level.GetComponent<LineRenderer>().SetPosition(0, CurrentLevel.transform.position);
                        level.GetComponent<LineRenderer>().SetPosition(1, level.transform.position);

                        // Send probe to new levels
                        Probe clone = ((Probe)Instantiate(ProbePrefab, CurrentLevel.transform.position, Quaternion.identity));
                        clone.SetDestination(level);
                        clone.transform.parent = CurrentLevel.transform;
                    }
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