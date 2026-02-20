using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LaunchPad : MonoBehaviour
{
    public FadeInOut fadeInOut;

    [Header("UI Elementos")]
    public Image fillImage; // Sustituye al Slider original
    public GameObject contenedorBarra;
    public TextMeshProUGUI label;
    public TextMeshProUGUI statusText;

    [Header("Configuraci�n")]
    public int MaxPlayers;
    public int CurrentPlayers;

    bool startCountDown = false;
    float countDown = 0f;
    const float COUNT_DOWN = 5f;

    void Start()
    {
        fadeInOut = FindObjectOfType<FadeInOut>();
        UpdateLabel();
    }

    void Update()
    {
        if (startCountDown)
        {
            // L�gica ORIGINAL tuya: la cuenta atr�s avanza
            countDown += Time.deltaTime;

            // L�gica ORIGINAL tuya: la barra se va llenando en base a los 5 segundos
            if (fillImage != null)
            {
                fillImage.fillAmount = countDown / COUNT_DOWN;
            }

            // L�gica ORIGINAL tuya: cambio de escena a los 5 segundos
            if (countDown >= COUNT_DOWN)
            {
                startCountDown = false;

                fadeInOut.SetOut(() =>
                {
                    foreach (PlayerInput pl in Data_Static.playerList)
                    {
                        if (pl != null && pl.GetComponent<SpriteRenderer>() != null)
                        {
                            pl.GetComponent<SpriteRenderer>().enabled = false;
                        }
                    }

                    SceneManager.LoadScene("Scenaries");
                });
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            CurrentPlayers++;
            UpdateLabel();

            // L�gica ORIGINAL tuya + comprobaci�n de que al menos haya 2 jugadores
            if (CurrentPlayers == MaxPlayers && MaxPlayers >= 2)
            {
                startCountDown = true;
                countDown = 0;
            }
        }
    }

    // --- RESTAURADO: Tu Reset original intocable ---
    public void Reset()
    {
        UpdateLabel();
        if (fillImage != null) fillImage.fillAmount = 0;
        countDown = 0;
        startCountDown = false;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            CurrentPlayers--;
            if (CurrentPlayers < 0) CurrentPlayers = 0; // Peque�a seguridad

            UpdateLabel();
            if (fillImage != null) fillImage.fillAmount = 0;
            countDown = 0;
            startCountDown = false;
        }
    }

    public void UpdateLabel()
    {
        if (label != null)
        {
            label.text = $"{CurrentPlayers} / {MaxPlayers}";
        }

        // Controlamos los textos nuevos
        if (statusText != null)
        {
            if (MaxPlayers >= 2)
                statusText.text = "�Comienza la Batalla!";
            else
                statusText.text = "M�nimo 2 jugadores";
        }

        // Ocultar/Mostrar contenedor (solo se muestra si hay 1+ encima y 2+ en total)
        if (contenedorBarra != null)
        {
            contenedorBarra.SetActive(MaxPlayers >= 2 && CurrentPlayers > 0);
        }
    }
}