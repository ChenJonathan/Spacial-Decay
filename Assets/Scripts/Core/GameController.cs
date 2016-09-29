using UnityEngine;
using DanmakU;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameController : Singleton<GameController>, IPausable
{
    public Level StartLevel;
    [HideInInspector]
    public Level CurrentLevel;

    private List<Level> unlockedLevels;

    public static GameController Singleton
    {
        get { return Instance; }
    }

    public bool Paused
    {
        get;
        set;
    }

    public override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        unlockedLevels = new List<Level>();
        unlockedLevels.Add(StartLevel);
        StartLevel.gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnLoad;
    }

    public void LoadLevel(Level level)
    {
        GameController.Singleton.CurrentLevel = level;
        SceneManager.LoadScene(level.Scene);
    }

    private void OnLoad(Scene scene, LoadSceneMode mode)
    {
        if(scene.name.Equals("Level Select"))
        {
            // Re-enable previously unlocked levels
            foreach(Level level in unlockedLevels)
            {
                level.gameObject.SetActive(true);
            }

            // Unlock new levels
            if(CurrentLevel != null)
            {
                foreach(Level level in CurrentLevel.Unlocks)
                {
                    if(!unlockedLevels.Contains(level))
                        unlockedLevels.Add(level);
                }
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