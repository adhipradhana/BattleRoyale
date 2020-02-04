using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Player : Agent
{
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    //private PlayerObservation playerObservation;
    private PlayerShooting playerShooting;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        //playerObservation = GetComponent<PlayerObservation>();
        playerShooting = GetComponent<PlayerShooting>();
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
        AddVectorObs((float) playerHealth.Health / (float) PlayerHealth.MaxHealth);

        // Add Health Pack Number
        AddVectorObs(playerHealth.HealthPack);

        // Add Bullet number
        AddVectorObs(playerShooting.BulletCount);
    }

}
