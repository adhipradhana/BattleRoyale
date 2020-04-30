using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensor;
using TMPro;

public class Player : Agent
{
    public enum PlayerType {
        Normal,
        Aggresive,
        Passive
    }

    // Agent type
    public PlayerType playerType;

    public int agentID;

    public SpriteRenderer body;
    public SpriteRenderer cone;
    public CircleCollider2D circle;
    public TextMeshPro number;

    protected const float BooleanTrigger = 0f;
    private const string BulletPackTag = "Bullet Pack";

    protected const float ItemFoundReward = 0.025f;
    private const float BulletHitReward = 0.25f;
    private const float KillReward = 0.75f;
    protected const float WinReward = 2.5f;

    protected const float DeathPunishment = -1f;
    private const float BulletMissPunishment = -0.01f;

    // Movement reward for aggresive agent and passive agent
    private Vector2 previousPosition;
    private const float MoveReward = 0.0025f;
    private const float MovePunishment = -0.0025f;

    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;
    public Rigidbody2D rb;

    protected Camera cam;
    protected Vector2 mousePos;

    protected bool isActive = true;
    protected int stepShooting = 0;
    protected int stepHealth = 0;
    protected const int stepReset = 10;

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

            // add movement reward if agent type different
            if (playerType != PlayerType.Normal)
            {
                if (Vector2.Distance(transform.position, previousPosition) >= 1)
                {
                    previousPosition.x = transform.position.x;
                    previousPosition.y = transform.position.y;

                    if (playerType == PlayerType.Aggresive)
                    {
                        AddReward(MoveReward);
                    }
                    else if (playerType == PlayerType.Passive)
                    {
                        AddReward(MovePunishment);
                    }
                }
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
                PlayerInformation.AddDeathStatistics(agentID);
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
        } else
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

    public void AgentMissPunishment()
    {
        AddReward(BulletMissPunishment);
    }

    public void AgentHitReward()
    {
        AddReward(BulletHitReward);
    }

    public void AgentHitPunishment()
    {
        int damage = playerHealth.AgentInjured();
        AddReward(damage / 100f);
    }

    public void AgentKillReward()
    { 
        AddReward(KillReward);
    }

    public bool CheckDeath()
    {
        return playerHealth.CheckDeath();
    }

    public void DeactivateEverything()
    {
        body.enabled = false;
        cone.enabled = false;
        circle.enabled = false;
        number.enabled = false;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(BulletPackTag))
        {
            playerShooting.GetBulletPack();
            AddReward(ItemFoundReward);
            Destroy(collision.gameObject);
        }
    }

}
