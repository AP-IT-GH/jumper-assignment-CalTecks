using UnityEngine;

public class Moving: MonoBehaviour
{
    public float speed = 5f; // Stel hier de gewenste snelheid in
    private AgentObject agent;
    private Rigidbody rb;

    void Start()
    {
        agent = FindObjectOfType<AgentObject>();
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed * -1f; // Hier wordt de snelheid ingesteld in de voorwaartse richting van het object
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BackWall"))
        {
            agent.WallSuccess();
            Destroy(gameObject);
        }
    }
}
