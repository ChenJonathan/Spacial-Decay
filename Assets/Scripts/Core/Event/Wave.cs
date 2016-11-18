using UnityEngine;
using System.Collections.Generic;
using DanmakU;

/// <summary>
/// A fragment of a level. Contains lists of enemies and warnings to spawn. Ends when all enemies are spawned and defeated.
/// </summary>
public class Wave : MonoBehaviour
{
    // Wave options
    public bool ClearEnemiesOnEnd;
    public bool ClearDanmakuOnEnd;

    // Lists of enemies and warnings ordered by prefab
    public List<EnemyChain> EnemyChains;
    public List<WarningChain> WarningChains;

    // Queues to spawn objects sorted by time
    protected List<EnemyData> enemyQueue;
    protected List<WarningData> warningQueue;

    // Active enemies
    protected List<Enemy> enemies;

    // The field that the bullets are spawned in
    protected DanmakuField field;

    // Time elapsed since the start of the wave
    protected float time;
    public float CurrentTime
    {
        get { return time; }
    }

    // Difficulty of the wave, from 0 to 2
    protected int difficulty;
    public int Difficulty
    {
        get { return difficulty; }
    }

    /// <summary>
    /// Contains the time and location to spawn an object.
    /// </summary>
    [System.Serializable]
    public struct SpawnData
    {
        public Vector2 Location;
        public float Time;
        public float[] Parameters;
    }

    /// <summary>
    /// Contains data about a specific enemy and spawn time.
    /// </summary>
    [System.Serializable]
    public struct EnemyData
    {
        public Enemy Prefab;
        public SpawnData Data;
    }

    /// <summary>
    /// Contains data about a specific enemy and the times and locations to spawn it.
    /// </summary>
    [System.Serializable]
    public struct EnemyChain
    {
        public Enemy Prefab;
        public List<SpawnData> Data;
    }

    /// <summary>
    /// Contains data about a specific warning and spawn time.
    /// </summary>
    [System.Serializable]
    public struct WarningData
    {
        public Warning Prefab;
        public float Duration;
        public float FadeInDuration;
        public float FadeOutDuration;
        public SpawnData Data;
    }

    /// <summary>
    /// Contains data about a specific warning and the times and locations to spawn it.
    /// </summary>
    [System.Serializable]
    public struct WarningChain
    {
        public Warning Prefab;
        public float Duration;
        public float FadeInDuration;
        public float FadeOutDuration;
        public List<SpawnData> Data;
    }

    public virtual void Start()
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
                spawn.Data.Parameters = chain.Data[i].Parameters;
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
                spawn.Data.Parameters = chain.Data[i].Parameters;
                warningQueue.Add(spawn);
            }
        }
        warningQueue.Sort((a, b) => (int)(a.Data.Time * 100 - b.Data.Time * 100));

        time = 0;
        difficulty = GameController.Singleton.Difficulty;
    }

    public virtual void Update()
    {
        time += Time.deltaTime;
        if(enemyQueue.Count == 0)
        {
            if(enemies.Count == 0)
            {
                End();
            }
        }
        else
        {
            while(enemyQueue.Count > 0 && time >= enemyQueue[0].Data.Time)
            {
                SpawnEnemy(enemyQueue[0]);
                enemyQueue.RemoveAt(0);
            }
        }
        while(warningQueue.Count > 0 && time >= warningQueue[0].Data.Time)
        {
            SpawnWarning(warningQueue[0]);
            warningQueue.RemoveAt(0);
        }
    }

    /// <summary>
    /// Ends the wave.
    /// </summary>
    public void End()
    {
        if(ClearEnemiesOnEnd)
            ClearEnemies();
        if(ClearDanmakuOnEnd)
            Danmaku.DeactivateAllImmediate();

        LevelController.Singleton.EndEvent();

    }

    /// <summary>
    /// Spawns an enemy.
    /// </summary>
    /// <param name="enemy">The enemy prefab to spawn</param>
    /// <returns>The enemy that was spawned</returns>
    public Enemy SpawnEnemy(EnemyData enemy)
    {
        if(time < enemy.Data.Time)
        {
            enemyQueue.Add(enemy);
            enemyQueue.Sort((a, b) => (int)(a.Data.Time * 100 - b.Data.Time * 100));
            return null;
        }
        else
        {
            Enemy temp = (Enemy)Instantiate(enemy.Prefab, enemy.Data.Location, Quaternion.identity);
            temp.parameters = enemy.Data.Parameters;
            temp.transform.parent = field.transform;
            enemies.Add(temp);
            return temp;
        }
    }

    /// <summary>
    /// Spawns a warning message.
    /// </summary>
    /// <param name="warning">The warning prefab to spawn</param>
    /// <returns>The warning that was spawned</returns>
    public Warning SpawnWarning(WarningData warning)
    {
        if(time < warning.Data.Time)
        {
            warningQueue.Add(warning);
            warningQueue.Sort((a, b) => (int)(a.Data.Time * 100 - b.Data.Time * 100));
            return null;
        }
        else
        {
            Warning temp = (Warning)Instantiate(warning.Prefab, warning.Data.Location, Quaternion.identity);
            temp.Duration = warning.Duration;
            temp.FadeInDuration = warning.FadeInDuration;
            temp.FadeOutDuration = warning.FadeOutDuration;
            temp.transform.parent = field.transform;
            return temp;
        }
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