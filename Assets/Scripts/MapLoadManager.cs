using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class MapLoadManager : MonoBehaviour
{
    [SerializeField] List<Transform> positions;
    [SerializeField] List<GameObject> players;
    [SerializeField] GameObject prefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(PlayerInput pl in Data_Static.playerList)
        {
            Transform pos = positions[0];
            positions.Remove(pos);
            pl.gameObject.transform.position = pos.position;
            pl.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
