using DanmakU;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A space probe that travels to a level, unlocking it.
/// </summary>
public class DifficultySelect : Singleton<DifficultySelect>
{
    [SerializeField]
    private GameObject starActivePrefab;
    [SerializeField]
    private GameObject starInactivePrefab;

    private Camera mainCamera;
    private CanvasScaler scaler;
    private Level level;
    public Level Level
    {
        get { return level; }
    }

    private Button[] stars;

    /// <summary>
    /// Called when the difficulty select is instantiated.
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        scaler = transform.GetComponentInParent<CanvasScaler>();
        stars = new Button[3];
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
            transform.position = mainCamera.WorldToScreenPoint(level.transform.position) + new Vector3(0, 200, 0) * scaler.scaleFactor;
        }
    }

    /// <summary>
    /// Activates the difficulty select and places it next to a level.
    /// </summary>
    /// <param name="level">The level object to anchor to</param>
    public void Activate(Level level)
    {
        this.level = level;
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
                if(stars[i] != null)
                    Destroy(stars[i].gameObject);
                if(difficulty >= i)
                    stars[i] = ((GameObject)Instantiate(starActivePrefab, transform.position + new Vector3(60, 0, 0) * (i - 1), Quaternion.identity)).GetComponent<Button>();
                else
                    stars[i] = ((GameObject)Instantiate(starInactivePrefab, transform.position + new Vector3(60, 0, 0) * (i - 1), Quaternion.identity)).GetComponent<Button>();
                int j = i;
                stars[i].onClick.AddListener(delegate { SetDifficulty(j); });
                stars[i].transform.SetParent(transform);
            }
        }
    }

    public void StartLevel()
    {
        GameController.Singleton.LoadLevel(level);
    }
}