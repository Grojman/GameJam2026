using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int DeathCount = 0;
    public LayerMask HitMask;
    public Action<Player> attackAction;
    public Image timebar;
    public Image timeFill;
    Mask? currentMask;
    float maskTimer = 0f;
    bool maskTimerActive = true; //Lo pongo en false para pruebas
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
    void Awake()
    {
        rg = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        //transform.position = SpawnPoint.position;
        HitPoints = DEFAULT_HIT_POINTS;
        AttackDirection = playerInput.actions["Aim"].ReadValue<Vector2>();
    }


    // Update is called once per frame
    void Update()
    {
        if (maskTimerActive) HandleTimeBar();

        input = playerInput.actions["Move"].ReadValue<Vector2>();
        AttackDirection = playerInput.actions["Aim"].ReadValue<Vector2>();
        movement = new Vector3(input.x, 0f, input.y) * Speed;
    }

    void HandleTimeBar()
    {
        maskTimer -= Time.deltaTime;

        if (maskTimer <= 0)
        {
            maskTimer = 0;
            maskTimerActive = false;
            timebar.enabled = false;
            currentMask.Close(this);
        } else
        {
            timeFill.fillAmount = maskTimer / currentMask.TimeMask;
        }
    }

    void FixedUpdate()
    {
        rg.linearVelocity = new Vector2(input.x * Speed, rg.linearVelocity.y);
        //rg.linearVelocity = new Vector2(movement.x, rg.linearVelocity.y);
        /*movement = new Vector3(input.x,0f, input.y) * Speed;
        rg.AddForce(movement);*/
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            rg.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

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
        timebar.enabled = true;
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

    public static string ARomano(int numero)
    {
        if (numero <= 0 || numero > 3999)
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
