using UnityEngine;
using UnityEngine.UI;
using DanmakU;
using System.Collections;

public class DashCounter : MonoBehaviour
{
    [SerializeField]
    private GameObject orbActivePrefab;
    [SerializeField]
    private GameObject orbInactivePrefab;

    private float width;
    private float orbSize;

    [SerializeField]
    private float gap = 6;

    private GameObject[] orbsCounterActive;
    private GameObject[] orbsCounterInactive;

    void Start()
    {
        orbSize = orbActivePrefab.GetComponent<RectTransform>().sizeDelta.x;
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(2 * gap + orbSize * transform.localScale.x / 2, -4 * gap - 2 * orbSize * transform.localScale.y / 2);
        rt.sizeDelta = new Vector2((orbSize + gap) * Player.MAX_DASHES, 30);
        rt.pivot = new Vector2(0.5f / Player.MAX_DASHES, 0.5f);

        orbsCounterActive = new GameObject[Player.MAX_DASHES];
        orbsCounterInactive = new GameObject[Player.MAX_DASHES];
        for(int i = 0; i < Player.MAX_DASHES; i++)
        {
            orbsCounterActive[i] = (GameObject)Instantiate(orbActivePrefab);
            orbsCounterActive[i].transform.SetParent(transform);
            orbsCounterActive[i].transform.localScale = Vector3.one;
            orbsCounterActive[i].transform.localPosition = new Vector2(i * (orbSize + gap), 0);
            orbsCounterInactive[i] = (GameObject)Instantiate(orbInactivePrefab);
            orbsCounterInactive[i].transform.SetParent(transform);
            orbsCounterInactive[i].transform.localScale = Vector3.one;
            orbsCounterInactive[i].transform.localPosition = new Vector2(i * (orbSize + gap), 0);
        }

        UpdateCounter(0);
    }

    public void UpdateCounter(int orbs)
    {
        orbs = Mathf.Max(orbs, 0);
        for(int i = 0; i < orbs; i++)
        {
            orbsCounterActive[i].SetActive(true);
            orbsCounterInactive[i].SetActive(false);
        }
        for(int i = orbs; i < Player.MAX_DASHES; i++)
        {
            orbsCounterActive[i].SetActive(false);
            orbsCounterInactive[i].SetActive(true);
        }
    }
}