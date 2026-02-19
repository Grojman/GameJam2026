using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.IO;

public class Player : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip burlaSound;
    public AudioClip punchSound;
    public AudioClip pushSound;
    public AudioClip getMaskSound;
    public AudioClip jumpSound;
    public float originalGravity;
    public bool WallSlide = false;
    public bool canChange;
    public bool FamilyFriendly;
    public PlayerSpawnManager psManager;
    public float DashSpeed = 50f;
    public GameObject head;
    public SpriteRenderer body;
    public bool Alive = true;
    public SpriteRenderer face;
    Sprite saveFace;

    [Header("UI: Vida y Tiempo (Fill Amount)")]
    public GameObject healthBarContainer; // Objeto padre de la vida (Para apagarlo entero)
    public Image healthBarFill;           // Reemplaza al Slider de vida
    public GameObject timeBarContainer;   // Objeto padre de la máscara
    public Image timeBarFill;             // Reemplaza al Slider de tiempo

    public float KnocBackForce = 5f;
    public GameObject HurtBox;
    HurtBoxPlayer hurtPlayer;
    public Animator animator;
    bool grounded = true;
    public int MaxJumps = 2;
    int jumpCounter = 0;

    public string name; // <-- OJO: Revisa en el Inspector que esto no sea "Jugador III"
    public TextMeshProUGUI nameVisual;
    public int DeathCount = 0; // <-- OJO: Revisa en el Inspector que esto sea 0

    public LayerMask HitMask;
    public Action<Player> attackAction;

    
    public int AttackDamage = 1;
    public Vector2 AttackSize = new Vector2(1f, 1f);
    public Vector2 AttackDirection;
    public Rigidbody2D rg;
    public PlayerInput playerInput;
    public Vector2 input;
    public Vector2 movement;
    public float Speed = 5f;
    public float Damage = 5f;
    public float JumpForce = 5f;
    public int HitPoints;
    public Transform SpawnPoint;
    Canvas playerCanvas;
    public List<UnityEngine.Color> colors;
    public int color = 0;

    public float fallMultiplier = 2.5f;
    public float maxFallSpeed = 20f;
    public float defaultGravity;

    public Color hitColor = Color.red;


    SimpleTimer beingPushedTimer;
    SimpleTimer pushTimer;
    SimpleTimer punchTimer;
    SimpleTimer hitTimer;
    SimpleTimer cannotGetMaskTimer;
    SimpleTimer maskTimer;
    SimpleTimer dashTimer;
    SimpleTimer dashCooldownTimer;
    Mask? currentMask;
    Mask? hittingMask;

    int defaultHitPoints;
    float hurtBoxPosX;
    float hurtBoxPosY;
    ConfigManager config;

    static float GetDeltaTime() => Time.deltaTime;

    void Awake()
    {
        config = new(Path.Combine(Application.streamingAssetsPath, "Config", "player_config.txt"));

        defaultHitPoints = config.Get<int>("DEFAULT_HIT_POINTS");
        hurtBoxPosX = config.Get<float>("HurtBoxPosX");
        hurtBoxPosY = config.Get<float>("HurtBoxPosY");

        Speed = config.Get<float>("Speed", Speed);
        JumpForce = config.Get<float>("JumpForce", JumpForce);
        DashSpeed = config.Get<float>("DashSpeed", DashSpeed);
        MaxJumps = config.Get<int>("MaxJumps", MaxJumps);
        fallMultiplier = config.Get<float>("FallMultiplier", fallMultiplier);
        maxFallSpeed = config.Get<float>("MaxFallSpeed", maxFallSpeed);

        dashTimer = new(config.Get<float>("DashTime"), 0, GetDeltaTime)
        {
            OnEnd = () => {
                rg.gravityScale = defaultGravity;
                dashCooldownTimer.Start();
            }
        };
        dashCooldownTimer = new(config.Get<float>("DashCooldown"), 0, GetDeltaTime);
        beingPushedTimer = new(config.Get<float>("BeingPushedCooldown"), 0, GetDeltaTime);
        pushTimer = new(config.Get<float>("PushCooldown"), 0, GetDeltaTime);
        hitTimer = new(config.Get<float>("HitColorCooldown"), 0, GetDeltaTime)
        {
            OnEnd = RestoreColor,
            OnStart = SetRedHitColor  
        };
        cannotGetMaskTimer = new(config.Get<float>("CannotGetMaskCooldown"), 0, GetDeltaTime)
        {
            OnEnd = UseMaskIfStillTouchingIt  
        };
        punchTimer = new(config.Get<float>("PunchCooldown"), 0, GetDeltaTime);

        canChange = false;
        RestoreColor();
        rg = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        HitPoints = defaultHitPoints;
        playerCanvas = GetComponentInChildren<Canvas>();

        SetNameVisual(); // Ponemos el nombre limpio

        animator = GetComponent<Animator>();
        animator.SetFloat("AnimationSpeed", Speed);

        hurtPlayer = HurtBox.GetComponent<HurtBoxPlayer>();
        hurtPlayer.myPlayer = this;

        if (timeBarContainer != null) timeBarContainer.SetActive(false);
        else if (timeBarFill != null) timeBarFill.gameObject.SetActive(false);

        originalGravity = rg.gravityScale;
        audioSource = GetComponent<AudioSource>();

        maskTimer = new(-1, 0, GetDeltaTime)
        {
            OnEnd = ForceRemoveMask
        };
    }

    void SetRedHitColor()
    {
        body.color = hitColor;
        head.GetComponent<SpriteRenderer>().color = hitColor;
    }

    void RestoreColor()
    {
        body.color = colors[color];
        head.GetComponent<SpriteRenderer>().color = colors[color];
    }

    public void EnableFamilyFriendly()
    {
        FamilyFriendly = true;
        if (healthBarContainer != null) healthBarContainer.SetActive(false);
        else if (healthBarFill != null) healthBarFill.gameObject.SetActive(false);
    }

    public void DisableFamilyFriendly()
    {
        FamilyFriendly = false;
        if (healthBarContainer != null) healthBarContainer.SetActive(true);
        else if (healthBarFill != null) healthBarFill.gameObject.SetActive(true);
    }

    public void SwapFace(InputAction.CallbackContext context)
    {
        if (canChange && context.performed)
            face.sprite = psManager.SwapSprite(face.sprite);
    }

    public void CangeSkin(InputAction.CallbackContext context)
    {
        if (canChange && context.performed)
        {
            color++;
            if (color >= colors.Count) color = 0;
            body.color = colors[color];
            head.GetComponent<SpriteRenderer>().color = colors[color];
        }
    }

    public void Taunt(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            audioSource.PlayOneShot(burlaSound);
            animator.SetTrigger("Taunt");
        }
    }

    void PositionHurtBox(Vector2 input)
    {
        float xValue = hurtBoxPosX;
        float yValue = input.y switch
        {
            > 0 => hurtBoxPosY,
            0 => 0,
            < 0 => -hurtBoxPosY
        };
        HurtBox.transform.localPosition = new Vector2(xValue, yValue);
    }

    // --- LÓGICA DE SALTO RESTAURADA A TU ORIGINAL ---
    public void OnTouchGround()
    {
        animator.SetBool("Jumping", !grounded);
        grounded = true;
        jumpCounter = MaxJumps;
    }

    public void OnLeaveGround()
    {
        jumpCounter--;
        animator.SetBool("Jumping", !grounded);
        grounded = false;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Tu condición original exacta
        if (context.performed && (jumpCounter != 0 || grounded) && Alive)
        {
            audioSource.PlayOneShot(jumpSound);
            if (!grounded)
            {
                jumpCounter--;
            }

            // LA MAGIA: Cortamos el grounded al instante para evitar el spam del bug
            grounded = false;
            animator.SetBool("Jumping", true);

            rg.linearVelocity = new Vector2(rg.linearVelocity.x, 0f);
            rg.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }
    }
    // ------------------------------------------------

    void Update()
    {
        dashTimer.Update();
        dashCooldownTimer.Update();
        maskTimer.Update();

        if (maskTimer.IsActive && 
            timeBarFill != null &&
            currentMask != null)
        {
            timeBarFill.fillAmount = maskTimer.Timer / currentMask.TimeMask;
        }

        input = playerInput.actions["Move"].ReadValue<Vector2>();
        movement = new Vector3(input.x, 0f, input.y) * Speed;

        if (input.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(input.x) * Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            playerCanvas.transform.localScale = new Vector3(Mathf.Sign(input.x) * Math.Abs(playerCanvas.transform.localScale.x), playerCanvas.transform.localScale.y, playerCanvas.transform.localScale.z);
        }

        hitTimer.Update();

        cannotGetMaskTimer.Update();

        PositionHurtBox(input);


        pushTimer.Update();

        punchTimer.Update();

        beingPushedTimer.Update();

        animator.SetBool("Jumping", !grounded);
        animator.SetFloat("AnimationSpeed", Math.Abs(input.x));
    }

    public void ForceRemoveMask()
    {
        if (currentMask != null)
        {
            maskTimer.StopTimer();
            cannotGetMaskTimer.Start();

            if (timeBarContainer != null) timeBarContainer.SetActive(false);
            else if (timeBarFill != null) timeBarFill.gameObject.SetActive(false);

            currentMask.Close(this);
            face.sprite = saveFace;
            currentMask.transform.position = new Vector2(transform.position.x, transform.position.y);
            currentMask.Show();
            currentMask = null;
        }
    }

    void FixedUpdate()
    {
        if(!dashTimer.IsActive)
        {
            if (!beingPushedTimer.IsActive && Alive)
            {
                rg.linearVelocity = new Vector2(input.x * Speed, rg.linearVelocity.y);
            }

            if (rg.linearVelocity.y < 0) rg.gravityScale = defaultGravity * fallMultiplier;
            else rg.gravityScale = defaultGravity;

            float clampedY = Mathf.Max(rg.linearVelocity.y, -maxFallSpeed);
            if (Alive) rg.linearVelocity = new Vector2(rg.linearVelocity.x, clampedY);

            if (WallSlide)
            {
                float maxFallSpeed = -2f;
                if (rg.linearVelocity.y < maxFallSpeed)
                {
                    rg.linearVelocity = new Vector2(rg.linearVelocity.x, maxFallSpeed);
                }
            }
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && Alive && !dashTimer.IsActive && !dashCooldownTimer.IsActive && input.x != 0)
        {
            rg.linearVelocity = new Vector2(Math.Sign(input.x) * DashSpeed, 0);
            rg.gravityScale = 0;
            dashTimer.Start();
        }
    }

    public void Push(InputAction.CallbackContext context)
    {
        if (context.started && !pushTimer.IsActive && Alive)
        {
            audioSource.PlayOneShot(pushSound);
            pushTimer.Start();
            animator.SetTrigger("Push");

            foreach (Player p in hurtPlayer.hittingPlayers)
            {
                Vector2 force = new Vector2(
                    input.x switch { > 0 => 1, 0 => playerCanvas.transform.localScale.x, < 0 => -1 },
                    input.y switch { > 0 => 1, 0 => 0, < 0 => -1 }
                );
                p.beingPushedTimer.Start();
                p.rg.AddForce(force * KnocBackForce, ForceMode2D.Impulse);
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !punchTimer.IsActive && Alive && !FamilyFriendly)
        {
            audioSource.PlayOneShot(punchSound);

            punchTimer.Start();

            if (attackAction is not null) attackAction(this);
            else DefaultAttack();
        }
    }

    public void OnLeaveMask() => hittingMask = null;
    void UseMaskIfStillTouchingIt()
    {
        if (hittingMask != null)
        {
            GetMask(hittingMask);
            hittingMask = null;
        }
    }
    public void GetMask(Mask mask)
    {
        if (!maskTimer.IsActive && !cannotGetMaskTimer.IsActive)
        {
            audioSource.PlayOneShot(getMaskSound);
            currentMask = mask;
            currentMask.Hide();
            saveFace = face.sprite;
            face.sprite = mask.GetSprite();
            maskTimer.StartValue = mask.TimeMask;
            maskTimer.Start();

            if (timeBarContainer != null) timeBarContainer.SetActive(true);
            else if (timeBarFill != null) timeBarFill.gameObject.SetActive(true);

            mask.Get(this);
        } else {
            hittingMask = mask;
        }
    }

    void DefaultAttack()
    {
        animator.SetTrigger("Punch");
        for (int i = 0; i < hurtPlayer.hittingPlayers.Count; i++)
        {
            Player p = hurtPlayer.hittingPlayers[i];
            p.Hit(AttackDamage);
        }
    }

    public void SetHealthBar()
    {
        if (healthBarFill != null) healthBarFill.fillAmount = (float)HitPoints / defaultHitPoints;
    }

    public void Hit(int hitPoints)
    {
        hitTimer.Start();
        HitPoints -= hitPoints;
        SetHealthBar();

        if (HitPoints <= 0) Kill();
    }

    public void SetNameVisual()
    {
        if (nameVisual != null)
        {
            // Si tiene 0 muertes, enseñamos SOLO el nombre, sin huecos raros ni palitos.
            if (DeathCount == 0)
                nameVisual.text = name;
            else
                nameVisual.text = $"{name} {ARomano(DeathCount)}";
        }
    }

    public void Kill()
    {
        DeathCount++;
        SetNameVisual();
        Alive = false;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        head.GetComponent<SpriteRenderer>().enabled = false;
        face.enabled = false;
        playerCanvas.enabled = false;
        rg.bodyType = RigidbodyType2D.Static;
        Data_Static.alivePLayers--;
    }

    public void Revive()
    {
        Alive = true;
        HitPoints = config.Get<int>("Defautl");
        SetHealthBar();
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        head.GetComponent<SpriteRenderer>().enabled = true;
        playerCanvas.enabled = true;
        face.enabled = true;
        rg.bodyType = RigidbodyType2D.Dynamic;
    }

    public static string ARomano(int numero)
    {
        if (numero == 0) return string.Empty;
        if (numero < 1 || numero > 3999) return "Error";

        int[] valores = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
        string[] simbolos = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

        StringBuilder romano = new StringBuilder();

        for (int i = 0; i < valores.Length; i++)
        {
            while (numero >= valores[i])
            {
                numero -= valores[i];
                romano.Append(simbolos[i]);
            }
        }
        return romano.ToString();
    }
}