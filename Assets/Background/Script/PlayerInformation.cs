using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    // For shooting statistics
    public static int[] numberOfShoots;
    public static int[] numberOfHits;

    public List<Player> playerList = new List<Player>();
    public Text infoText;

    public void InitShootingInfo(int numberPlayer)
    {
        numberOfHits = new int[numberPlayer];
        numberOfShoots = new int[numberPlayer];
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
        numberOfShoots[playerID - 1]++;
    }

    public static void AddShootingHitStatistic(int playerID)
    {
        numberOfHits[playerID - 1]++;
    }

    public void UpdateInfo()
    {
        StringBuilder info = new StringBuilder();

        foreach (Player player in playerList)
        {
            info.AppendFormat("Agent {0}\n", player.agentID.ToString());
            info.AppendFormat("Health : {0}\n", player.playerHealth.Health.ToString());
            info.AppendFormat("Bullet : {0}\n", player.playerShooting.BulletCount.ToString());

            float accuracy = numberOfShoots[player.agentID - 1] != 0 ?
                ((float)numberOfHits[player.agentID - 1] / (float)numberOfShoots[player.agentID - 1]) * 100f
                : 0.0f;

            info.AppendFormat("Accuracy : {0}%\n", accuracy.ToString());
            info.AppendLine();
        }

        infoText.text = info.ToString();
    }
}
