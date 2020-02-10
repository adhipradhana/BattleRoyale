using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Player : Agent
{
    private const float BooleanTrigger = 0.5f;
    private const string BulletPackTag = "Bullet Pack";
    private const string HealthPackTag = "Health Pack";

    private const float ItemFoundReward = 0.02f;
    private const float BulletHitReward = 0.025f;
    private const float KillReward = 0.1f;
    private const float WinReward = 1f;

    private const float DeathPunishment = -0.5f;

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
        // Agent movement
        Vector2 movement = new Vector2(vectorAction[0], vectorAction[1]);

        playerMovement.Move(movement, vectorAction[2]);

        // Agent shooting state
        bool isShooting = vectorAction[3] >= BooleanTrigger;
        if (isShooting)
        {
            playerShooting.Shoot();
        }

        // Agent health pack state
        bool isUsingHealthPack = vectorAction[4] >= BooleanTrigger;
        if (isUsingHealthPack)
        {
            int health = playerHealth.UseHealthPack();
            AddReward(health / 1000f);
        }

        // Check if agent win
        if (StageAcademy.playerCount <= 1 && gameObject.activeSelf)
        {
            AddReward(WinReward);
            Done();
        }

        // Agent death condition
        if (playerHealth.CheckDeath())
        {
            StageAcademy.playerCount--;
            gameObject.SetActive(false);
            AddReward(DeathPunishment);
        }

        //Debug.Log(rewa)
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
        action[2] = Mathf.Atan2(lookDir.y, lookDir.x);

        // Shooting state
        if (Input.GetButtonDown("Fire1"))
        {
            action[3] = 1f;
        } else
        {
            action[3] = 0f;
        }

        // Using Health Pack state
        if (Input.GetKeyDown(KeyCode.H))
        {
            action[4] = 1f;
        }
        else
        {
            action[4] = 0f;
        }

        // Agent health condition

        return action;
    }

    public override void CollectObservations()
    {
        // Add positional vector
        AddVectorObs((transform.position.x - StageAcademy.minimumX) / (StageAcademy.maximumX - StageAcademy.minimumX));
        AddVectorObs((transform.position.y - StageAcademy.minimumY) / (StageAcademy.maximumY - StageAcademy.minimumY));

        // Add rotational vector
        AddVectorObs((rb.rotation + 90f) / 180f);

        // Add Health Vector
        AddVectorObs((float)playerHealth.Health / (float)PlayerHealth.MaxHealth);

        //// Add Health Pack Number
        AddVectorObs(playerHealth.HealthPack);

        //// Add Bullet number
        AddVectorObs(playerShooting.BulletCount);
    }

    public override void AgentOnDone()
    {
        Destroy(gameObject);
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
