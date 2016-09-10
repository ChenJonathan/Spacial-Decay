using UnityEngine;
using System.Collections.Generic;
using DanmakU;

public class Wave : MonoBehaviour
{
    public List<SpawnChain> spawns;

    private List<SpawnData> spawnQueue;
    private List<Enemy> enemies;

    private DanmakuField field;

    private float time;

    [System.Serializable]
    public struct SpawnData
    {
        public Enemy Prefab;

        public Vector2 Location;
        public float Time;
    }

    [System.Serializable]
    public struct SpawnChain
    {
        public Enemy Prefab;

        public List<Vector2> Locations;
        public List<float> Times;
    }

    void Start()
    {
        field = ((LevelController)LevelController.Instance).Field;
        spawnQueue = new List<SpawnData>();
        enemies = new List<Enemy>();

        foreach(SpawnChain chain in spawns)
        {
            int numSpawns = Mathf.Min(chain.Locations.Count, chain.Times.Count);
            for(int i = 0; i < numSpawns; i++)
            {
                SpawnData spawn;
                spawn.Prefab = chain.Prefab;
                spawn.Location = chain.Locations[i];
                spawn.Time = chain.Times[i];
                spawnQueue.Add(spawn);
            }
        }
        spawnQueue.Sort((a, b)=>(int)(a.Time * 100 - b.Time * 100));

        time = 0;
    }

    void Update()
    {
        time += Time.deltaTime;
        if(spawnQueue.Count == 0)
        {
            if(enemies.Count == 0)
            {
                Danmaku.DeactivateAllImmediate();
                ClearEnemies();

                LevelController.Singleton.EndWave();
            }
        }
        else
        {
            while(spawnQueue.Count > 0 && time >= spawnQueue[0].Time)
            {
                SpawnEnemy(spawnQueue[0]);
                spawnQueue.RemoveAt(0);
            }
        }
    }

    public Enemy SpawnEnemy(SpawnData enemy)
    {
        Enemy temp = (Enemy)Instantiate(enemy.Prefab, enemy.Location, Quaternion.identity);
        temp.transform.parent = field.transform;
        enemies.Add(temp);
        return temp;
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public void ClearEnemies()
    {
        for(int i = enemies.Count - 1; i >= 0; i--)
        {
            enemies[i].Die();
        }
        enemies.Clear();
    }
}
