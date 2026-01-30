using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapLoadManager : MonoBehaviour
{
    [SerializeField] List<Transform> positions;
    public GameObject prefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(Transform go in positions)
        {
            Instantiate(prefab, go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
