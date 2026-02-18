using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Configuración del Parallax")]
    [Tooltip("Arrastra aquí tu Main Camera. Si lo dejas vacío, la buscará automáticamente.")]
    public GameObject cam;

    [Tooltip("0 = Se mueve igual que la cámara (fondo infinito muy lejano). 1 = Se queda estático (objeto muy cercano).")]
    public float parallaxEffect;

    private float length;
    private float startpos;

    void Start()
    {
        // Si no asignas la cámara en el inspector, la busca por ti
        if (cam == null)
        {
            cam = Camera.main.gameObject;
        }

        // Guardamos la posición inicial y el ancho del Sprite
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void LateUpdate()
    {
        // Calcula cuánto se ha movido la cámara en el mundo real
        float temp = (cam.transform.position.x * (1 - parallaxEffect));

        // Calcula cuánto se debe mover ESTE fondo en base a su efecto Parallax
        float dist = (cam.transform.position.x * parallaxEffect);

        // Movemos el fondo
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        // --- EFECTO INFINITO ---
        // Si la cámara sobrepasa la imagen por la derecha, reposicionamos el inicio
        if (temp > startpos + length)
        {
            startpos += length;
        }
        // Si la cámara sobrepasa la imagen por la izquierda, reposicionamos el inicio
        else if (temp < startpos - length)
        {
            startpos -= length;
        }
    }
}