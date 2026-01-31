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
        Debug.Log("pito");
        SceneManager.LoadScene("Login");
    }

    public void Opciones()
    {
        Debug.Log("pito");
        panelOpciones.SetActive(true);
        firstOptions.Select();
    }

    public void Creditos()
    {
        panelCreditos.SetActive(true);
        Debug.Log("pito");
        firstCredits.Select();
    }

    public void OpcionesSalir()
    {
        panelOpciones.SetActive(false);
        firstMain.Select();
    }

    public void CreditosSalir()
    {
        panelCreditos.SetActive(false);
        firstMain.Select();
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}
