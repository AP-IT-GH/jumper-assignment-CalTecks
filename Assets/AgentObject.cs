using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentObject : Agent
{
    Rigidbody rBody;
    private Vector3 startPos;
    private Quaternion startRot;
    public float jumpForce = 3f;
    public float nojumpReward = 0.01f;
    private bool canJump = true;
    private float lastJumpTime;
    public float timeBetweenJumps = 0.8f;
    public ObjectSpawner wallSpawner;
    public ObjectSpawner rewardSpawner;
    private float totalReward = 0f;
    public float noJumpRewardInterval = 0.25f; // Interval in seconden voor het toekennen van de no jump beloning
    private float timeSinceLastJump = 0f; // Timer voor bijhouden van tijd sinds de laatste sprong


    void Start()
    {
        // rigidbody van de agent zelf
        rBody = GetComponent<Rigidbody>();
        // beginpositie bewaren, zodat we er na einde episode terug naar kunnen resetten
        startPos = transform.position;
        startRot = transform.rotation;
        lastJumpTime = -1000f; // Initialiseren met een negatieve waarde om ervoor te zorgen dat er meteen kan worden gesprongen
    }

    public override void OnEpisodeBegin()
    {
        // reward tellen vuur te loggen terug op nul zetten
        totalReward = 0f;
        // al de gespawnde muren vernietigen
        // zoek alle objecten met de tag "CollidingWall"
        GameObject[] walls = GameObject.FindGameObjectsWithTag("CollidingWall");

        // loop door alle gevonden objecten en vernietig ze
        foreach (GameObject wall in walls)
        {
            Destroy(wall);
        }
        // al de gespawnde reward spheres vernietigen
        // zoek alle objecten met de tag "CollisionReward"
        GameObject[] spheres = GameObject.FindGameObjectsWithTag("CollisionReward");

        // loop door alle gevonden objecten en vernietig ze
        foreach (GameObject sphere in spheres)
        {
            Destroy(sphere);
        }


        // nieuwe snelheid van de muren door de spawner laten bepalen
        wallSpawner.changeSpeed();
        rewardSpawner.changeSpeed();

        // positie, rotatie en eventuele bewegingskrachten resetten van de agent
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        transform.position = startPos;
        transform.rotation = startRot;
        canJump = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // we observeren de lokale positie en de velocity op de rigidbody (y as)
        sensor.AddObservation(this.transform.localPosition.y);
        sensor.AddObservation(rBody.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // controleren of de 'AI' wil springen
        bool jumpAction = actionBuffers.DiscreteActions[0] == 1;
        //Debug.Log(Time.time - lastJumpTime);
        // Controleren of de agent mag springen en vervolgens springen
        if (jumpAction && canJump)
        {
            Jump();
            canJump = false;
            lastJumpTime = Time.time;
        }
        if (!canJump && (Time.time - lastJumpTime > timeBetweenJumps))
        {
            canJump = true;
        }
        timeSinceLastJump += Time.deltaTime;

        // Belonen om NIET te springen als de agent kan springen
        if (canJump && timeSinceLastJump >= noJumpRewardInterval)
        {
            AddReward(nojumpReward);
            totalReward += nojumpReward;
            Debug.Log("REWARD TOTAAL = " + totalReward);
            timeSinceLastJump = 0f; // Reset de timer
        }



    }

    public void BonusPoints(float points)
    {
        AddReward(points);
        totalReward += points;
        Debug.Log("REWARD TOTAAL = " + totalReward);
    }

    public void Jump()
    {
        Debug.Log("Jumped!");
        rBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        AddReward(-0.2f); // afstraffing voor springen
        totalReward -= 0.2f;
        Debug.Log("REWARD TOTAAL = " + totalReward);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Controleer of het object waarmee wordt gebotst de juiste tag heeft
        if (collision.gameObject.CompareTag("CollidingWall"))
        {
            Debug.Log("Oeps! Botsing met een muur.");
            AddReward(-10f);
            totalReward -= 10f;
            Debug.Log("REWARD TOTAAL = " + totalReward);
            EndEpisode();
        }
    }

    public void WallSuccess()
    {
        Debug.Log("Een muur heeft succesvol de eindmuur bereikt.");
        AddReward(2f);
        totalReward += 2f;
        Debug.Log("REWARD TOTAAL = " + totalReward);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        // Controleren of de spatiebalk wordt ingedrukt en of de agent kan springen
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}