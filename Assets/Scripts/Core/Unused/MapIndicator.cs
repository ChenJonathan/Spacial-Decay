using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DanmakU;

public class MapIndicator : Singleton<MapIndicator>
{
    [SerializeField]
    private GameObject room;
    private GameObject[][] rooms;

    [SerializeField]
    private GameObject endpointPrefab;
    private GameObject endMarker;
    [SerializeField]
    private GameObject playerLocPrefab;
    private GameObject playerMarker;

    private float roomSize;
    private RectTransform rt;

    private HashSet<IntVector> opened = new HashSet<IntVector>();
    private HashSet<IntVector> cleared = new HashSet<IntVector>();

    private IntVector playerLocation;
    
    public enum State { Clickable, Invisible, ViewOnly }
    private State currentState;
    public State CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;
            UpdateMap();
        }
    }
    
    public override void Awake()
    {
        base.Awake();

        float corridorSize = room.transform.FindChild("Right Door").GetComponent<RectTransform>().sizeDelta.x;
        roomSize = room.GetComponent<RectTransform>().sizeDelta.x + corridorSize;
        rt = GetComponent<RectTransform>();
    }

    private void UpdateMap()
    {
        opened = ((MapController)MapController.Instance).OpenedRooms;
        cleared = ((MapController)MapController.Instance).ClearedRooms;
        playerLocation = ((MapController)MapController.Instance).Location;

        Image mapRenderer = gameObject.GetComponent<Image>();
        mapRenderer.enabled = !CurrentState.Equals(State.Invisible);
        playerMarker.SetActive(!CurrentState.Equals(State.Invisible));
        endMarker.SetActive(!CurrentState.Equals(State.Invisible));

        for (int i = 0; i < rooms.Length; i++)
        {
            for (int j = 0; j < rooms[i].Length; j++)
            {
                if (rooms[i][j] != null)
                {
                    rooms[i][j].GetComponent<Button>().enabled = (currentState == State.Clickable);
                    Image im = rooms[i][j].GetComponent<Image>();
                    
                    if (cleared.Contains(new IntVector(i, j)))
                    {
                        rooms[i][j].SetActive(!CurrentState.Equals(State.Invisible));
                        for (int k = rooms[i][j].transform.childCount - 1; k >= 0; k--)
                            rooms[i][j].transform.GetChild(k).gameObject.SetActive(true);
                        rooms[i][j].GetComponent<Button>().enabled = false;
                        im.color = Color.green;
                    }
                    else if (opened.Contains(new IntVector(i, j)))
                    {
                        rooms[i][j].SetActive(!CurrentState.Equals(State.Invisible));
                        ColorBlock cb = ColorBlock.defaultColorBlock;
                        cb.normalColor = im.color = Color.red;
                        cb.highlightedColor = Color.red / 2;
                        rooms[i][j].GetComponent<Button>().colors = cb;
                    }
                    else
                    {
                        rooms[i][j].SetActive(false);
                    }
                }
            }
        }
        if (playerMarker.activeSelf)
            playerMarker.transform.localPosition = rooms[playerLocation.x][playerLocation.y].transform.localPosition;
    }

    public void Generate(MapController.Map map)
    {
        rt.sizeDelta = new Vector2(map.size.x * roomSize, map.size.y * roomSize);
        rt.anchoredPosition = Vector3.zero;
        rt.pivot = new Vector2(.5f, .5f);
        Vector2 shiftVector = new Vector2((-rt.sizeDelta.x + roomSize) / 2, (rt.sizeDelta.y - roomSize) / 2);

        rt.localScale = new Vector3(Mathf.Min(1, 4.0f / map.size.x), Mathf.Min(1, 4.0f / map.size.y), 1);

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i));
        }

        rooms = new GameObject[map.size.x][];
        for (int i = 0; i < map.size.x; i++)
        {
            rooms[i] = new GameObject[map.size.y];
            for (int j = 0; j < map.size.y; j++)
            {
                if (map.rooms[i][j].active)
                {
                    rooms[i][j] = (GameObject)Instantiate(room);
                    rooms[i][j].transform.SetParent(transform);
                    rooms[i][j].transform.localPosition = new Vector2(i * 80, -j * 80) + shiftVector;
                    rooms[i][j].transform.localScale = new Vector2(1, 1);

                    if (!map.rooms[i][j].up)
                        Destroy(rooms[i][j].transform.Find("Up Door").gameObject);
                    if (!map.rooms[i][j].down)
                        Destroy(rooms[i][j].transform.Find("Down Door").gameObject);
                    if (!map.rooms[i][j].left)
                        Destroy(rooms[i][j].transform.Find("Left Door").gameObject);
                    if (!map.rooms[i][j].right)
                        Destroy(rooms[i][j].transform.Find("Right Door").gameObject);

                    int index = j + map.size.y * i;
                    Button b = rooms[i][j].GetComponent<Button>();
                    b.onClick.AddListener(() =>
                    {
                        ((MapController)MapController.Instance).SetRoom(index);
                    }
                    );
                    rooms[i][j].SetActive(map.start.Equals(new IntVector(i, j)));
                }
                if (map.end.Equals(new IntVector(i, j)))
                {
                    endMarker = (GameObject)Instantiate(endpointPrefab);
                    endMarker.transform.SetParent(transform);
                    endMarker.transform.localPosition = rooms[i][j].transform.localPosition;
                    endMarker.transform.localScale = new Vector3(1, 1, 1);
                }

            }
        }
        playerMarker = (GameObject)Instantiate(playerLocPrefab);
        playerMarker.transform.SetParent(transform);
        playerMarker.transform.localPosition = rooms[map.start.x][map.start.y].transform.localPosition;
        playerMarker.transform.localScale = new Vector3(1, 1, 1);
    }

    /*
    IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(2);

        CanvasRenderer[] mapRenderers = gameObject.GetComponentsInChildren<CanvasRenderer>();
        for (float a = 1f; a >= 0f; a -= 0.01f)
        {
            foreach (CanvasRenderer mapRenderer in mapRenderers)
            {
                mapRenderer.SetAlpha(a);
            }
            yield return null;
        }

        gameObject.SetActive(false);
        foreach (CanvasRenderer mapRenderer in mapRenderers)
        {
            mapRenderer.SetAlpha(1);
        }
    }
    */
}
