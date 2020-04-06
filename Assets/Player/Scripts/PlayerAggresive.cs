using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PlayerAggresive : Player
{
    private Vector2 previousPosition;
    private const float MoveReward = 0.001f;

    void Awake()
    {
        cam = FindObjectOfType<Camera>();
        previousPosition = new Vector2(transform.position.x, transform.position.y);
    }



    public override void AgentAction(float[] vectorAction)
    {
        if (isActive && !AcademyValue.gameDone)
        {
            // Agent movement
            float moveHorizontal = Mathf.Clamp(vectorAction[0], -1, 1);
            float moveVertical = Mathf.Clamp(vectorAction[1], -1, 1);
            Vector2 movement = new Vector2(moveHorizontal, moveVertical);

            // Agent rotation
            bool isFacingUp = vectorAction[3] >= BooleanTrigger;
            float moveRotation;
            if (isFacingUp)
            {
                moveRotation = ScaleAction(vectorAction[2], Mathf.PI, 0);
            }
            else
            {
                moveRotation = ScaleAction(vectorAction[2], -Mathf.PI, 0);
            }


            playerMovement.Move(movement, moveRotation);

            // check if position is greater than 1
            if (Vector2.Distance(transform.position, previousPosition) >= 1)
            {
                AddReward(MoveReward);
                previousPosition.x = transform.position.x;
                previousPosition.y = transform.position.y;
            }

            // Agent shooting state
            vectorAction[4] = Mathf.Clamp(vectorAction[4], -1, 1);
            bool isShooting = vectorAction[4] >= BooleanTrigger;
            if (isShooting)
            {
                if (stepShooting % stepReset == 0)
                {
                    playerShooting.Shoot();
                }

                stepShooting++;
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
        float direction = Mathf.Atan2(lookDir.y, lookDir.x) / Mathf.PI;

        if (direction >= BooleanTrigger)
        {
            action[2] = (direction * -2) + 1;
            action[3] = 1;
        }
        else
        {
            action[2] = (direction * 2) + 1;
            action[3] = -1;
        }

        // Shooting state
        if (Input.GetButtonDown("Fire1"))
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

        //// Add Bullet number
        AddVectorObs(playerShooting.BulletCount);

        // Add death state
        AddVectorObs(isActive);
    }

}
