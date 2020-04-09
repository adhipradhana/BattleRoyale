using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public const int MaxHealth = 100;
    private const int MinHealth = 0;
    private const int BulletDamage = 25;

    private int health = 100;

    public int Health
    {
        get { return health;  }
        set { health = value; }
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

}
