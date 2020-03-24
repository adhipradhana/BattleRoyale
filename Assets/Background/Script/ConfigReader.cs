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
    public StringReader input;
    Deserializer deserializer;

    public ConfigReader()
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/../../config/unity_config.yaml");
        string fileContents = sr.ReadToEnd();
        sr.Close();

        input = new StringReader(fileContents);

        DeserializerBuilder deserializerBuilder = new DeserializerBuilder();
        deserializerBuilder.WithNamingConvention(new CamelCaseNamingConvention());
        deserializer = deserializerBuilder.Build();
    }

    public Environment ReadEnvironment()
    {
        Environment environment = deserializer.Deserialize<Environment>(input);

        return environment;
    }

    public class Environment
    {
        public int MazeRows { get; set; }
        public int MazeColumns { get; set; }
        public int BulletPackNumber { get; set; }
        public int HealthPackNumber { get; set; }
        public int AgentNumber { get; set; }
        public int GenerateStep { get; set; }
        public List<Agent> Agents { get; set; }
    }

    public class Agent
    {
        public string Type { get; set; }
        public float Speed { get; set; }
    }

}
