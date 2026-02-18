using UnityEngine;
using TMPro;

public class PlayerCountUI : MonoBehaviour
{
    public TextMeshProUGUI textoUI;
    public int maxJugadores = 4;

    // Tu texto original intacto
    private string textoBase = "Pulsa   <space=20px><voffset=0.25em><sprite=8></voffset>para entrar ";

    public void ActualizarTexto(int jugadoresActuales)
    {
        if (textoUI == null) return;

        // ROJO: M·ximo de jugadores (Ej: 4)
        if (jugadoresActuales >= maxJugadores)
        {
            textoUI.text = textoBase + " <color=#ff3b3c>(M·X)</color>";
        }
        // AMARILLO: Falta 1 jugador (Ej: 3)
        else if (jugadoresActuales == maxJugadores - 1)
        {
            textoUI.text = textoBase + $" <color=yellow>({jugadoresActuales}/{maxJugadores})</color>";
        }
        // BLANCO: El resto (Ej: 0, 1, 2)
        else
        {
            textoUI.text = textoBase + $" <color=white>({jugadoresActuales}/{maxJugadores})</color>";
        }
    }
}