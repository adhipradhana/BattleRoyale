using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public const int MaxHealth = 100;
    private const int MinHealth = 0;
    private const int HealthAddition = 40;
    private const int BulletDamage = 30;
    private const int MinHealthPack = 0;
    private const string BulletTag = "Bullet";
    private const string HealthPackTag = "Health Pack";

    private int health = 100;
    private int healthPack = 0;

    public int Health
    {
        get { return health;  }
        set { health = value; }
    }
    public int HealthPack
    {
        get { return healthPack; }
        set { healthPack = value; }
    }

    void Update()
    {
        CheckDeath();    
    }

    void UseHealthPack()
    {
        if (Input.GetKeyDown(KeyCode.H))
        { 
            if ((health < MaxHealth) && (healthPack > MinHealthPack))
            {
                health += HealthAddition;
                if (health > MaxHealth)
                {
                    health = MaxHealth;         
                }

                healthPack--;

                // TODO : Add reward for using health
            }
        }
    }

    void CheckDeath()
    {
        if (health < MinHealth)
        {
            gameObject.SetActive(false);
            StageAcademy.playerCount--;
            // TODO : Give punishment if player died
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(BulletTag))
        {
            health -= BulletDamage;
            Debug.Log("Player health " + Health);

            Destroy(collision.gameObject);
        } else if (collision.gameObject.CompareTag(HealthPackTag))
        {
            HealthPack++;
            Destroy(collision.gameObject);
        }
    }
}
