using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A visual display for the player's remaining lives.
/// </summary>
public class LivesCounter : MonoBehaviour
{
    [SerializeField]
    private GameObject heartPrefab;
    [SerializeField]
    private GameObject overFlow;
    
    private float width;
    private float heartSize;

    [SerializeField]
    private float gap = 6; // Space between hearts
    [SerializeField]
    private int maxDisplayCount = 5; // Number of hearts that can be displayed - Any more will result in a numerical display instead

    private GameObject[] livesCounter;

    /// <summary>
    /// Called on instantiation. Handles initialization.
    /// </summary>
    public void Start()
    {
        int maxLives = Player.MAX_LIVES;
        heartSize = heartPrefab.GetComponent<RectTransform>().sizeDelta.x;
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(2 * gap + heartSize * transform.localScale.x / 2, -2 * gap - heartSize * transform.localScale.y / 2);
        rt.sizeDelta = new Vector2((heartSize + gap) * maxDisplayCount, 30);
        rt.pivot = new Vector2(0.5f / maxDisplayCount, 0.5f);

        livesCounter = new GameObject[maxLives];
        for(int i = 0; i < Mathf.Min(maxLives, maxDisplayCount); i++)
        {
            livesCounter[i] = (GameObject)Instantiate(heartPrefab);
            livesCounter[i].transform.SetParent(transform);
            livesCounter[i].transform.localScale = new Vector3(1, 1, 1);
            livesCounter[i].transform.localPosition = new Vector2(i * (heartSize + gap), 0);
        }
        if(maxLives > maxDisplayCount)
        {
            livesCounter[maxDisplayCount] = Instantiate(overFlow);
            livesCounter[maxDisplayCount].transform.SetParent(transform);
            livesCounter[maxDisplayCount].transform.localScale = this.transform.localScale;
            livesCounter[maxDisplayCount].transform.localPosition = new Vector2((heartSize + gap) / 2, 0);
        }

        UpdateCounter(maxLives);
	}

    /// <summary>
    /// Updates the counter with a new number of player lives.
    /// </summary>
    /// <param name="lives">The player's number of remaining lives</param>
    public void UpdateCounter(int lives)
    {
        lives = Mathf.Max(lives, 0);
        if(lives <= maxDisplayCount)
        {
            for(int i = 0; i < lives; i++)
            {
                livesCounter[i].SetActive(true);
            }
            for(int i = lives; i < maxDisplayCount; i++)
            {
                livesCounter[i].SetActive(false);
            }
            if(Player.MAX_LIVES > maxDisplayCount)
                livesCounter[maxDisplayCount].SetActive(false);
        }
        else
        {
            livesCounter[0].SetActive(true);
            for(int i = 1; i < maxDisplayCount; i++)
            {
                livesCounter[i].SetActive(false);
            }
            livesCounter[maxDisplayCount].GetComponent<Text>().text = " × " + lives;
            livesCounter[maxDisplayCount].SetActive(true);
        }
    }
}