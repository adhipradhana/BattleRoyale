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


    public int UseHealthPack()
    {
        int addition = 0;

        if ((Health < MaxHealth) && (healthPack > MinHealthPack))
        {
            if ((MaxHealth - Health) < HealthAddition)
            {
                addition = MaxHealth - Health;
                Health = MaxHealth;
            }
            else
            {
                addition = HealthAddition;
                Health += HealthAddition; 
            }

            healthPack--;
        }

        return addition;
    }

    public bool CheckDeath()
    {
        return (health <= MinHealth);
    }

    public int AgentInjured()
    {
        int damage;

        if (Health < BulletDamage)
        {
            damage = Health * -1;
            Health = 0;
        }
        else
        {
            damage = BulletDamage * -1;
            Health -= BulletDamage;
        }

        return damage;
    }

    public void GetHealthPack()
    {
        HealthPack++;
    }


}
