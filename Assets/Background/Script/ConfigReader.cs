using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class ConfigReader
{
    public StringReader inputEnvironment;
    public StringReader inputRewardSystem;
    Deserializer deserializer;

    public ConfigReader()
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/../../config/unity_config.yaml");
        string fileContents = sr.ReadToEnd();
        sr.Close();

        inputEnvironment = new StringReader(fileContents);

        sr = new StreamReader(Application.dataPath + "/../../config/reward_system_config.yaml");
        fileContents = sr.ReadToEnd();
        sr.Close();

        inputRewardSystem = new StringReader(fileContents);

        DeserializerBuilder deserializerBuilder = new DeserializerBuilder();
        deserializerBuilder.WithNamingConvention(new CamelCaseNamingConvention());
        deserializer = deserializerBuilder.Build();
    }

    public Environment ReadEnvironment()
    {
        Environment environment = deserializer.Deserialize<Environment>(inputEnvironment);

        return environment;
    }

    public RewardSystem ReadRewardSystem()
    {
        RewardSystem rewardSystem = deserializer.Deserialize<RewardSystem>(inputRewardSystem);

        return rewardSystem;
    }

    public class Environment
    {
        public int ArenaRows { get; set; }
        public int ArenaColumns { get; set; }
        public int BulletPackNumber { get; set; }
        public int AgentNumber { get; set; }
        public int NormalNumber { get; set; }
        public int AggresiveNumber { get; set; }
        public int PassiveNumber { get; set; }
        public int GenerateStep { get; set; }
    }

    public class RewardSystem
    {
        public float ItemFoundReward { get; set; }
        public float BulletHitReward { get; set; }
        public float KillReward { get; set; }
        public float WinReward { get; set; }
        public float MoveReward { get; set; }
        public float DeathPunishment { get; set; }
        public float BulletMissPunishment { get; set; }
        public float DamagePunishment { get; set; }
        public float MovePunishment { get; set; }
    }


}
