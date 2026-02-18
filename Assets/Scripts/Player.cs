using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Audio;

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
    bool cannotGetMask = false;
    float cannotGetMaskTimer = 0f;
    float cannotGetMaskCooldown = 3f;
    public bool canChange;
    public bool FamilyFriendly;
    public PlayerSpawnManager psManager;
    public float DashSpeed = 5f;
    public GameObject head;
    public SpriteRenderer body;
    public bool Alive = true;
    public SpriteRenderer face;
    Sprite saveFace;
    float beingPushedTimer = 0;
    public float beingPushedCooldown = 0.35f;
    bool IAmBeingPushed = false;
    public float pushCooldown = 0.75f;
    float pushTimer = 0;
    bool isPushOnCooldown = false;

    public float punchCooldown = 0.5f;
    float punchTimer = 0;
    bool isPunchOnCooldown = false;

    [Header("UI: Vida y Tiempo (Fill Amount)")]
    public GameObject healthBarContainer; // Objeto padre de la vida (Para apagarlo entero)
    public Image healthBarFill;           // Reemplaza al Slider de vida
    public GameObject timeBarContainer;   // Objeto padre de la máscara
    public Image timeBarFill;             // Reemplaza al Slider de tiempo

    public float ActionCooldown = 5f;
    public float KnocBackForce = 5f;
    const float HURT_BOX_POS_X = 1.12f;
    const float HURT_BOX_POS_Y = 0.74f;
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

    Mask? currentMask;
    float maskTimer = 0f;
    bool maskTimerActive = false;
    public int AttackDamage = 1;
    public Vector2 AttackSize = new Vector2(1f, 1f);
    public Vector2 AttackDirection;
    public Rigidbody2D rg;
    public PlayerInput playerInput;
    public Vector2 input;
    public Vector2 movement;
    public const int DEFAULT_HIT_POINTS = 3;
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
    float hitColorTimer = 0f;
    bool isHitted = false;
    float hitColorCooldown = 0.5f;

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

    void Awake()
    {
        canChange = false;
        RestoreColor();
        rg = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        HitPoints = DEFAULT_HIT_POINTS;
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
        float xValue = HURT_BOX_POS_X;
        float yValue = input.y switch
        {
            > 0 => HURT_BOX_POS_Y,
            0 => 0,
            < 0 => -HURT_BOX_POS_Y
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
        if (maskTimerActive) HandleTimeBar();

        input = playerInput.actions["Move"].ReadValue<Vector2>();
        movement = new Vector3(input.x, 0f, input.y) * Speed;

        if (input.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(input.x) * Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            playerCanvas.transform.localScale = new Vector3(Mathf.Sign(input.x) * Math.Abs(playerCanvas.transform.localScale.x), playerCanvas.transform.localScale.y, playerCanvas.transform.localScale.z);
        }

        if (isHitted)
        {
            hitColorTimer -= Time.deltaTime;
            if (hitColorTimer <= 0)
            {
                hitColorTimer = 0;
                isHitted = false;
                RestoreColor();
            }
        }

        if (cannotGetMask)
        {
            cannotGetMaskTimer -= Time.deltaTime;
            if (cannotGetMaskTimer <= 0) cannotGetMask = false;
        }

        PositionHurtBox(input);

        if (isPushOnCooldown)
        {
            pushTimer -= Time.deltaTime;
            if (pushTimer <= 0)
            {
                pushTimer = 0;
                isPushOnCooldown = false;
            }
        }

        if (isPunchOnCooldown)
        {
            punchTimer -= Time.deltaTime;
            if (punchTimer <= 0)
            {
                punchTimer = 0;
                isPunchOnCooldown = false;
            }
        }

        if (IAmBeingPushed)
        {
            beingPushedTimer -= Time.deltaTime;
            if (beingPushedTimer <= 0)
            {
                IAmBeingPushed = false;
                beingPushedTimer = 0;
            }
        }

        animator.SetBool("Jumping", !grounded);
        animator.SetFloat("AnimationSpeed", Math.Abs(input.x));
    }

    public void ForceRemoveMask()
    {
        if (currentMask != null)
        {
            cannotGetMask = true;
            cannotGetMaskTimer = cannotGetMaskCooldown;
            maskTimer = 0;
            maskTimerActive = false;

            if (timeBarContainer != null) timeBarContainer.SetActive(false);
            else if (timeBarFill != null) timeBarFill.gameObject.SetActive(false);

            currentMask.Close(this);
            face.sprite = saveFace;
            currentMask.transform.position = new Vector2(transform.position.x, transform.position.y);
            currentMask.Show();
            currentMask = null;
        }
    }

    void HandleTimeBar()
    {
        maskTimer -= Time.deltaTime;

        if (maskTimer <= 0)
        {
            ForceRemoveMask();
        }
        else
        {
            if (timeBarFill != null) timeBarFill.fillAmount = maskTimer / currentMask.TimeMask;
        }
    }

    void FixedUpdate()
    {
        if (!IAmBeingPushed && Alive)
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

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && Alive)
        {
            IAmBeingPushed = true;
            beingPushedTimer = beingPushedCooldown;
            rg.AddForce(input * DashSpeed, ForceMode2D.Impulse);
        }
    }

    public void Push(InputAction.CallbackContext context)
    {
        if (context.started && !isPushOnCooldown && Alive)
        {
            audioSource.PlayOneShot(pushSound);
            isPushOnCooldown = true;
            pushTimer = pushCooldown;
            animator.SetTrigger("Push");

            foreach (Player p in hurtPlayer.hittingPlayers)
            {
                Vector2 force = new Vector2(
                    input.x switch { > 0 => 1, 0 => playerCanvas.transform.localScale.x, < 0 => -1 },
                    input.y switch { > 0 => 1, 0 => 0, < 0 => -1 }
                );
                p.IAmBeingPushed = true;
                p.beingPushedTimer = p.beingPushedCooldown;
                p.rg.AddForce(force * KnocBackForce, ForceMode2D.Impulse);
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isPunchOnCooldown && Alive && !FamilyFriendly)
        {
            audioSource.PlayOneShot(punchSound);
            isPunchOnCooldown = true;
            punchTimer = punchCooldown;

            if (attackAction is not null) attackAction(this);
            else DefaultAttack();
        }
    }

    public void GetMask(Mask mask)
    {
        if (!maskTimerActive && !cannotGetMask)
        {
            audioSource.PlayOneShot(getMaskSound);
            currentMask = mask;
            currentMask.Hide();
            saveFace = face.sprite;
            face.sprite = mask.GetSprite();
            maskTimer = mask.TimeMask;
            maskTimerActive = true;

            if (timeBarContainer != null) timeBarContainer.SetActive(true);
            else if (timeBarFill != null) timeBarFill.gameObject.SetActive(true);

            mask.Get(this);
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
        if (healthBarFill != null) healthBarFill.fillAmount = (float)HitPoints / DEFAULT_HIT_POINTS;
    }

    public void Hit(int hitPoints)
    {
        isHitted = true;
        hitColorTimer = hitColorCooldown;
        SetRedHitColor();

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
        HitPoints = DEFAULT_HIT_POINTS;
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