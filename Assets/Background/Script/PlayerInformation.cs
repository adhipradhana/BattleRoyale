﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    public List<Player> playerList = new List<Player>();
    public Text infoText;

    public void InitShootingInfo(int numberPlayer)
    {
        AcademyValue.numberOfHits = new int[numberPlayer];
        AcademyValue.numberOfShoots = new int[numberPlayer];
        AcademyValue.numberOfDeath = new int[numberPlayer];
        AcademyValue.numberOfKills = new int[numberPlayer];
    }

    public void ClearList()
    {
        playerList.Clear();
        infoText.text = "";
    }

    public void AddPlayer(Player player)
    {
        playerList.Add(player);
    }

    public static void AddShootingStatistic(int playerID)
    {
        AcademyValue.numberOfShoots[playerID - 1]++;
    }

    public static void AddShootingHitStatistic(int playerID)
    {
        AcademyValue.numberOfHits[playerID - 1]++;
    }

    public static void AddKillStatistics(int playerID)
    {
        AcademyValue.numberOfKills[playerID - 1]++;
    }

    public static void AddDeathStatistics(int playerID)
    {
        AcademyValue.numberOfDeath[playerID - 1]++;
    }

    public void UpdateInfo()
    {
        StringBuilder info = new StringBuilder();

        foreach (Player player in playerList)
        {
            info.AppendFormat("Agent {0}\n", player.agentID.ToString());
            info.AppendFormat("Health : {0}\n", player.playerHealth.Health.ToString());
            info.AppendFormat("Bullet : {0}\n", player.playerShooting.BulletCount.ToString());

            float accuracy = AcademyValue.numberOfShoots[player.agentID - 1] != 0 ?
                ((float)AcademyValue.numberOfHits[player.agentID - 1] / (float)AcademyValue.numberOfShoots[player.agentID - 1]) * 100f
                : 0.0f;

            info.AppendFormat("Accuracy : {0}%\n", accuracy.ToString());
            info.AppendFormat("Kill : {0}\n", AcademyValue.numberOfKills[player.agentID - 1]);
            info.AppendFormat("Death : {0}\n", AcademyValue.numberOfDeath[player.agentID - 1]);

            info.AppendLine();
        }

        infoText.text = info.ToString();
    }
}
