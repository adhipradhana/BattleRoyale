using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private const string WallTag = "Wall";
    private const string AgentTag = "Agent";

    public Player sourcePlayer;

    private int bulletDamage;
    public int BulletDamage
    {
        get { return bulletDamage; }
        set { bulletDamage = value; }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(WallTag))
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag(AgentTag))
        {
            sourcePlayer.AgentHitReward();

            Player hitPlayer = collision.GetComponent<Player>();
            hitPlayer.AgentHitPunishment();

            if (hitPlayer.CheckDeath())
            {
                sourcePlayer.AgentKillReward();
            }
        }
    }
}
