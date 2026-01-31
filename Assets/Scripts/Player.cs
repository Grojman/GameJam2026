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
    public float beingPushedCooldown = 0.5f;
    bool IAmBeingPushed = false;
    public float pushCooldown = 0.75f;
    float pushTimer = 0;
    bool isPushOnCooldown = false;


    public float punchCooldown = 0.5f;
    float punchTimer = 0;
    bool isPunchOnCooldown = false;


    public Slider HealthSlider;
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
    public string name;
    public TextMeshProUGUI nameVisual;
    public int DeathCount = 0;
    public LayerMask HitMask;
    public Action<Player> attackAction;
    public Slider timebar;
    Mask? currentMask;
    float maskTimer = 0f;
    bool maskTimerActive = false; //Lo pongo en false para pruebas
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

    public float fallMultiplier = 2.5f; // Multiplicador de gravedad al caer
    public float maxFallSpeed = 20f;    // Velocidad máxima de caída (Para que no atraviese el suelo)
    public float defaultGravity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        canChange = false;
        body.color = colors[color];
        head.GetComponent<SpriteRenderer>().color = colors[color];
        rg = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        //transform.position = SpawnPoint.position;
        HitPoints = DEFAULT_HIT_POINTS;
        // AttackDirection = playerInput.actions["Aim"].ReadValue<Vector2>();
        playerCanvas = GetComponentInChildren<Canvas>();
        nameVisual.text = $"{name} {ARomano(DeathCount)}";
        animator = GetComponent<Animator>();
        animator.SetFloat("AnimationSpeed", Speed);

        hurtPlayer = HurtBox.GetComponent<HurtBoxPlayer>();

        hurtPlayer.myPlayer = this;

        timebar.gameObject.SetActive(false);

        originalGravity = rg.gravityScale;

        audioSource = GetComponent<AudioSource>();

    }

    public void EnableFamilyFriendly()
    {
        FamilyFriendly = true;
        HealthSlider.enabled = false;
        HealthSlider.GetComponentInChildren<Image>().enabled = false;
    }

    public void DisableFamilyFriendly()
    {
        FamilyFriendly = false;
        HealthSlider.enabled = true;
        HealthSlider.GetComponentInChildren<Image>().enabled = true;
        
    }

    public void SwapFace(InputAction.CallbackContext context)
    {
        Debug.Log($"{canChange}\n");
        if(canChange && context.performed)
            face.sprite = psManager.SwapSprite(face.sprite);
    }

    public void CangeSkin(InputAction.CallbackContext context)
    {
        Debug.Log($"{canChange}\n");
        if(canChange && context.performed)
        {
            color++;
            if(color >= colors.Count)
                color = 0;
            body.color = colors[color];
            head.GetComponent<SpriteRenderer>().color = colors[color];
        }
    }

    public void Taunt(InputAction.CallbackContext context)
    {
        if(context.started)
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

        var vector = new Vector2(xValue, yValue);

        HurtBox.transform.localPosition = vector;
    }


    public void OnTouchGround()
    {
        Debug.Log("Enter \n");
        animator.SetBool("Jumping", !grounded);
        grounded = true;
        jumpCounter = MaxJumps;
    }

    public void OnLeaveGround()
    {
        Debug.Log("Leave \n");
        jumpCounter--;
        animator.SetBool("Jumping", !grounded);
        grounded = false;
    }


    // Update is called once per frame
    void Update()
    {   
        if (maskTimerActive) HandleTimeBar();

        input = playerInput.actions["Move"].ReadValue<Vector2>();
        // AttackDirection = playerInput.actions["Aim"].ReadValue<Vector2>();
        movement = new Vector3(input.x, 0f, input.y) * Speed;

        if (input.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(input.x) * Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            playerCanvas.transform.localScale = new Vector3(Mathf.Sign(input.x) * Math.Abs(playerCanvas.transform.localScale.x), playerCanvas.transform.localScale.y,
                playerCanvas.transform.localScale.z);
        }

        if (cannotGetMask)
        {
            cannotGetMaskTimer -= Time.deltaTime;

            if(cannotGetMaskTimer <= 0)
            {
                cannotGetMask = false;
            }
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

        if(isPunchOnCooldown)
        {
            punchTimer -= Time.deltaTime;

            if (punchTimer <= 0)
            {
                punchTimer = 0;
                isPunchOnCooldown = false;
            }
        }

        if(IAmBeingPushed)
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
            timebar.gameObject.SetActive(false);
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

        } else
        {
            timebar.value = maskTimer / currentMask.TimeMask;
            // timeFill.fillAmount = maskTimer / currentMask.TimeMask;
        }
    }

    void FixedUpdate()
    {
        if (!IAmBeingPushed && Alive)
        {
            rg.linearVelocity = new Vector2(input.x * Speed, rg.linearVelocity.y);
        }

        //Gravedad dinámica

        if (rg.linearVelocity.y < 0)
        {
            rg.gravityScale = defaultGravity * fallMultiplier;
        }
        else
        {
            rg.gravityScale = defaultGravity;
        }

        float clampedY = Mathf.Max(rg.linearVelocity.y, -maxFallSpeed);
        if (Alive) rg.linearVelocity = new Vector2(rg.linearVelocity.x, clampedY);

        if (WallSlide)
        {
            float maxFallSpeed = -2f; // ajusta a gusto
            if (rg.linearVelocity.y < maxFallSpeed)
            {
                rg.linearVelocity = new Vector2(rg.linearVelocity.x, maxFallSpeed);
            }
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(context.started && Alive)
        {
            IAmBeingPushed = true;
            beingPushedTimer = beingPushedCooldown;
            
            rg.AddForce(input * DashSpeed, ForceMode2D.Impulse);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log($"{jumpCounter}\n");
        if(context.performed && (jumpCounter != 0 || grounded) && Alive)
        {
            audioSource.PlayOneShot(jumpSound);
            if (!grounded)
            {
                jumpCounter--;
            }
            rg.linearVelocity = new Vector2(rg.linearVelocity.x, 0f);
            rg.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
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
            foreach(Player p in hurtPlayer.hittingPlayers)
            {
                
                Vector2 force = new Vector2(
                    input.x switch
                    {
                        > 0 => 1,
                        0 => playerCanvas.transform.localScale.x,
                        < 0 => -1
                    },
                    input.y switch
                    {
                        > 0 => 1,
                        0 => 0,
                        < 0 => -1
                    }
                );
                Debug.Log("Aplicando fuerza a p\n");
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

            Debug.Log("Atacando\n");

            if (attackAction is not null) attackAction(this);
            else DefaultAttack();
            //Si tiene una mascara que cambie su ataque, pues eso
        }
    }

    public void GetMask(Mask mask)
    {
        if(!maskTimerActive && !cannotGetMask)
        {
            audioSource.PlayOneShot(getMaskSound);

            currentMask = mask;
            currentMask.Hide();
            saveFace = face.sprite;
            face.sprite = mask.GetSprite();
            maskTimer = mask.TimeMask;
            maskTimerActive = true;
            timebar.gameObject.SetActive(true);
            mask.Get(this); 
        }
    }

    void DefaultAttack()
    {
        animator.SetTrigger("Punch");
        for(int i = 0; i < hurtPlayer.hittingPlayers.Count; i++)
        {
            Player p = hurtPlayer.hittingPlayers[i];
            p.Hit(AttackDamage);
        }
    }

    public void SetHealthBar() => HealthSlider.value = (float)((float)HitPoints / DEFAULT_HIT_POINTS);

    public void Hit(int hitPoints)
    {
        Debug.Log("Hitted\n");
        HitPoints -= hitPoints;
        HealthSlider.value = (float)((float)HitPoints / DEFAULT_HIT_POINTS);

        if (HitPoints <= 0)
        {
            Kill();
        }
    }

    public void SetNameVisual() => nameVisual.text = $"{name} {ARomano(DeathCount)}";

    public void Kill()
    {
        DeathCount++;
        nameVisual.text = $"{name} {ARomano(DeathCount)}";
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
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        head.GetComponent<SpriteRenderer>().enabled = true;
        playerCanvas.enabled = true;
        face.enabled = true;
        rg.bodyType = RigidbodyType2D.Dynamic;
    }

    public static string ARomano(int numero)
    {
        if(numero == 0) return string.Empty;
        if (numero <= 1 || numero > 3999)
            return "Número fuera de rango (1-3999)";

        // Pares de valores y sus símbolos romanos
        int[] valores = {1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1};
        string[] simbolos = {"M","CM","D","CD","C","XC","L","XL","X","IX","V","IV","I"};

        StringBuilder romano = new StringBuilder();

        for(int i = 0; i < valores.Length; i++)
        {
            while(numero >= valores[i])
            {
                numero -= valores[i];
                romano.Append(simbolos[i]);
            }
        }

        return romano.ToString();
    }
}
