﻿using UnityEngine;
using DanmakU;
using DanmakU.Modifiers;
using System.Collections.Generic;

public class HiddenEnemy : Enemy
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
        fireList.Sort((a, b) => (int)(a.Time * 100 - b.Time * 100));

        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.WithSpeed(fireSpeed);

        warningList = new List<Wave.WarningData>();
        foreach(Wave.SpawnData spawn in fireList)
        {
            if(spawn.Time - warningDuration >= 0)
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
	}
	
	public override void Update()
    {
        base.Update();

        if(!LevelController.Singleton.Paused)
        {
            time += Time.deltaTime;
            while(warningList.Count > 0 && time >= warningList[0].Data.Time)
            {
                Wave.SpawnWarning(warningList[0]);
                warningList.RemoveAt(0);
            }
            while(fireList.Count > 0 && time >= fireList[0].Time)
            {
                for(int i = 0; i < 360; i += 5)
                {
                    for(int j = 0; j < 10; j++)
                    {
                        fireData.From(new Vector2(fireList[0].Location.x + Mathf.Cos((i + j) * Mathf.Deg2Rad) * (30 + j), 
                                                  fireList[0].Location.y + Mathf.Sin((i + j) * Mathf.Deg2Rad) * (30 + j)));
                        fireData.Towards(new Vector2(fireList[0].Location.x + Mathf.Cos((i + j + 90) * Mathf.Deg2Rad) * fireRadius,
                                                     fireList[0].Location.y + Mathf.Sin((i + j + 90) * Mathf.Deg2Rad) * fireRadius));
                        fireData.Fire();
                    }
                }
                fireList.RemoveAt(0);
                if(fireList.Count == 0)
                    Destroy(gameObject, 5);
            }
        }
	}
}