using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonActions : MonoBehaviour
{
    [SerializeField] GameObject panelOpciones;
    [SerializeField] GameObject panelCreditos;
    [SerializeField] Button firstMain;
    [SerializeField] Button firstCredits;
    [SerializeField] Button firstOptions;
    public void Jugar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Login");
    }

    public void Opciones()
    {
        Time.timeScale = 1f;
        panelOpciones.SetActive(true);
        firstOptions.Select();
    }

    public void Creditos()
    {
        Time.timeScale = 1f;
        panelCreditos.SetActive(true);
        firstCredits.Select();
    }

    public void OpcionesSalir()
    {
        Time.timeScale = 1f;
        panelOpciones.SetActive(false);
        firstMain.Select();
    }

    public void CreditosSalir()
    {
        Time.timeScale = 1f;
        panelCreditos.SetActive(false);
        firstMain.Select();
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}
