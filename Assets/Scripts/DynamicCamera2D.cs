using UnityEngine;

public class DynamicCamera2D : MonoBehaviour
{
    [Header("Límites del Mapa")]
    [Tooltip("Asigna aquí un BoxCollider2D vacío que delimite tu nivel. Marca 'Is Trigger' en el collider.")]
    public BoxCollider2D limitesMapa;

    [Header("Límites de Zoom (Orthographic Size)")]
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float zoomLimiter = 15f;
    public float padding = 2f;

    [Header("Suavizado (Smooth)")]
    public float smoothTime = 0.25f;

    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private float zoomVelocity;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (!cam.orthographic)
        {
            Debug.LogWarning("¡Ojo! La cámara debe estar en modo Orthographic.");
        }
    }

    void LateUpdate()
    {
        if (Data_Static.playerList == null || Data_Static.playerList.Count == 0)
            return;

        // Es importante hacer el zoom PRIMERO, porque el tamaño de la cámara 
        // afecta a los límites de posición que calculamos después.
        HacerZoom();
        MoverCamara();
    }

    void MoverCamara()
    {
        Vector3 puntoCentral = ObtenerPuntoCentral();
        Vector3 nuevaPosicion = new Vector3(puntoCentral.x, puntoCentral.y, transform.position.z);

        // --- NUEVA LÓGICA: Limitar la posición con el BoxCollider2D ---
        if (limitesMapa != null)
        {
            // Calculamos el tamaño real de la cámara en unidades de Unity
            float camMitadAlto = cam.orthographicSize;
            float camMitadAncho = cam.orthographicSize * cam.aspect;

            // Calculamos hasta dónde puede llegar el centro de la cámara sin salir del collider
            float minX = limitesMapa.bounds.min.x + camMitadAncho;
            float maxX = limitesMapa.bounds.max.x - camMitadAncho;
            float minY = limitesMapa.bounds.min.y + camMitadAlto;
            float maxY = limitesMapa.bounds.max.y - camMitadAlto;

            // Seguridad: Si la cámara es más grande que el propio límite, la centramos
            if (minX > maxX) nuevaPosicion.x = limitesMapa.bounds.center.x;
            else nuevaPosicion.x = Mathf.Clamp(nuevaPosicion.x, minX, maxX);

            if (minY > maxY) nuevaPosicion.y = limitesMapa.bounds.center.y;
            else nuevaPosicion.y = Mathf.Clamp(nuevaPosicion.y, minY, maxY);
        }

        // Movemos la cámara
        transform.position = Vector3.SmoothDamp(transform.position, nuevaPosicion, ref velocity, smoothTime);
    }

    void HacerZoom()
    {
        float mayorDistancia = ObtenerMayorDistancia();

        float zoomDeseado = Mathf.Lerp(minZoom, maxZoom, mayorDistancia / zoomLimiter);
        zoomDeseado += padding;

        zoomDeseado = Mathf.Clamp(zoomDeseado, minZoom, maxZoom);

        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoomDeseado, ref zoomVelocity, smoothTime);
    }

    Vector3 ObtenerPuntoCentral()
    {
        if (Data_Static.playerList.Count == 1)
            return Data_Static.playerList[0].transform.position;

        var bounds = new Bounds(Data_Static.playerList[0].transform.position, Vector3.zero);
        for (int i = 0; i < Data_Static.playerList.Count; i++)
        {
            if (Data_Static.playerList[i] != null)
            {
                bounds.Encapsulate(Data_Static.playerList[i].transform.position);
            }
        }
        return bounds.center;
    }

    float ObtenerMayorDistancia()
    {
        var bounds = new Bounds(Data_Static.playerList[0].transform.position, Vector3.zero);
        for (int i = 0; i < Data_Static.playerList.Count; i++)
        {
            if (Data_Static.playerList[i] != null)
            {
                bounds.Encapsulate(Data_Static.playerList[i].transform.position);
            }
        }
        return Mathf.Max(bounds.size.x, bounds.size.y);
    }
}