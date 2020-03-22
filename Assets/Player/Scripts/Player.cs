using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Player : Agent
{
    private const float BooleanTrigger = 0f;
    private const string BulletPackTag = "Bullet Pack";
    private const string HealthPackTag = "Health Pack";

    private const float ItemFoundReward = 0.02f;
    private const float BulletHitReward = 0.025f;
    private const float KillReward = 0.1f;
    private const float WinReward = 1f;

    private const float DeathPunishment = -0.5f;
    private const float BulletMissPunishment = 0.01f;

    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;
    public Rigidbody2D rb;

    private Camera cam;
    private Vector2 mousePos;

    void Awake()
    {
        cam = FindObjectOfType<Camera>();
    }

    public override void AgentAction(float[] vectorAction)
    {
        if (gameObject.activeSelf)
        {
            // Agent movement
            float moveHorizontal = Mathf.Clamp(vectorAction[0], -1, 1);
            float moveVertical = Mathf.Clamp(vectorAction[1], -1, 1);
            Vector2 movement = new Vector2(moveHorizontal, moveVertical);

            // Agent rotation
            vectorAction[2] = vectorAction[2] * Mathf.PI;
            float moveRotation = Mathf.Clamp(vectorAction[2], -Mathf.PI, Mathf.PI);

            playerMovement.Move(movement, moveRotation);

            // Agent shooting state
            vectorAction[3] = Mathf.Clamp(vectorAction[3], -1, 1);
            bool isShooting = vectorAction[3] >= BooleanTrigger;
            if (isShooting)
            {
                playerShooting.Shoot();
            }

            // Agent health pack state
            vectorAction[4] = Mathf.Clamp(vectorAction[4], -1, 1);
            bool isUsingHealthPack = vectorAction[4] >= BooleanTrigger;
            if (isUsingHealthPack)
            {
                int health = playerHealth.UseHealthPack();
                AddReward(health / 1000f);
            }

            // Check if agent win
            if (AcademyValue.playerCount <= 1)
            {
                AddReward(WinReward);
                Done();
            }

            // Agent death condition
            if (playerHealth.CheckDeath())
            {
                AcademyValue.playerCount--;
                AddReward(DeathPunishment);
                gameObject.SetActive(false);
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
        AddVectorObs(gameObject.activeSelf);
    }

    public override void AgentOnDone()
    {
        Destroy(gameObject);
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
        AddReward(damage / 1000f);
    }

    public void AgentKillReward()
    { 
        AddReward(KillReward);
    }

    public bool CheckDeath()
    {
        return playerHealth.CheckDeath();
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
