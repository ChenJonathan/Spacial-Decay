using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DanmakU;
using DanmakU.Modifiers;

public class BasicProgrammableEnemy : Enemy
{
    public DanmakuPrefab bulletPrefab;
    public float fireSpeed;
    public float fireAngle;
    public int fireCount;       
    public float deltaSpeed;
    public float deltaAngularSpeed;    

    public List<location> locations;
    public List<fireTime> fireTimes;

    private FireBuilder fireData;
    private Rigidbody2D rigidbody2d;
    private float time = 0f;
    private float prevTime = 0f;
    int locCount = 0;
    private Vector2 prevLoc;
    
    [System.Serializable]
    public struct location
    {
        public Vector2 loc;
        public float time;
    }

    [System.Serializable]
    public struct fireTime
    {
        public float startTime;
        public float endTime;
        public bool isFacingPlayer;
    } 

    public override void Start()
    {            
        fireData = new FireBuilder(bulletPrefab, Field);
        fireData.From(transform);
        fireData.WithSpeed(fireSpeed);
        fireData.WithModifier(new CircularBurstModifier(fireAngle, fireCount, deltaSpeed, deltaAngularSpeed));

        locations.Sort((a, b) => (int)(a.time * 100 - b.time * 100));
        fireTimes.Sort((a, b) => (int)(a.startTime * 100 - b.startTime * 100));
        
        prevLoc = transform.position;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if(locations.Count == 0)
        {
            Die();
        }

        if(time - prevTime >= locations[locCount].time)
        {
            prevTime = time;
            Vector2 nextLoc = locations[locCount].loc;
            Vector2 direction = nextLoc - prevLoc;
            float speed = Vector3.Distance(nextLoc, prevLoc) / locations[locCount].time;
            GetComponent<Rigidbody2D>().velocity = direction / direction.magnitude * speed;
            locCount++;
        }
        time += Time.fixedDeltaTime;
    }
}