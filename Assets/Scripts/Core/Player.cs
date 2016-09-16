using UnityEngine;
using System.Collections;
using DanmakU;
using System;

public class Player : DanmakuCollider
{
    [SerializeField]
    private GameObject targetPrefab;

    private DanmakuField field;
    private Collider2D collider2d;
    private Rigidbody2D rigidbody2d;

    private LivesCounter livesCounter;
    private DashCounter dashCounter;
    
    private int lives = maxLives;
    public int Lives
    {
        get { return lives; }
    }
    public static int maxLives = 5;

    private bool moving;
    public bool IsMoving
    {
        get { return moving; }
    }

    private bool dashing = false;
    public bool IsDashing
    {
        get { return dashing; }
    }
    private bool selecting = false;
    private int dashes = 0;
    private float dashCooldown = 0;
    public static int maxDashes = 3;
    private static float MAX_DASH_COOLDOWN = 3;

    private bool invincible = false;
    public bool IsInvincible
    {
        get { return invincible; }
    }
    private static readonly float INVINCIBILITY_ON_SPAWN = 0;
    private static readonly float INVINCIBILITY_ON_HIT = 2;

    [SerializeField]
    private float moveSpeed = 360;
    [SerializeField]
    private float dashSpeed = 720;
    [SerializeField]
    private float rotateSpeed = 18;

    [SerializeField]
    private Color dashStartActive;
    [SerializeField]
    private Color dashEndActive;
    [SerializeField]
    private Color dashStartInactive;
    [SerializeField]
    private Color dashEndInactive;

    private Vector2 target;
    private SpriteRenderer targetRenderer;
    private LineRenderer dashRenderer;
    private ParticleSystem hitEffect;

    private Vector2 mousePos = Vector2.zero;
    
    public override void Awake()
    {
        base.Awake();
        TagFilter = "Enemy";

        field = LevelController.Singleton.Field;
        collider2d = GetComponent<Collider2D>();
        rigidbody2d = GetComponent<Rigidbody2D>();

        target = new Vector2(transform.position.x, transform.position.y);
        GameObject targetObject = (GameObject)Instantiate(targetPrefab, Vector3.zero, Quaternion.identity);
        targetRenderer = targetObject.GetComponent<SpriteRenderer>();
        dashRenderer = targetObject.GetComponent<LineRenderer>();
        dashRenderer.sortingOrder = -1;
        dashRenderer.material = new Material(Shader.Find("Particles/Additive"));
        dashRenderer.SetColors(dashStartInactive, dashEndInactive);
        hitEffect = GetComponentInChildren<ParticleSystem>();

        foreach(GameObject counter in GameObject.FindGameObjectsWithTag("Counter"))
        {
            if(counter.GetComponent<LivesCounter>() != null)
                livesCounter = counter.GetComponent<LivesCounter>();
            if(counter.GetComponent<DashCounter>() != null)
                dashCounter = counter.GetComponent<DashCounter>();
        }
    }

    public void Start()
    {
        StartCoroutine(setInvincible(INVINCIBILITY_ON_SPAWN));
    }
	
	public void Update()
    {
	    if(!LevelController.Singleton.Paused)
        {
            HandleInput();
            
            // Stop movement
            if(moving)
            {
                targetRenderer.enabled = true;
                moving = Vector2.Distance(transform.position, target) > (dashing ? 1 : 0.1);
                if(!moving)
                {
                    targetRenderer.enabled = false;
                    dashing = false;
                    rigidbody2d.velocity = Vector3.zero;
                }
            }
            else if(selecting)
            {
                targetRenderer.enabled = true;
            }

            // Dash cooldown
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

    public void FixedUpdate()
    {
        if(!LevelController.Singleton.Paused)
        {
            // Normal movement and rotation
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
                if(dashes > 0)
                {
                    dashing = true;
                    dashes--;
                    if(dashes == 0)
                        dashRenderer.SetColors(dashStartInactive, dashEndInactive);
                }
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

    public void Hit()
    {
        lives--;
        livesCounter.UpdateCounter(lives);
        StartCoroutine(setInvincible(INVINCIBILITY_ON_HIT));
        hitEffect.Play();
    }

    private void SetMoveTarget(Vector2 target)
    {
        moving = true;
        targetRenderer.transform.position = target;
        this.target = BoundsUtil.VerifyBounds(target, new Bounds2D(collider2d.bounds), field.MovementBounds);
    }

    public IEnumerator setInvincible(float time)
    {
        Renderer renderer = GetComponent<Renderer>();
        Color color = renderer.material.color;
        invincible = true;
        float timer = 0;
        while(timer < time)
        {
            if(!LevelController.Singleton.Paused)
            {
                if(timer / 0.05f != (timer + Time.deltaTime) / 0.05f)
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

    protected override void DanmakuCollision(Danmaku danmaku, RaycastHit2D info)
    {
        danmaku.Deactivate();
        if(!dashing && !invincible)
        {
            Hit();
        }
    }
}
