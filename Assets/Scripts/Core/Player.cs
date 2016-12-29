using UnityEngine;
using System.Collections;
using DanmakU;
using System.Collections.Generic;

/// <summary>
/// The player. Follows the mouse cursor and performs a dash when left click is held and released.
/// </summary>
public class Player : MonoBehaviour
{
    // Visual indicator that shows where the enemy is headed
    [SerializeField]
    private GameObject targetPrefab;
    
    private DanmakuField field;
    private Collider2D collider2d;
    private Rigidbody2D rigidbody2d;

    // Counters to display information
    private LivesCounter livesCounter;
    public DashCounter dashCounter;
    
    // Player life values
    private int lives = MAX_LIVES;
    public int Lives
    {
        get { return lives; }
    }
    public static readonly int MAX_LIVES = 8;

    // Whether the player is moving or not
    private bool moving;
    public bool IsMoving
    {
        get { return moving; }
    }

    // Whether the player is dashing or not, followed by some dashing-related values
    private bool dashing = false;
    public bool IsDashing
    {
        get { return dashing; }
    }
    private bool selecting = false; // True while the mouse is held down
    private int dashes = 0; // Number of dashes remaining
    private float dashCooldown = 0; // Time since last dash charge was received
    public static readonly int MAX_DASHES = 3; // Maximum number of dashes that can be held at once
    private static float MAX_DASH_COOLDOWN = 4; // Time required for a dash charge to be gained
    public bool CanDash = true; // True if dashing and dash charge gaining is enabled

    // Whether the player is invincible or not
    private bool invincible = false;
    public bool IsInvincible
    {
        get { return invincible; }
    }
    private static readonly float INVINCIBILITY_ON_SPAWN = 2; // Invinciblity time after the player is instantiated
    private static readonly float INVINCIBILITY_ON_HIT = 2; // Invincibility time after the player is hit

    // Player movement values
    [SerializeField]
    private float moveSpeed = 360;
    [SerializeField]
    private float dashSpeed = 720;
    [SerializeField]
    private float rotateSpeed = 18;

    // List of enemies hit during a dash - prevents the player from hitting enemies multiple times
    private List<Enemy> hitEnemies = new List<Enemy>();

    // Dash selection line colors
    [SerializeField]
    private Color dashStartActive;
    [SerializeField]
    private Color dashEndActive;
    [SerializeField]
    private Color dashStartInactive;
    [SerializeField]
    private Color dashEndInactive;

    private Vector2 target; // Location that the enemy is moving towards
    private SpriteRenderer targetRenderer; // Renders the movement target
    private LineRenderer dashRenderer; // Renders the dash selection line
    private SpriteRenderer spriteRenderer; // Renders the ship
    private SpriteRenderer hitboxGlowRenderer; // Renders the glow effect for the hitbox
    private SpriteRenderer wingsGlowRenderer; // Renders the glow effect for the wings
    private SpriteRenderer trailRenderer; // Renders the ship trail
    private Animator trailAnimator; // Animates the ship trail
    private ParticleSystem hitEffect; // Particle effect for when the player takes damage

    private float currentAlphaHitbox = 0f;
    private float currentAlphaWings = 0f;
    private float targetAlphaHitbox = 0f;
    private float targetAlphaWings = 0f;
    private float deltaAlphaWings = 0f; // Speed at which the wing alpha value moves towards its target

    // Cached mouse position for easy access
    private Vector2 mousePos = Vector2.zero;

    private AudioSource audioSource;

    public AudioClip OnHitAudio;
    public AudioClip OnDeathAudio;
    public AudioClip OnNewDashAudio;
    public AudioClip OnDashAudio;

    /// <summary>
    /// Called when the player is instantiated (before Start). Handles player initialization.
    /// </summary>
    public void Awake()
    {
        // Retrieve references
        field = LevelController.Singleton.Field;
        collider2d = GetComponent<Collider2D>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        trailAnimator = transform.FindChild("Trail").GetComponent<Animator>();
        hitEffect = GetComponentInChildren<ParticleSystem>();

        // Initializes the renderers
        target = new Vector2(transform.position.x, transform.position.y);
        GameObject targetObject = (GameObject)Instantiate(targetPrefab, Vector3.zero, Quaternion.identity);
        targetRenderer = targetObject.GetComponent<SpriteRenderer>();
        dashRenderer = targetObject.GetComponent<LineRenderer>();
        dashRenderer.sortingOrder = -1;
        dashRenderer.material = new Material(Shader.Find("Particles/Additive"));
        dashRenderer.SetColors(dashStartInactive, dashEndInactive);
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitboxGlowRenderer = transform.FindChild("GlowHitbox").GetComponent<SpriteRenderer>();
        wingsGlowRenderer = transform.FindChild("GlowWings").GetComponent<SpriteRenderer>();
        trailRenderer = transform.FindChild("Trail").GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();

        // Retrieves counter references
        foreach(GameObject counter in GameObject.FindGameObjectsWithTag("Counter"))
        {
            if(counter.GetComponent<LivesCounter>() != null)
                livesCounter = counter.GetComponent<LivesCounter>();
            if(counter.GetComponent<DashCounter>() != null)
                dashCounter = counter.GetComponent<DashCounter>();
        }
    }

    /// <summary>
    /// Called when the player is instantiated. Makes the player invincible for a bit.
    /// </summary>
    public void Start()
    {
        StartCoroutine(SetInvincible(INVINCIBILITY_ON_SPAWN));
    }

    /// <summary>
    /// Called periodically. Handles the dash mechanic, setting the target, and more.
    /// </summary>
    public void Update()
    {
        // Handle input if not paused / pausing
        if(LevelController.Singleton.TargetTimeScale != 0)
        {
            HandleInput();
        }
        else
        {
            selecting = false;
            SetMoveTarget(mousePos);
            dashRenderer.enabled = false;
        }

        // General movement-related functions
        if(moving)
        {
            float distance = Vector2.Distance(transform.position, target);
            if(distance > 1)
                targetRenderer.enabled = true;
            moving = distance > (dashing ? 1 : 0.1);
            if(!moving)
            {
                // Player reached its target
                targetRenderer.enabled = false;
                rigidbody2d.velocity = Vector3.zero;
                if(dashing)
                {
                    dashing = false;
                    hitEnemies.Clear();
                    targetAlphaWings = 0f;
                    deltaAlphaWings = 1f;
                    trailAnimator.SetBool("Active", false);
                }
            }
        }
        else if(selecting)
        {
            targetRenderer.enabled = true;
        }

        // Handling the dash cooldown
        if(dashes < MAX_DASHES && CanDash)
        {
            dashCooldown += Time.deltaTime;
            if(dashCooldown >= MAX_DASH_COOLDOWN)
            {
                dashes++;
                audioSource.clip = OnNewDashAudio;
                audioSource.Play();
                dashCooldown = 0;
                dashRenderer.SetColors(dashStartActive, dashEndActive);
                dashCounter.UpdateCounter(dashes);
            }
        }

        // Update hitbox alpha
        currentAlphaHitbox = Mathf.MoveTowards(currentAlphaHitbox, targetAlphaHitbox, Time.deltaTime);
        Color temp = hitboxGlowRenderer.color;
        temp.a = currentAlphaHitbox * spriteRenderer.color.a;
        hitboxGlowRenderer.color = temp;
        if(currentAlphaHitbox == targetAlphaHitbox)
            targetAlphaHitbox = 1 - targetAlphaHitbox;

        // Update wings alpha
        currentAlphaWings = Mathf.MoveTowards(currentAlphaWings, targetAlphaWings, Time.deltaTime * deltaAlphaWings);
        temp = wingsGlowRenderer.color;
        temp.a = currentAlphaWings * spriteRenderer.color.a;
        wingsGlowRenderer.color = temp;
    }

    /// <summary>
    /// Called in fixed-time intervals. Handles movement, rotation, and collision detection.
    /// </summary>
    public void FixedUpdate()
    {
        // Move and rotate towards the target
        if(moving)
        {
            rigidbody2d.velocity = ((Vector3)target - transform.position) * Time.fixedDeltaTime * (dashing ? dashSpeed : moveSpeed);
            if(!selecting)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(
                    Vector3.forward, (Vector3)target - transform.position), Time.fixedDeltaTime * rotateSpeed);
            }
        }

        // Rotate to mouse if selecting
        if(selecting)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(
                Vector3.forward, (Vector3)targetRenderer.transform.position - transform.position), Time.fixedDeltaTime * rotateSpeed);
        }

        // Collision detection
        RaycastHit2D[] hitArray = Physics2D.RaycastAll(transform.position, rigidbody2d.velocity, rigidbody2d.velocity.magnitude * Time.fixedDeltaTime);
        if(hitArray.Length > 0)
        {
            foreach(RaycastHit2D hit in hitArray)
            {
                Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
                if(enemy != null)
                {
                    if(dashing)
                    {
                        if(!hitEnemies.Contains(enemy))
                        {
                            hitEnemies.Add(enemy);
                            enemy.Damage(50);
                        }
                    }
                    else if(!invincible)
                    {
                        Hit();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called in Update. Handles user input.
    /// </summary>
    private void HandleInput()
    {
        // Calculate mouse position in world coordinates
        mousePos.x = (Input.mousePosition.x / Screen.width - LevelController.Singleton.ViewportRect.x) / LevelController.Singleton.ViewportRect.width;
        mousePos.y = (Input.mousePosition.y / Screen.height - LevelController.Singleton.ViewportRect.y) / LevelController.Singleton.ViewportRect.height;
        mousePos = field.WorldPoint(mousePos);

        if(Input.GetMouseButtonDown(0) && !dashing && CanDash)
        {
            // Begin dash targeting
            selecting = true;
            SetMoveTarget(mousePos);
            targetAlphaWings = 1f;
            deltaAlphaWings = 3f;
            trailAnimator.SetBool("Active", true);
            dashRenderer.SetPosition(0, transform.position);
            dashRenderer.SetPosition(1, mousePos);
            dashRenderer.enabled = true;
            LevelController.Singleton.TargetTimeScale = 0.5f;
        }
        else if(selecting)
        {
            // Dash targeting
            if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(1))
            {
                // Begin dash
                if(Input.GetMouseButtonUp(0) && dashes > 0)
                {
                    SetDashTarget(mousePos);
                    dashes--;

                    if(dashes == 0)
                        dashRenderer.SetColors(dashStartInactive, dashEndInactive);
                    audioSource.clip = OnDashAudio;
                    audioSource.Play();
                }

                // Disable dash selection
                selecting = false;
                targetAlphaWings = 0f;
                deltaAlphaWings = 3f;
                trailAnimator.SetBool("Active", false);
                SetMoveTarget(mousePos);
                dashRenderer.enabled = false;
                dashCounter.UpdateCounter(dashes);
                LevelController.Singleton.TargetTimeScale = 1;
            }
            else
            {
                targetRenderer.transform.position = mousePos;
                dashRenderer.SetPosition(0, transform.position);
                dashRenderer.SetPosition(1, mousePos);
            }
        }
        else if(!dashing)
        {
            // Normal movement targeting
            SetMoveTarget(mousePos);
        }
    }

    /// <summary>
    /// Called when the player collides with an enemy or an enemy bullet.
    /// </summary>
    public void Hit()
    {
        if (!LevelController.Singleton.PermanentInvincible)
        {
            lives--;
            livesCounter.UpdateCounter(lives);
        }

        hitEffect.Play();
        if(lives == 0)
        {
            LevelController.Singleton.EndLevel(false);
            audioSource.clip = OnDeathAudio;
        }
        else
        {
            StartCoroutine(SetInvincible(INVINCIBILITY_ON_HIT));

            audioSource.clip = OnHitAudio;
        }
        audioSource.Play();
    }

    /// <summary>
    /// Sets the movement target and the target location. Note that the two may not be the same.
    /// The visual target can be out of bounds but the target location cannot.
    /// </summary>
    public void SetMoveTarget(Vector2 target)
    {
        moving = true;
        targetRenderer.transform.position = target;
        this.target = BoundsUtil.VerifyBounds(target, new Bounds2D(collider2d.bounds), field.MovementBounds);
    }

    /// <summary>
    /// Sets the dash target and the target location. Note that the two may not be the same.
    /// The visual target can be out of bounds but the target location cannot.
    /// The player will be locked out of controlling movement until the dash target is reached.
    /// </summary>
    public void SetDashTarget(Vector2 target)
    {
        dashing = true;
        SetMoveTarget(target);
    }

    /// <summary>
    /// Coroutine to make the player invincible for some time. Also handles the flashing effect.
    /// </summary>
    public IEnumerator SetInvincible(float time)
    {
        Color color = spriteRenderer.color;
        invincible = true;
        float timer = 0;
        while(timer < time)
        {
            if(timer % 0.05f > (timer + Time.deltaTime) % 0.05f)
            {
                color.a = 1.25f - color.a;
                spriteRenderer.color = color;
                trailRenderer.color = color;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        color.a = 1;
        spriteRenderer.color = color;
        trailRenderer.color = color;
        invincible = false;
        yield break;
    }
}