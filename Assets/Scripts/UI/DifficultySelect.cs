using DanmakU;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A space probe that travels to a level, unlocking it.
/// </summary>
public class DifficultySelect : Singleton<DifficultySelect>
{
    [SerializeField]
    private Text levelName;
    [SerializeField]
    private GameObject[] starsActive;
    [SerializeField]
    private GameObject[] starsInactive;

    private Camera mainCamera;
    private Level level;
    public Level Level
    {
        get { return level; }
    }

    /// <summary>
    /// Called when the difficulty select is instantiated.
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        gameObject.SetActive(false);
    }

    public void Start()
    {
        SetDifficulty(0);
    }

    /// <summary>
    /// Called periodically. Keeps the camera anchored to the selected level.
    /// </summary>
    public void Update()
    {
        if(level != null)
        {
            transform.position = mainCamera.WorldToScreenPoint(level.transform.position) + new Vector3(0, (float)(0.25 * Screen.height), 0);
        }
    }

    /// <summary>
    /// Activates the difficulty select and places it next to a level.
    /// </summary>
    /// <param name="level">The level object to anchor to</param>
    public void Activate(Level level)
    {
        this.level = level;
        levelName.text = level.Scene;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Deactivates the difficulty select.
    /// </summary>
    public void Deactivate()
    {
        level = null;
        gameObject.SetActive(false);
    }

    public void SetDifficulty(int difficulty)
    {
        if(difficulty >= 0 && difficulty <= 2)
        {
            GameController.Singleton.Difficulty = difficulty;
            for(int i = 0; i < 3; i++)
            {
                if(difficulty >= i)
                {
                    starsActive[i].gameObject.SetActive(true);
                    starsInactive[i].gameObject.SetActive(false);
                }
                else
                {
                    starsActive[i].gameObject.SetActive(false);
                    starsInactive[i].gameObject.SetActive(true);
                }
            }
        }
    }

    public void StartLevel()
    {
        GameController.Singleton.LoadLevel(level);
    }
}