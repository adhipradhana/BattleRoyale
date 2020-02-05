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


    public void UseHealthPack()
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
            }
        }
    }

    public bool CheckDeath()
    {
        if (health < MinHealth)
        {
            StageAcademy.playerCount--;
            return true;
        }

        return false;
    }

    public void AgentInjured()
    {
        Health -= BulletDamage;
        Debug.Log("Player health " + Health);
    }

    public void GetHealthPack()
    {
        HealthPack++;
    }


}
