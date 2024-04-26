using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // Het object dat moet worden gespawned
    public float minSpawnInterval = 1f; // Minimale tijd tussen spawns
    public float maxSpawnInterval = 2f; // Maximale tijd tussen spawns
    private float moveSpeed;

    private float nextSpawnTime; // Tijd wanneer het volgende object wordt gespawned

    void Start()
    {
        // Initialiseer de volgende spawn tijd
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        // Controleer of het tijd is om een object te spawnen
        if (Time.time >= nextSpawnTime)
        {
            SpawnObject();
            // Bereken de volgende spawn tijd
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    public void changeSpeed()
    {
        // Deze functie wordt elke keer opgeroepen als er een nieuwe leerepisode begint
        // Deze functie is daarom dus ook public, zodat AgentObject deze functie kan in gang zetten
        // bij het begin van elke nieuwe leerepisode van ML-Agent
        moveSpeed = Random.Range(6f, 12f);
        Debug.Log("Muren snelheid: " + moveSpeed);
    }

    void SpawnObject()
    {
        // Spawn het object op de positie van de spawner
        GameObject newObject = Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        Moving movingComponent = newObject.GetComponent<Moving>();

        // Controleer of de Moving-component is gevonden
        if (movingComponent != null)
        {
            // Stel de snelheid in
            movingComponent.speed = moveSpeed;
        }
    }
}
