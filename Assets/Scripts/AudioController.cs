using UnityEngine;
using UnityEngine.UI; // Necesario para componentes UI

public class AudioController : MonoBehaviour
{
    public Slider sliderVolumen;
    public AudioSource audioSource;

    void Start()
    {
        // Inicializa el slider con el volumen actual
        sliderVolumen.value = audioSource.volume;
        // Escucha cambios en el slider
        sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
    }

    public void CambiarVolumen(float valor)
    {
        audioSource.volume = valor;
    }
}
