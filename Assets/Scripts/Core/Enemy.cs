using UnityEngine;
using DanmakU;
using System.Collections;

public partial class Enemy : DanmakuCollider
{
    [HideInInspector]
    public Player Player;
    [HideInInspector]
    public DanmakuField Field;
    [HideInInspector]
    public Wave Wave;

    public int MaxHealth;
    [HideInInspector]
    public int Health;
    
    public bool FacePlayer;
    
    private GameObject healthBar;
    private float healthBarSize = 1.0f;
    
    [SerializeField]
    private GameObject healthBarPrefab;

    public override sealed void Awake()
    {
        base.Awake();
        Player = LevelController.Singleton.Player;
        Field = LevelController.Singleton.Field;
        Wave = LevelController.Singleton.Wave;
        TagFilter = "Friendly";

        healthBar = (GameObject)Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        healthBar.transform.parent = transform;
        healthBar.transform.localScale = new Vector3(healthBarSize, 1, 1);

        Health = MaxHealth;
    }

    public virtual void Start()
    {
        StartCoroutine(Run());
    }

    public virtual void FixedUpdate()
    {
        if(!LevelController.Singleton.Paused)
        {
            if(FacePlayer)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, Player.transform.position - transform.position), Time.fixedDeltaTime * 4);
        }
    }

    public virtual void Damage(int damage)
    {
        Health -= damage;

        float healthProportion = (float)Health / MaxHealth;
        healthBar.GetComponentInChildren<HealthIndicator>().Activate(healthProportion);

        if(Health <= 0)
        {
            Die();
        }
    }

    protected virtual IEnumerator Run()
    {
        return null;
    }

    public virtual void Die()
    {
        Destroy(gameObject);
        LevelController.Singleton.Wave.UnregisterEnemy(this);
    }

    public void OnDestroy()
    {
        Die();
    }

    protected override void DanmakuCollision(Danmaku danmaku, RaycastHit2D info)
    {
        Damage(danmaku.Damage);
        danmaku.Deactivate();
    }
}