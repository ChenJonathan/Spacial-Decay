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
    public Probe ProbePrefab;

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

        // Destroyed instances stop here
        if(Singleton == this)
        {
            DontDestroyOnLoad(gameObject);
            unlockedLevels = new List<Level>();
            unlockedLevels.Add(StartLevel);
            StartLevel.gameObject.SetActive(true);
            StartLevel.Appear();
            SceneManager.sceneLoaded += OnLoad;
        }
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
                    {
                        unlockedLevels.Add(level);
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