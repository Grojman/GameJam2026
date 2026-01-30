using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapLoadManager : MonoBehaviour
{
    [SerializeField] List<Transform> positions;
    [SerializeField] List<GameObject> players;
    [SerializeField] GameObject prefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(GameObject pl in players)
        {
            Transform pos = positions[0];
            positions.Remove(pos);
            Instantiate(prefab, pos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
