using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PlayerAggresive : Player
{
    private const float BooleanTrigger = 0f;
    private const string BulletPackTag = "Bullet Pack";
    private const string HealthPackTag = "Health Pack";

    private const float MoveReward = 0.001f;

    void Awake()
    {
        cam = FindObjectOfType<Camera>();
    }

    public override void AgentAction(float[] vectorAction)
    {
        if (isActive && !AcademyValue.gameDone)
        {
            // Agent movement
            float moveHorizontal = Mathf.Clamp(vectorAction[0], -1, 1);
            if (moveHorizontal > 0.75f || moveHorizontal < -0.75f)
            {
                AddReward(MoveReward);
            }

            float moveVertical = Mathf.Clamp(vectorAction[1], -1, 1);
            if (moveVertical > 0.75f || moveVertical < -0.75f)
            {
                AddReward(MoveReward);
            }

            Vector2 movement = new Vector2(moveHorizontal, moveVertical);

            // Agent rotation
            float moveRotation = ScaleAction(vectorAction[2], -Mathf.PI, Mathf.PI);

            playerMovement.Move(movement, moveRotation);

            // Agent shooting state
            vectorAction[3] = Mathf.Clamp(vectorAction[3], -1, 1);
            bool isShooting = vectorAction[3] >= BooleanTrigger;
            if (isShooting)
            {
                if (stepShooting % stepReset == 0)
                {
                    playerShooting.Shoot();
                }

                stepShooting++;
            }

            // Agent health pack state
            vectorAction[4] = Mathf.Clamp(vectorAction[4], -1, 1);
            bool isUsingHealthPack = vectorAction[4] >= BooleanTrigger;
            if (isUsingHealthPack)
            {
                if (stepHealth % stepReset == 0)
                {
                    int health = playerHealth.UseHealthPack();
                    AddReward(health / 100f);
                }

                stepHealth++;
            }

            // Check if agent win
            if (AcademyValue.playerCount <= 1)
            {
                AddReward(WinReward);
                AcademyValue.gameDone = true;
            }
            else if (playerHealth.CheckDeath())
            {
                AddReward(DeathPunishment);
                DeactivateEverything();
                isActive = false;
                AcademyValue.playerCount--;
            }
        }
    }

    public override float[] Heuristic()
    {
        var action = new float[5];

        // Movement
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        

        // Look direction
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - rb.position;
        action[2] = Mathf.Atan2(lookDir.y, lookDir.x) / Mathf.PI;

        // Shooting state
        if (Input.GetButtonDown("Fire1"))
        {
            action[3] = 1f;
        } else
        {
            action[3] = -1f;
        }

        // Using Health Pack state
        if (Input.GetKeyDown(KeyCode.H))
        {
            action[4] = 1f;
        }
        else
        {
            action[4] = -1f;
        }

        return action;
    }

    public override void CollectObservations()
    {
        // Add positional vector
        AddVectorObs((transform.position.x - AcademyValue.minimumX) / (AcademyValue.maximumX - AcademyValue.minimumX));
        AddVectorObs((transform.position.y - AcademyValue.minimumY) / (AcademyValue.maximumY - AcademyValue.minimumY));

        // Add rotational vector
        AddVectorObs((rb.rotation + 90f) / 180f);

        // Add Health Vector
        AddVectorObs((float)playerHealth.Health / (float)PlayerHealth.MaxHealth);

        //// Add Health Pack Number
        AddVectorObs(playerHealth.HealthPack);

        //// Add Bullet number
        AddVectorObs(playerShooting.BulletCount);

        // Add death state
        AddVectorObs(isActive);
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(HealthPackTag))
        {
            playerHealth.GetHealthPack();
            AddReward(ItemFoundReward);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag(BulletPackTag))
        {
            playerShooting.GetBulletPack();
            AddReward(ItemFoundReward);
            Destroy(collision.gameObject);
        }
    }

}
