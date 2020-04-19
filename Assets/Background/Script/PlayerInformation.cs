using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    public List<Player> playerList = new List<Player>();
    public Text infoText;

    public void ClearList()
    {
        playerList.Clear();
        infoText.text = "";
    }

    public void AddPlayer(Player player)
    {
        playerList.Add(player);
    }

    public void UpdateInfo()
    {
        StringBuilder info = new StringBuilder();

        foreach (Player player in playerList)
        {
            info.AppendFormat("Agent {0}\n", player.agentID.ToString());
            info.AppendFormat("Health : {0}\n", player.playerHealth.Health.ToString());
            info.AppendFormat("Bullet : {0}\n", player.playerShooting.BulletCount.ToString());
            info.AppendLine();
        }

        infoText.text = info.ToString();
    }
}
