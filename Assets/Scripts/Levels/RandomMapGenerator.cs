using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RandomMapGenerator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roundsVisual;
    [SerializeField] List<GameObject> Maps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        System.Random rnd = new System.Random();
        int nRand = rnd.Next(0, Maps.Count - 1);
        Instantiate(Maps[nRand]);


        if(Data_Static.ROUND_LIMIT < Data_Static.roundCounter)
        {
            foreach(PlayerInput p in Data_Static.playerList)
            {
                Destroy(p.gameObject);
            }

            Data_Static.alivePLayers = 0;
            Data_Static.playerList = new ();
            Data_Static.roundCounter = 1;
            SceneManager.LoadScene("Login");
        }
        roundsVisual.text = $"Ronda {Data_Static.roundCounter}";
        Data_Static.roundCounter++;

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
