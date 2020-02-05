using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Player : Agent
{
    private const float BooleanTrigger = 0.5f;
    private const string BulletPackTag = "Bullet Pack";
    private const string BulletTag = "Bullet";
    private const string HealthPackTag = "Health Pack";

    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;

    private Rigidbody2D rb;
    private Camera cam;
    private Vector2 mousePos;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        cam = FindObjectOfType<Camera>();
        rb = GetComponent<Rigidbody2D>();
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
            // TODO : Add reward for shooting other agent
        }

        // Agent health pack state
        bool isUsingHealthPack = vectorAction[4] >= BooleanTrigger;
        if (isUsingHealthPack)
        {
            playerHealth.UseHealthPack();
            // TODO : Add reward for using health pack
        }

        // Check if agent win
        if (StageAcademy.playerCount <= 1 && gameObject.activeSelf)
        {
            Done();
            // TODO : Add reward for winning the game
        }

        // Agent death condition
        if (playerHealth.CheckDeath())
        {
            gameObject.SetActive(false);
            // TODO : Add punishmend for death
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(BulletTag))
        {
            playerHealth.AgentInjured();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag(HealthPackTag))
        {
            playerHealth.GetHealthPack();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag(BulletPackTag))
        {
            playerShooting.GetBulletPack();
            Destroy(collision.gameObject);
        }
    }

}
