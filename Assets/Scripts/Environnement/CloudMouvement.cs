using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public float speed = 1f; // Vitesse de déplacement

    private Vector3 direction;
    private float minX, maxX, minZ, maxZ; // Limites de la map
    private CloudSpawner spawner; // Référence au spawner pour récupérer la taille de la map

    void Start()
    {
        // Trouver le CloudSpawner pour obtenir les dimensions de la map
        spawner = FindObjectOfType<CloudSpawner>();

        if (spawner != null)
        {
            minX = spawner.GetMinX();
            maxX = spawner.GetMaxX();
            minZ = spawner.GetMinZ();
            maxZ = spawner.GetMaxZ();
        }

        // Chaque nuage a une direction légèrement différente
        direction = new Vector3(Random.Range(1f, 1f), 0, Random.Range(0.3f, 0.3f)).normalized;
    }

    void Update()
    {
        // Déplacement en ligne droite
        transform.position += direction * speed * Time.deltaTime;

        // Recycler le nuage quand il sort de la zone
        if (transform.position.x > maxX + 8f) // Si le nuage sort du bord droit
        {
            RecycleCloud(minX - 8f);
        }
        else if (transform.position.x < minX - 8f) // Si le nuage sort du bord gauche
        {
            RecycleCloud(maxX + 8f);
        }
    }

    void RecycleCloud(float newX)
    {
        transform.position = new Vector3(newX, Random.Range(spawner.minHeight, spawner.maxHeight), Random.Range(minZ, maxZ));
    }
}
