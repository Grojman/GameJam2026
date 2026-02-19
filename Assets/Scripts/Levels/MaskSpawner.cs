using UnityEngine;

public class MaskSpawner : MonoBehaviour
{
    [Header("Lista de Máscaras (Prefabs)")]
    [Tooltip("Arrastra aquí todos los prefabs de las máscaras/armas que pueden aparecer.")]
    public GameObject[] mascarasDisponibles;

    [Header("Configuración de Tiempo (Segundos)")]
    public float tiempoMinimo = 3f;
    public float tiempoMaximo = 10f;

    void Start()
    {
        float tiempoEspera = Random.Range(tiempoMinimo, tiempoMaximo);

        Invoke("SpawnearArmaAleatoria", tiempoEspera);
    }

    public void SpawnearArmaAleatoria()
    {
        if (mascarasDisponibles == null || mascarasDisponibles.Length == 0)
        {
            Debug.LogWarning("¡Ojo! El Spawner " + gameObject.name + " no tiene armas asignadas.");
            return;
        }

        int indiceAleatorio = Random.Range(0, mascarasDisponibles.Length);

        GameObject armaElegida = mascarasDisponibles[indiceAleatorio];

        Instantiate(armaElegida, transform.position, transform.rotation);
    }
}