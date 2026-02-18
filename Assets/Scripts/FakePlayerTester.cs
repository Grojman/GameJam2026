using UnityEngine;
using UnityEngine.InputSystem; // Necesario para crear mandos virtuales

public class FakePlayerTester : MonoBehaviour
{
    [Header("Instrucciones de Testeo")]
    [Tooltip("Activa o desactiva las trampas de testeo")]
    public bool activarTester = true;

    private PlayerInputManager inputManager;

    void Start()
    {
        // Buscamos el manager que se encarga de spawnear a los jugadores
        inputManager = FindFirstObjectByType<PlayerInputManager>();

        if (inputManager == null)
        {
            Debug.LogWarning("No se encontró ningún PlayerInputManager en la escena.");
        }
    }

    void Update()
    {
        if (!activarTester || inputManager == null) return;

        // Si pulsamos la tecla '2' en el teclado, spawneamos un mando falso
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            CrearMandoFantasma();
        }

        // Puedes añadir más teclas si quieres spawnear varios de golpe
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            CrearMandoFantasma();
        }
    }

    private void CrearMandoFantasma()
    {
        // 1. Le decimos a Unity: "Créate un mando de la nada y conéctalo al PC"
        Gamepad mandoVirtual = InputSystem.AddDevice<Gamepad>();

        // 2. Le decimos a tu Manager que una a un jugador usando este nuevo mando
        // El -1, -1 significa que elija la ID de jugador y la variante de control automáticamente
        // "Gamepad" es el nombre del Control Scheme que tienes en tu Input Actions
        inputManager.JoinPlayer(-1, -1, "Gamepad", mandoVirtual);

        Debug.Log($"<color=cyan>¡Mando fantasma conectado! Jugadores actuales: {inputManager.playerCount}</color>");
    }
}