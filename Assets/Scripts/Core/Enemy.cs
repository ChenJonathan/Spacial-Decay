using UnityEngine;
using System.Collections;
using DanmakU;
using System.Collections.Generic;

public class Enemy : DanmakuCollider
{
    protected Player player;
    protected DanmakuField field;

    public int MaxHealth;
    [HideInInspector]
    public int Health;
    
    public bool FacePlayer;
    
    private GameObject healthBar;
    private float healthBarSize = 1.0f;

    [SerializeField]
    private GameObject damageGUIPrefab;
    [SerializeField]
    private GameObject healthBarPrefab;

    public override sealed void Awake()
    {
        base.Awake();
        player = LevelController.Singleton.Player;
        field = LevelController.Singleton.Field;
        TagFilter = "Friendly";

        healthBar = (GameObject)Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        healthBar.transform.parent = transform;
        healthBar.transform.localScale = new Vector3(healthBarSize, 1, 1);

        Health = MaxHealth;
    }

    public void Update()
    {
        if(!LevelController.Singleton.Paused)
            NormalUpdate();
    }

    public virtual void NormalUpdate() { }

    public void FixedUpdate()
    {
        if(!LevelController.Singleton.Paused)
        {
            if(FacePlayer)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, player.transform.position - transform.position), Time.deltaTime * 4);
            NormalFixedUpdate();
        }
    }

    public virtual void NormalFixedUpdate() { }

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
        LevelController.Singleton.Wave.UnregisterEnemy(this);
        Destroy(gameObject);
    }

    protected override void DanmakuCollision(Danmaku danmaku, RaycastHit2D info)
    {
        Damage(danmaku.Damage);
        danmaku.Deactivate();

        /*
        GameObject damageGUI = (GameObject) Instantiate(damageGUIPrefab, new Vector2(transform.position.x, transform.position.y + 2), Quaternion.identity);
        damageGUI.transform.parent = Field.transform;
        damageGUI.GetComponent<TextMesh>().text = "" + danmaku.Damage;
        */
    }
}
