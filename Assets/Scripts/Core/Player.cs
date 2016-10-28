using UnityEngine;
using System.Collections;
using DanmakU;

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
    private DashCounter dashCounter;
    
    // Player life values
    private int lives = maxLives;
    public int Lives
    {
        get { return lives; }
    }
    public static int maxLives = 5;

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
    public static int maxDashes = 3; // Maximum number of dashes that can be held at once
    private static float MAX_DASH_COOLDOWN = 3; // Time required for a dash charge to be gained

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

    // Storage for player velocity when the level is paused
    private Vector3 oldVelocity;

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
    private ParticleSystem hitEffect; // Particle effect for when the player takes damage

    // Cached mouse position for easy access
    private Vector2 mousePos = Vector2.zero;

    /// <summary>
    /// Called when the player is instantiated (before Start). Handles player initialization.
    /// </summary>
    public void Awake()
    {
        // Retrieve references
        field = LevelController.Singleton.Field;
        collider2d = GetComponent<Collider2D>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        hitEffect = GetComponentInChildren<ParticleSystem>();

        // Initializes the renderers
        target = new Vector2(transform.position.x, transform.position.y);
        GameObject targetObject = (GameObject)Instantiate(targetPrefab, Vector3.zero, Quaternion.identity);
        targetRenderer = targetObject.GetComponent<SpriteRenderer>();
        dashRenderer = targetObject.GetComponent<LineRenderer>();
        dashRenderer.sortingOrder = -1;
        dashRenderer.material = new Material(Shader.Find("Particles/Additive"));
        dashRenderer.SetColors(dashStartInactive, dashEndInactive);

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
        StartCoroutine(setInvincible(INVINCIBILITY_ON_SPAWN));
    }

    /// <summary>
    /// Called periodically. Handles the dash mechanic, setting the target, and more.
    /// </summary>
    public void Update()
    {
        // Stores the player's velocity when the level is paused
        if(LevelController.Singleton.Paused && oldVelocity == Vector3.zero)
        {
            oldVelocity = GetComponent<Rigidbody2D>().velocity;
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;

            // Cancel dash targeting on pause
            selecting = false;
            SetMoveTarget(field.WorldPoint(mousePos));
            dashRenderer.enabled = false;
        }
        else if(!LevelController.Singleton.Paused && oldVelocity != Vector3.zero)
        {
            GetComponent<Rigidbody2D>().velocity = oldVelocity;
            oldVelocity = Vector3.zero;
        }

        if(!LevelController.Singleton.Paused)
        {
            HandleInput();
            
            // General movement-related functions
            if(moving)
            {
                targetRenderer.enabled = true;
                moving = Vector2.Distance(transform.position, target) > (dashing ? 1 : 0.1);
                if(!moving)
                {
                    // Player reached its target
                    targetRenderer.enabled = false;
                    dashing = false;
                    rigidbody2d.velocity = Vector3.zero;
                }
            }
            else if(selecting)
            {
                targetRenderer.enabled = true;
            }

            // Handling the dash cooldown
            if(dashes < maxDashes)
            {
                dashCooldown += Time.deltaTime;
                if(dashCooldown >= MAX_DASH_COOLDOWN)
                {
                    dashes++;
                    dashCooldown = 0;
                    dashRenderer.SetColors(dashStartActive, dashEndActive);
                    dashCounter.UpdateCounter(dashes);
                }
            }
        }
	}

    /// <summary>
    /// Called in fixed-time intervals. Handles movement and rotation.
    /// </summary>
    public void FixedUpdate()
    {
        if(!LevelController.Singleton.Paused)
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
        }
    }

    /// <summary>
    /// Called in Update. Handles user input.
    /// </summary>
    private void HandleInput()
    {
        mousePos.x = Input.mousePosition.x / Screen.width;
        mousePos.y = Input.mousePosition.y / Screen.height;

        if(Input.GetMouseButtonDown(0))
        {
            // Begin dash targeting
            selecting = true;
            SetMoveTarget(field.WorldPoint(mousePos));
            dashRenderer.SetPosition(0, transform.position);
            dashRenderer.SetPosition(1, field.WorldPoint(mousePos));
            dashRenderer.enabled = true;
        }
        else if(selecting)
        {
            // Dash targeting
            if(Input.GetMouseButtonUp(0))
            {
                // Begin dash
                if(dashes > 0)
                {
                    SetDashTarget(field.WorldPoint(mousePos));
                    dashes--;
                    if(dashes == 0)
                        dashRenderer.SetColors(dashStartInactive, dashEndInactive);
                }

                // Disable dash selection
                selecting = false;
                SetMoveTarget(field.WorldPoint(mousePos));
                dashRenderer.enabled = false;
                dashCounter.UpdateCounter(dashes);
            }
            else
            {
                targetRenderer.transform.position = field.WorldPoint(mousePos);
                dashRenderer.SetPosition(0, transform.position);
                dashRenderer.SetPosition(1, field.WorldPoint(mousePos));
            }
        }
        else if(!dashing)
        {
            // Normal movement targeting
            SetMoveTarget(field.WorldPoint(mousePos));
        }

        if(Input.GetMouseButtonDown(1))
        {
            // Cancel dash targeting
            selecting = false;
            dashRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Called when the player collides with an enemy or an enemy bullet.
    /// </summary>
    public void Hit()
    {
        lives--;
        livesCounter.UpdateCounter(lives);
        StartCoroutine(setInvincible(INVINCIBILITY_ON_HIT));
        hitEffect.Play();
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
    private IEnumerator setInvincible(float time)
    {
        Renderer renderer = GetComponent<Renderer>();
        Color color = renderer.material.color;
        invincible = true;
        float timer = 0;
        while(timer < time)
        {
            if(!LevelController.Singleton.Paused)
            {
                if(timer % 0.05f > (timer + Time.deltaTime) % 0.05f)
                {
                    color.a = 1.25f - color.a;
                    renderer.material.color = color;
                }
                timer += Time.deltaTime;
            }
            yield return null;
        }
        color.a = 1;
        renderer.material.color = color;
        invincible = false;
        yield break;
    }

    /// <summary>
    /// Called when the player first collides with an object. Handles collision with enemies.
    /// </summary>
    /// <param name="collider">The collider that the player collided with</param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if(enemy != null)
        {
            if(dashing)
            {
                enemy.Damage(50);
            }
            else if(!invincible)
            {
                Hit();
            }
        }
    }

    /// <summary>
    /// Called repeatedly when the player continues to collide with an object. Handles collision with enemies.
    /// </summary>
    /// <param name="collider">The collider that the player collided with</param>
    private void OnTriggerStay2D(Collider2D collider)
    {
        Enemy enemy = collider.gameObject.GetComponent<Enemy>();
        if(enemy != null && !dashing && !invincible)
        {
            Hit();
        }
    }
}
