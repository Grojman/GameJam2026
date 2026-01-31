using UnityEngine;

public class parallax : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 2f; // Velocidad de movimiento (MÁS ALTA = MÁS CERCA)

    private float width;

    void Start()
    {
        // Calculamos el ancho exacto de la imagen automáticamente
        // (Asegúrate de que la imagen tiene un SpriteRenderer)
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // 1. Movemos la imagen a la izquierda constantemente
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 2. Si la imagen se ha salido totalmente de la pantalla (hacia la izquierda)
        // La teletransportamos a la derecha para que vuelva a entrar
        if (transform.position.x <= -width)
        {
            // La movemos 2 veces su ancho a la derecha para ponerla justo detrás de su "gemela"
            // (Asumiendo que el punto de pivote está en el centro)
            Vector2 resetPosition = new Vector2(width * 2f, 0);
            transform.position = (Vector2)transform.position + resetPosition;
        }
    }
}