using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using UnityEditor;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Posiciones de Inicio")]
    public Transform[] spawnPoints;

    [Header("Caras por Jugador (Opcional visual)")]
    [SerializeField] Sprite[] sprites;
    private System.Random rng = new System.Random();


    public void Start()
    {
        Shuffle<Sprite>(sprites);
    }

    public void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // Esta función se llamará automáticamente cuando el PlayerInputManager detecte un mando nuevo
    public void OnPlayerJoined(PlayerInput newPlayer)
    {
        DontDestroyOnLoad(newPlayer.gameObject);
        int index = newPlayer.playerIndex;
        Data_Static.playerList.Add(newPlayer);
        Debug.Log(index);        
        Debug.Log($"¡Jugador {index + 1} se ha unido con el mando: {newPlayer.devices[0].displayName}!");

        //Colocar al jugador en su punto de spawn
        if (spawnPoints.Length > index)
        {
            newPlayer.transform.position = spawnPoints[index].position;
        }

        //Cambiar la cara para diferenciarlos
        Player player = newPlayer.GetComponent<Player>();
        if (player != null && sprites.Length > index)
        {
            player.face.sprite = sprites[index];
        }

        GameObject pad = GameObject.FindGameObjectWithTag("LaunchPad");

        pad.GetComponent<LaunchPad>().MaxPlayers++;
        pad.GetComponent<LaunchPad>().UpdateLabel();

        GameObject.FindGameObjectWithTag("LaunchPad").GetComponent<LaunchPad>().Reset();


        // Aqui en se estableceran las vidas etc
    }

    public void OnPlayerLeft(PlayerInput leftPlayer)
    {
        Debug.Log($"Jugador {leftPlayer.playerIndex + 1} se ha desconectado.");
    }
}