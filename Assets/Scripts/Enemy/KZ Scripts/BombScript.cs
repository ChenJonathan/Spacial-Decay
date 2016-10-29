using UnityEngine;
using DanmakU;
using DanmakU.Modifiers;
using System.Collections.Generic;

public class BombScript : Enemy
{
    public DanmakuPrefab bulletPrefab;
    public List<Wave.SpawnData> fireList;
    public float fireSpeed;
    public float fireRadius;
    public Warning warningPrefab;
    public float warningDuration;
    public float warningFadeInDuration;
    public float warningFadeOutDuration;

    private FireBuilder fireData;
    private List<Wave.WarningData> warningList;
    private float time;

    public override void Start()
    {
        SetRotation(90);
        fireList.Sort((a, b) => (int)(a.Time * 100 - b.Time * 100));

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.WithSpeed(fireSpeed);
        fireData.WithModifier(new CircularBurstModifier(360, 1, 0, 0));

        warningList = new List<Wave.WarningData>();
        foreach (Wave.SpawnData spawn in fireList)
        {
            if (spawn.Time - warningDuration >= 0)
            {
                Wave.WarningData temp = new Wave.WarningData();
                temp.Prefab = warningPrefab;
                temp.Data = spawn;
                temp.Data.Time -= warningDuration;
                temp.Duration = warningDuration;
                temp.FadeInDuration = warningFadeInDuration;
                temp.FadeOutDuration = warningFadeOutDuration;
                warningList.Add(temp);
            }
        }

        GetComponent<Rigidbody2D>().velocity = new Vector2(-30, 0);
    }

    public void Update()
    {
        time += Time.deltaTime;
        while(warningList.Count > 0 && time >= warningList[0].Data.Time)
        {
            Wave.SpawnWarning(warningList[0]);
            warningList.RemoveAt(0);
        }
        while(fireList.Count > 0 && time >= fireList[0].Time)
        {
            for(int i = 0; i < 20; i++)
            {
                for(int j = 0; j < 5 + Difficulty * 5; j++)
                {
                    fireData.WithSpeed(new DynamicInt(3, 6 + Difficulty));
                    fireData.From(new Vector2(fireList[0].Location.x, fireList[0].Location.y));
                    fireData.Towards(new Vector2(new DynamicInt(-100, 100), new DynamicInt(-100, 100)));
                    fireData.Fire();
                }
            }
            fireList.RemoveAt(0);
            if(fireList.Count == 0)
                Destroy(gameObject, 8);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(time > 6)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
