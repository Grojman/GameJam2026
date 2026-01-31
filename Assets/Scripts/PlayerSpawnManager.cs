using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;
using TMPro;

public class PlayerSpawnManager : MonoBehaviour
{
    int playerCount = 0;
    readonly string[] PLAYER_NAMES = new string[] {"Evasilio", "Filadelfo", "Segismunda", "Segismundo", "Onesíforo", "Telesforo", "Unga", "Godofredo", "Casimiro"};
    public AudioSource lobbyMusic;
    [SerializeField] TextMeshProUGUI pressAToenter; 

    [Header("Posiciones de Inicio")]
    public Transform[] spawnPoints;

    [Header("Caras por Jugador (Opcional visual)")]
    [SerializeField] List<Sprite> sprites;
    private System.Random rng = new System.Random();


    public void Start()
    {
        // lobbyMusic.Play();
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

    public Sprite SwapSprite(Sprite sprite)
    {
        Sprite result;
        if(sprites.Count > 0)
        {
            result = sprites[0];
            sprites.RemoveAt(0);
            sprites.Add(sprite);
        } else
        {
            result = sprite;
        }
        return result;
    }

    // Esta función se llamará automáticamente cuando el PlayerInputManager detecte un mando nuevo
    public void OnPlayerJoined(PlayerInput newPlayer)
    {
        playerCount++;
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
        player.name = PLAYER_NAMES[new System.Random().Next(PLAYER_NAMES.Count())];
        player.SetNameVisual();
        player.EnableFamilyFriendly();
        player.SpawnPoint = spawnPoints[index];
        player.psManager = this;
        if (player != null && sprites.Count > 0)
        {
            player.face.sprite = sprites[0];
            sprites.RemoveAt(0);
        }

        GameObject pad = GameObject.FindGameObjectWithTag("LaunchPad");

        pad.GetComponent<LaunchPad>().MaxPlayers++;
        pad.GetComponent<LaunchPad>().UpdateLabel();

        pressAToenter.enabled = playerCount < 3;

        GameObject.FindGameObjectWithTag("LaunchPad").GetComponent<LaunchPad>().Reset();


        // Aqui en se estableceran las vidas etc
    }

    public void OnPlayerLeft(PlayerInput leftPlayer)
    {
        playerCount--;
        pressAToenter.enabled = playerCount < 3;
        Debug.Log($"Jugador {leftPlayer.playerIndex + 1} se ha desconectado.");
    }
}