using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Posiciones de Inicio")]
    public Transform[] spawnPoints;

    [Header("Colores por Jugador (Opcional visual)")]
    public Color[] playerColors;

    // Esta función se llamará automáticamente cuando el PlayerInputManager detecte un mando nuevo
    public void OnPlayerJoined(PlayerInput newPlayer)
    {
        int index = newPlayer.playerIndex;

        Debug.Log(index);        
        Debug.Log($"¡Jugador {index + 1} se ha unido con el mando: {newPlayer.devices[0].displayName}!");

        //Colocar al jugador en su punto de spawn
        if (spawnPoints.Length > index)
        {
            newPlayer.transform.position = spawnPoints[index].position;
        }

        //Cambiar el color para diferenciarlos
        SpriteRenderer sr = newPlayer.GetComponent<SpriteRenderer>();
        if (sr != null && playerColors.Length > index)
        {
            sr.color = playerColors[index];
        }

        GameObject pad = GameObject.FindGameObjectWithTag("LaunchPad");

        pad.GetComponent<LaunchPad>().MaxPlayers++;
        pad.GetComponent<LaunchPad>().UpdateLabel();

        // Aqui en se estableceran las vidas etc
    }

    public void OnPlayerLeft(PlayerInput leftPlayer)
    {
        Debug.Log($"Jugador {leftPlayer.playerIndex + 1} se ha desconectado.");
    }
}