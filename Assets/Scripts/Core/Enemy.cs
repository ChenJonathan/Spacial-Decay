using UnityEngine;
using DanmakU;
using System.Collections;

/// <summary>
/// An enemy that fires bullets at the player and can be hurt by the player's dash mechanic.
/// </summary>
public partial class Enemy : DanmakuCollider
{
    [HideInInspector]
    public Player Player;
    [HideInInspector]
    public DanmakuField Field;
    [HideInInspector]
    public Wave Wave;
    [HideInInspector]
    public int Difficulty;
    
    // Enemy health values
    public int MaxHealth;
    [HideInInspector]
    public int Health;
    [HideInInspector]
    public float RotateSpeed = 8;

    protected AudioSource audioSource;
    [SerializeField]
    private AudioClip onHitAudio;
    [SerializeField]
    private AudioClip onDeathAudio;
    [SerializeField]
    private AudioClip onFireAudio;

    [SerializeField]
    private Warning explosion;

    // Whether the enemy is invincible or not
    protected bool invincible = false;
    public bool IsInvincible
    {
        get { return invincible; }
    }
    protected static readonly float INVINCIBILITY_ON_HIT = 2; // Invincibility time after the enemy is hit

    // Enemy rotation values
    [SerializeField]
    protected bool FacePlayer; // Enemy constantly rotates toward the player if true - overrides TargetRotation
    protected Quaternion TargetRotation; // Enemy constantly rotates toward this rotation if it is not null

    // Enemy health bar reference and size
    private GameObject healthBar;
    private float healthBarSize = 1.0f;
    
    [SerializeField]
    private GameObject healthBarPrefab;

    /// <summary> Parameters that can be used to modify the enemy's behavior. </summary>
    [HideInInspector]
    public float[] parameters;

    /// <summary>
    /// Called when the enemy is instantiated (before Start). Initializes the enemy.
    /// </summary>
    public override sealed void Awake()
    {
        base.Awake();
        Player = LevelController.Singleton.Player;
        Field = LevelController.Singleton.Field;
        Wave = LevelController.Singleton.Event.GetComponent<Wave>();
        Difficulty = Wave.Difficulty;
        TagFilter = "Untagged";

        healthBar = (GameObject)Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
        healthBar.transform.localScale = new Vector3(healthBarSize, 1, 1);
        healthBar.transform.parent = transform;

        Health = MaxHealth;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.75f;

        // Assigns fields from resources folder if not customized
        if(!onHitAudio)
            onHitAudio = Resources.Load<AudioClip>("SFX/Hit_Hurt");
        if(!onDeathAudio)
            onDeathAudio = Resources.Load<AudioClip>("SFX/Explosion");
        if(!onFireAudio)
            onFireAudio = Resources.Load<AudioClip>("SFX/Default_Fire");
        if(!explosion)
            explosion = Resources.Load<Warning>("Warning/Explosion/Explosion");
    }

    /// <summary>
    /// Called when the enemy is instantiated. Starts the Run coroutine.
    /// </summary>
    public virtual void Start()
    {
        StartCoroutine(Run());
    }

    /// <summary>
    /// Generic coroutine to control the enemy.
    /// </summary>
    protected virtual IEnumerator Run()
    {
        return null;
    }

    /// <summary>
    /// Called in fixed-time intervals. Updates the enemy rotation and handles other physics-related functions.
    /// </summary>
    /// <param name="warning">The warning prefab to spawn</param>
    /// <returns>The warning that was spawned</returns>
    public virtual void FixedUpdate()
    {
        if(FacePlayer)
            TargetRotation = Quaternion.LookRotation(Vector3.forward, Player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, Time.fixedDeltaTime * RotateSpeed);
    }

    /// <summary>
    /// Damages the enemy by a value.
    /// </summary>
    /// <param name="damage">The amount of damage to deal</param>
    public virtual void Damage(int damage)
    {
        if(!invincible)
        {
            Health -= damage;

            float healthProportion = (float)Health / MaxHealth;
            healthBar.GetComponentInChildren<HealthIndicator>().Activate(healthProportion);

            if(Health <= 0)
            {
                audioSource.PlayOneShot(onDeathAudio, GameController.Instance.Audio.VolumeEffects);
                Die();
            }
            else
            {
                audioSource.PlayOneShot(onHitAudio, GameController.Instance.Audio.VolumeEffects);
                StartCoroutine(SetInvincible(INVINCIBILITY_ON_HIT));
            }
        }
    }

    /// <summary>
    /// Kills the enemy and removes the enemy from the list of active enemies in the wave.
    /// </summary>
    public virtual void Die()
    {
        Wave.WarningData explosionData = new Wave.WarningData();
        explosionData.Prefab = explosion;
        explosionData.Duration = 10.083f;
        explosionData.Data.Location = transform.position;
        Wave.SpawnWarning(explosionData);

        Destroy(gameObject);
    }

    /// <summary>
    /// Called when the enemy's GameObject is destroyed. Makes sure that finalization code is executed.
    /// </summary>
    public void OnDestroy()
    {
        Wave.UnregisterEnemy(this);
    }

    /// <summary>
    /// Coroutine to make the enemy invincible for some time. Also handles the flashing effect.
    /// </summary>
    public IEnumerator SetInvincible(float time)
    {
        Renderer renderer = GetComponent<Renderer>();
        Color color = renderer.material.color;
        invincible = true;
        float timer = 0;
        while(timer < time)
        {
            if(timer % 0.05f > (timer + Time.deltaTime) % 0.05f)
            {
                color.a = 1.25f - color.a;
                renderer.material.color = color;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        color.a = 1;
        renderer.material.color = color;
        invincible = false;
        yield break;
    }

    #region Rotation methods

    /// <summary>
    /// Adds a value to the enemy's rotation.
    /// </summary>
    /// <param name="degrees">The angle of increase in degrees</param>
    protected void AddRotation(float degrees)
    {
        TargetRotation *= Quaternion.Euler(new Vector3(0, 0, degrees));
    }

    /// <summary>
    /// Sets the enemy's rotation.
    /// </summary>
    /// <param name="degrees">The angle of rotation in degrees</param>
    protected void SetRotation(float degrees)
    {
        TargetRotation = Quaternion.Euler(new Vector3(0, 0, degrees));
    }

    /// <summary>
    /// Rotates the enemy in a direction.
    /// </summary>
    /// <param name="direction">A vector representing the direction to rotate towards</param>
    protected void SetRotation(Vector2 direction)
    {
        SetRotation(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90);
    }

    /// <summary>
    /// Rotates the enemy toward a point.
    /// </summary>
    /// <param name="point">The point to rotate the enemy towards</param>
    protected void RotateTowards(Vector3 point)
    {
        SetRotation(point - transform.position);
    }

    #endregion

    /// <summary>
    /// Handles collision with a bullet.
    /// </summary>
    /// <param name="danmaku">The bullet that the enemy collided with</param>
    /// <param name="info">Collision information</param>
    protected override void DanmakuCollision(Danmaku danmaku, RaycastHit2D info)
    {
        danmaku.Deactivate();
    }
}