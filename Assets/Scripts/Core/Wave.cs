using UnityEngine;
using System.Collections.Generic;
using DanmakU;

public class Wave : MonoBehaviour
{
    public List<EnemyChain> EnemyChains;
    public List<WarningChain> WarningChains;

    private List<EnemyData> enemyQueue;
    private List<WarningData> warningQueue;
    private List<Enemy> enemies;

    private DanmakuField field;

    private float time;

    [System.Serializable]
    public struct SpawnData
    {
        public Vector2 Location;
        public float Time;
    }

    [System.Serializable]
    public struct EnemyData
    {
        public Enemy Prefab;
        public SpawnData Data;
    }

    [System.Serializable]
    public struct EnemyChain
    {
        public Enemy Prefab;
        public List<SpawnData> Data;
    }

    [System.Serializable]
    public struct WarningData
    {
        public Warning Prefab;
        public float Duration;
        public float FadeInDuration;
        public float FadeOutDuration;
        public SpawnData Data;
    }

    [System.Serializable]
    public struct WarningChain
    {
        public Warning Prefab;
        public float Duration;
        public float FadeInDuration;
        public float FadeOutDuration;
        public List<SpawnData> Data;
    }

    void Start()
    {
        field = ((LevelController)LevelController.Instance).Field;
        enemyQueue = new List<EnemyData>();
        warningQueue = new List<WarningData>();
        enemies = new List<Enemy>();

        foreach(EnemyChain chain in EnemyChains)
        {
            int numSpawns = chain.Data.Count;
            for(int i = 0; i < numSpawns; i++)
            {
                EnemyData spawn;
                spawn.Prefab = chain.Prefab;
                spawn.Data.Location = chain.Data[i].Location;
                spawn.Data.Time = chain.Data[i].Time;
                enemyQueue.Add(spawn);
            }
        }
        enemyQueue.Sort((a, b) => (int)(a.Data.Time * 100 - b.Data.Time * 100));

        foreach(WarningChain chain in WarningChains)
        {
            int numSpawns = chain.Data.Count;
            for(int i = 0; i < numSpawns; i++)
            {
                WarningData spawn;
                spawn.Prefab = chain.Prefab;
                spawn.Duration = chain.Duration;
                spawn.FadeInDuration = chain.FadeInDuration;
                spawn.FadeOutDuration = chain.FadeOutDuration;
                spawn.Data.Location = chain.Data[i].Location;
                spawn.Data.Time = chain.Data[i].Time;
                warningQueue.Add(spawn);
            }
        }
        warningQueue.Sort((a, b) => (int)(a.Data.Time * 100 - b.Data.Time * 100));

        time = 0;
    }

    void Update()
    {
        time += Time.deltaTime;
        if(enemyQueue.Count == 0)
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
            while(enemyQueue.Count > 0 && time >= enemyQueue[0].Data.Time)
            {
                SpawnEnemy(enemyQueue[0]);
                enemyQueue.RemoveAt(0);
            }
            while(warningQueue.Count > 0 && time >= warningQueue[0].Data.Time)
            {
                SpawnWarning(warningQueue[0]);
                warningQueue.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// Spawns an enemy.
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public Enemy SpawnEnemy(EnemyData enemy)
    {
        Enemy temp = (Enemy)Instantiate(enemy.Prefab, enemy.Data.Location, Quaternion.identity);
        temp.transform.parent = field.transform;
        enemies.Add(temp);
        return temp;
    }

    /// <summary>
    /// Spawns a warning message.
    /// </summary>
    /// <param name="warning"></param>
    /// <returns></returns>
    public Warning SpawnWarning(WarningData warning)
    {
        Warning temp = (Warning)Instantiate(warning.Prefab, warning.Data.Location, Quaternion.identity);
        temp.Duration = warning.Duration;
        temp.FadeInDuration = warning.FadeInDuration;
        temp.FadeOutDuration = warning.FadeOutDuration;
        temp.transform.parent = field.transform;
        return temp;
    }

    /// <summary>
    /// To be called when an enemy dies. Stops the Wave from tracking the enemy.
    /// </summary>
    /// <param name="enemy">The enemy that died.</param>
    public void UnregisterEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    /// <summary>
    /// Kills all enemies and empties the list of tracked enemies.
    /// </summary>
    public void ClearEnemies()
    {
        for(int i = enemies.Count - 1; i >= 0; i--)
        {
            enemies[i].Die();
        }
        enemies.Clear();
    }
}