using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Necesario para componentes UI

public class AudioController : MonoBehaviour
{
    public AudioMixer mixer;

    public void Start()
    {
        // Inicializa el slider con el volumen actual
    }

    public void ControlMusica(float sliderValue)
    {
        mixer.SetFloat("VolumenMusica", Mathf.Log10(sliderValue) * 20);
    }

    public void ControlSonido(float sliderValue)
    {
        mixer.SetFloat("VolumenSonido",  Mathf.Log10(sliderValue) * 20);
    }

}
