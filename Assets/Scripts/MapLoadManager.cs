using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class MapLoadManager : MonoBehaviour
{
    [SerializeField] List<Transform> positions;
    [SerializeField] List<GameObject> players;
    [SerializeField] GameObject prefab;

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
        Data_Static.alivePLayers = Data_Static.playerList.Count;
        cam = GetComponent<Camera>();
        defaultSize = cam.orthographicSize; // Guardamos el tamaño original
        foreach (PlayerInput pl in Data_Static.playerList)
        {
            Transform pos = positions[0];
            positions.Remove(pos);
            pl.gameObject.transform.position = pos.position;
            pl.GetComponent<SpriteRenderer>().enabled = true;
            pl.GetComponent<Player>().DisableFamilyFriendly();
        }
    }

    private void Update()
    {
        PlayerInput alivePlayer = null;
        if(Data_Static.alivePLayers == 1)
        {
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
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * zoomSpeed);

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

        cam.orthographicSize = defaultSize;
        isZooming = false;
    }
}
