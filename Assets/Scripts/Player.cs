using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public LayerMask HitMask;
    public Action<Player> attackAction; 
    Image timeFill;
    Mask? currentMask;
    float maskTimer = 0f;
    bool maskTimerActive = true;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        transform.position = SpawnPoint.position;
        HitPoints = DEFAULT_HIT_POINTS;
    }

    // Update is called once per frame
    void Update()
    {
        if (maskTimerActive) HandleTimeBar();

        input = playerInput.actions["Move"].ReadValue<Vector2>();
        AttackDirection = playerInput.actions["Aim"].ReadValue<Vector2>();
        movement = new Vector2(Time.deltaTime * Speed * input.x, 0);
    }

    void HandleTimeBar()
    {
        maskTimer -= Time.deltaTime;

        if (maskTimer <= 0)
        {
            maskTimer = 0;
            maskTimerActive = false;
            currentMask.Close(this);
        } else
        {
            timeFill.fillAmount = maskTimer / currentMask.TimeMask;
        }
    }

    void FixedUpdate()
    {
        rg.AddForce(movement);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            rg.AddForce(Vector2.up * JumpForce);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (attackAction is not null) attackAction(this);
            else DefaultAttack();
            //Si tiene una mascara que cambie su ataque, pues eso
        }
    }

    public void GetMask(Mask mask)
    {
        currentMask = mask;
        maskTimer = mask.TimeMask;
        maskTimerActive = true;
        mask.Get(this);   
    }

    void DefaultAttack()
    {
        Collider2D hit = Physics2D.OverlapBox(
            AttackDirection,
            AttackSize,
            0f,
            HitMask
        );

        if (hit != null)
        {
            Player p = hit.GetComponent<Player>();

            if(p != null)
            {
                p.Hit(AttackDamage);
            }

        }

    }

    public void Hit(int hitPoints)
    {
        this.HitPoints -= hitPoints;

        if (HitPoints == 0)
        {
            //TODO: HACER ALGO
        }
    }
}
