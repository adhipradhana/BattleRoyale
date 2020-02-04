using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensor;

public class Player : Agent
{
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
    }

    public override void AgentAction(float[] vectorAction)
    {
        Vector2 movement = new Vector2(vectorAction[0], vectorAction[1]);

        playerMovement.Move(movement);
    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

    public override void CollectObservations()
    {
        // Add positional vector
        AddVectorObs((transform.position.x - StageAcademy.minimumX) / (StageAcademy.maximumX - StageAcademy.minimumX));
        AddVectorObs((transform.position.y - StageAcademy.minimumY) / (StageAcademy.maximumY - StageAcademy.minimumY));

        // Add rotational vector
        float rotation = transform.rotation.eulerAngles.z / 360.0f;
        AddVectorObs(rotation);

        // Add Health Vector
        AddVectorObs((float)playerHealth.Health / (float)PlayerHealth.MaxHealth);

        //// Add Health Pack Number
        AddVectorObs(playerHealth.HealthPack);

        //// Add Bullet number
        AddVectorObs(playerShooting.BulletCount);
    }

}
