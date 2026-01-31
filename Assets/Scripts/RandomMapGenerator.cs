using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RandomMapGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> Maps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        System.Random rnd = new System.Random();
        int nRand = rnd.Next(0, Maps.Count - 1);
        Instantiate(Maps[nRand]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
