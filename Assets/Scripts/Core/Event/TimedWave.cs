using UnityEngine;

/// <summary>
/// A fragment of a level. Contains lists of enemies and warnings to spawn. Ends after a set duration.
/// </summary>
public class TimedWave : Wave
{
    // Wave options
    public float Duration;

    public override void Update()
    {
        time += Time.deltaTime;
        if(time >= Duration)
            End();
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