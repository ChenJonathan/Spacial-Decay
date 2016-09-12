using UnityEngine;
using DanmakU;
using System.Collections.Generic;
using System.Collections;

public partial class LevelController : DanmakuGameController, IPausable
{
    [SerializeField]
    private DanmakuField field;
    public DanmakuField Field
    {
        get { return field; }
    }

    [SerializeField]
    private Player playerPrefab;
    private Player player;
    public Player Player
    {
        get { return player; }
    }

    [SerializeField]
    private List<Wave> waves;
    private Wave currentWave;
    public Wave Wave
    {
        get { return currentWave; }
    }
    private int waveCount;

    public GameObject waveMessage;

    public static LevelController Singleton
    {
        get { return (LevelController)Instance; }
    }

    public bool Paused
    {
        get;
        set;
    }

    public override void Awake()
    {
        base.Awake();
        
        Vector2 spawnPos = Field.WorldPoint(Vector2.zero);
        player = (Player)Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        player.transform.parent = Field.transform;
    }

    public void Start()
    {
        waveCount = 0;
        StartWave();
    }

    public override void Update()
    {
        if(!Paused)
        {
            base.Update();
        }
    }

    public void StartWave()
    {
        currentWave = Instantiate(waves[waveCount]);
        currentWave.transform.parent = transform;
    }

    public void EndWave()
    {
        Destroy(currentWave.gameObject);
        waveCount++;

        if(waveCount == waves.Count)
        {
            Debug.Log("Level complete");
            // TODO
        }
        else
        {
            StartCoroutine(ShowWaveMessage());
        }
    }

    private IEnumerator ShowWaveMessage()
    {
        CanvasRenderer messageRender = waveMessage.GetComponent<CanvasRenderer>();
        waveMessage.SetActive(true);

        for(float a = 0f; a <= 1f; a += 0.01f)
        {
            messageRender.SetAlpha(a);
            yield return null;
        }
        yield return new WaitForSeconds(2);
        for(float a = 1f; a >= 0f; a -= 0.01f)
        {
            messageRender.SetAlpha(a);
            yield return null;
        }

        waveMessage.SetActive(false);
        StartWave();
    }
}
