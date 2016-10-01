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
    
    [SerializeField]
    protected bool FacePlayer;
    protected Quaternion TargetRotation;
    
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

    protected virtual IEnumerator Run()
    {
        return null;
    }

    public virtual void FixedUpdate()
    {
        if(!LevelController.Singleton.Paused)
        {
            if(FacePlayer)
                TargetRotation = Quaternion.LookRotation(Vector3.forward, Player.transform.position - transform.position);
            if(TargetRotation != null)
                transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.fixedDeltaTime * 4);
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

    public virtual void Die()
    {
        Destroy(gameObject);
        LevelController.Singleton.Wave.UnregisterEnemy(this);
    }

    public void OnDestroy()
    {
        Die();
    }

    protected void Rotate(float degrees)
    {
        TargetRotation *= Quaternion.Euler(new Vector3(0, 0, degrees));
    }

    protected override void DanmakuCollision(Danmaku danmaku, RaycastHit2D info)
    {
        Damage(danmaku.Damage);
        danmaku.Deactivate();
    }
}