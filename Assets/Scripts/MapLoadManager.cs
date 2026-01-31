using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MapLoadManager : MonoBehaviour
{
    [SerializeField] List<Transform> positions;
    [SerializeField] List<GameObject> players;
    [SerializeField] GameObject prefab;
    private FadeInOut fade;

    [Header("Zoom Camera")]
    public float zoomLevel = 3f;      // Cuanto más bajo, más cerca (ej: 3 es muy cerca, 5 es normal)
    public float zoomSpeed = 5f;      // Qué tan rápido entra el zoom
    public float duration = 2f;       // Cuánto tiempo se queda el zoom (2 segundos)

    private Camera cam;
    private float defaultSize;
    private Vector3 defaultPosition;
    private bool isZooming = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fade = FindAnyObjectByType<FadeInOut>();
        fade.SetIn(() => { });
        Data_Static.alivePLayers = Data_Static.playerList.Count;
        cam = Camera.main;
        defaultSize = cam.orthographicSize; // Guardamos el tamaño original
        foreach (PlayerInput pl in Data_Static.playerList)
        {
            Transform pos = positions[0];
            positions.Remove(pos);
            pl.gameObject.transform.position = pos.position;
            if (pl.GetComponent<Player>().Alive)
            {
                pl.GetComponent<SpriteRenderer>().enabled = true;
                pl.GetComponent<Player>().DisableFamilyFriendly();
            }
            else{
                pl.GetComponent<Player>().Revive();
            }

            
            
            
        }
    }

    private void Update()
    {
        
        if(Data_Static.alivePLayers <= 1)
        {
            PlayerInput alivePlayer = null;
            foreach (PlayerInput pl in Data_Static.playerList)
            {
                if(pl.GetComponent<Player>().Alive) alivePlayer = pl;
            }
            TriggerZoom(alivePlayer.transform);
        }
    }

    public void TriggerZoom(Transform targetPlayer)
    {
        // Si ya está haciendo zoom, no lo interrumpimos (o puedes pararlo con StopAllCoroutines)
        if (isZooming || targetPlayer == null) return;

        StartCoroutine(DoZoomEffect(targetPlayer));
    }

    private IEnumerator DoZoomEffect(Transform target)
    {
        isZooming = true;

        // Guardamos la posición central original antes de movernos
        defaultPosition = transform.position;

        float timer = 0f;

        while (timer < duration)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomLevel, Time.deltaTime * zoomSpeed);

            Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10f);
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, Time.deltaTime * zoomSpeed);

            timer += Time.deltaTime;
            yield return null;
        }

        /*//Volver a la normalidad
        while (Mathf.Abs(cam.orthographicSize - defaultSize) > 0.1f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultSize, Time.deltaTime * zoomSpeed);
            transform.position = Vector3.Lerp(transform.position, defaultPosition, Time.deltaTime * zoomSpeed);

            yield return null;
        }*/

        
        isZooming = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
