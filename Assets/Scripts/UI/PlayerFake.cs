using UnityEngine;
using DanmakU;

/// <summary>
/// The player. Follows the mouse cursor and performs a dash when left click is held and released.
/// </summary>
public class PlayerFake : MonoBehaviour
{
    // Visual indicator that shows where the enemy is headed
    [SerializeField]
    private GameObject targetPrefab;

    [SerializeField]
    private DanmakuField field;
    private Collider2D collider2d;
    private Rigidbody2D rigidbody2d;

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

    // Player movement values
    [SerializeField]
    private float moveSpeed = 360;
    [SerializeField]
    private float dashSpeed = 720;
    [SerializeField]
    private float rotateSpeed = 18;

    // Forces the player to stay at a location
    private Vector2 forcedTarget;
    private float forcedTargetTimer;
    private readonly float MAX_FORCED_TARGET_TIMER = 0.25f;

    // Dash selection line colors
    [SerializeField]
    private Color dashStart;
    [SerializeField]
    private Color dashEnd;

    private Vector2 target; // Location that the enemy is moving towards
    private SpriteRenderer targetRenderer; // Renders the movement target
    private LineRenderer dashRenderer; // Renders the dash selection line
    private SpriteRenderer spriteRenderer; // Renders the ship
    private SpriteRenderer hitboxGlowRenderer; // Renders the glow effect for the hitbox
    private SpriteRenderer wingsGlowRenderer; // Renders the glow effect for the wings
    private Animator trailAnimator; // Animates the ship trail

    private float currentAlphaHitbox = 0f;
    private float currentAlphaWings = 0f;
    private float targetAlphaHitbox = 0f;
    private float targetAlphaWings = 0f;
    private float deltaAlphaWings = 0f; // Speed at which the wing alpha value moves towards its target

    // Cached mouse position for easy access
    private Vector2 mousePos = Vector2.zero;

    /// <summary>
    /// Called when the player is instantiated (before Start). Handles player initialization.
    /// </summary>
    public void Awake()
    {
        // Retrieve references
        collider2d = GetComponent<Collider2D>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        trailAnimator = transform.FindChild("Trail").GetComponent<Animator>();
        
        // Initializes the renderers
        target = new Vector2(transform.position.x, transform.position.y);
        GameObject targetObject = (GameObject)Instantiate(targetPrefab, new Vector3(0, 0, 100), Quaternion.identity);
        targetObject.transform.SetParent(transform.parent);
        targetRenderer = targetObject.GetComponent<SpriteRenderer>();
        dashRenderer = targetObject.GetComponent<LineRenderer>();
        dashRenderer.sortingOrder = -1;
        dashRenderer.material = new Material(Shader.Find("Particles/Additive"));
        dashRenderer.startColor = dashStart;
        dashRenderer.endColor = dashEnd;
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitboxGlowRenderer = transform.FindChild("GlowHitbox").GetComponent<SpriteRenderer>();
        wingsGlowRenderer = transform.FindChild("GlowWings").GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Called periodically. Handles the dash mechanic, setting the target, and more.
    /// </summary>
    public void Update()
    {
        HandleInput();

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

        // Hold player for a short duration after mouse leaves the button
        if(forcedTargetTimer > 0)
        {
            forcedTargetTimer = Mathf.Max(forcedTargetTimer - Time.deltaTime, 0);
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

        // Disable player if the state changed
        if(Menu.Instance.StateChanged)
        {
            Menu.Instance.StateChanged = false;
            ResetState();
            if(Menu.Instance.CurrentState == Menu.State.LevelSelect)
                gameObject.SetActive(false);
        }
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
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward,
                    (Vector3)target - transform.position), Time.fixedDeltaTime * rotateSpeed);
            }
        }
        else if(forcedTargetTimer > 0)
        {
            float distance = Vector2.Distance(transform.position, target);
            if(distance < 1)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward,
                (Vector3)mousePos - transform.position), Time.fixedDeltaTime * rotateSpeed);
        }

        // Rotate to mouse if selecting
        if(selecting)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(
                Vector3.forward, targetRenderer.transform.position - transform.position), Time.fixedDeltaTime * rotateSpeed);
        }
    }

    /// <summary>
    /// Called in Update. Handles user input.
    /// </summary>
    private void HandleInput()
    {
        // Calculate mouse position in world coordinates
        mousePos.x = Input.mousePosition.x / Screen.width;
        mousePos.y = Input.mousePosition.y / Screen.height;
        mousePos = field.WorldPoint(mousePos);

        if(Input.GetMouseButtonDown(0) && !dashing && forcedTargetTimer == 0)
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
        }
        else if(selecting)
        {
            // Dash targeting
            if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(1))
            {
                // Begin dash
                if(Input.GetMouseButtonUp(0))
                {
                    SetDashTarget(mousePos);
                }

                // Disable dash selection
                selecting = false;
                targetAlphaWings = 0f;
                deltaAlphaWings = 3f;
                trailAnimator.SetBool("Active", false);
                SetMoveTarget(mousePos);
                dashRenderer.enabled = false;
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
            if(forcedTargetTimer > 0)
                SetMoveTarget(forcedTarget);
            else
                SetMoveTarget(mousePos);
        }
    }

    /// <summary>
    /// Sets the movement target and the target location. Note that the two may not be the same.
    /// The visual target can be out of bounds but the target location cannot.
    /// </summary>
    public void SetMoveTarget(Vector2 target)
    {
        moving = true;
        targetRenderer.transform.position = new Vector3(target.x, target.y, 100);
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
    /// Sets whether or not the player should be forced to stay at a specific location, and what that location may be.
    /// </summary>
    /// <param name="active">Whether the sprite should be forced to stay at a location or not</param>
    /// <param name="target">The location that the sprite should be forced to stay at</param>
    public void SetForcedMoveTarget(bool active, Vector2 target = new Vector2())
    {
        if(!dashing)
        {
            if(active)
            {
                forcedTarget = target;
                forcedTargetTimer = MAX_FORCED_TARGET_TIMER;
            }
        }
    }

    /// <summary>
    /// Resets the ship. For use when the menu changes states.
    /// </summary>
    private void ResetState()
    {
        // Disable dash selection
        selecting = false;
        targetAlphaWings = 0f;
        deltaAlphaWings = 3f;
        trailAnimator.SetBool("Active", false);
        SetMoveTarget(mousePos);
        dashRenderer.enabled = false;

        targetRenderer.enabled = false;
        
        SetForcedMoveTarget(false);
        forcedTargetTimer = 0f;
    }
}