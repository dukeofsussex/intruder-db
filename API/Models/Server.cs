// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using API.Utils;
    using API.Utils.Converters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

#pragma warning disable CA1707 // Identifiers should not contain underscores
    public enum JoinableBy
    {
        None = 0,
        AUG_Only = 1,
        AUG_And_Agents = 2,
        Everybody = 3
    }

    public enum VisibleBy
    {
        None = 0,
        Nobody = 1,
        AUG_Only = 2,
        Everybody = 3
    }
#pragma warning restore CA1707 // Identifiers should not contain underscores

    public class Server
    {
        public Server()
        {
            this.OnlineAgents = new List<AgentProfile>();
        }

        [JsonInput("id")]
        public int ID { get; set; }

        [JsonInput("uuid")]
        public string UUID { get; set; }

        [JsonInput("name")]
        public string Name { get; set; }

        [JsonInput("fancyname")]
        public string FancyName { get; set; }

        [JsonInput("description")]
        public string Description { get; set; }

        [JsonInput("region")]
        public string Region { get; set; }

        [JsonInput("servertype")]
#pragma warning disable CA1721 // Property names should not match get methods
        public string Type { get; set; }
#pragma warning restore CA1721 // Property names should not match get methods

        [JsonInput("serverstyle")]
        public string Style { get; set; }

        [JsonInput("serverversion")]
        public int Version { get; set; }

        [JsonInput("levelname")]
        [JsonConverter(typeof(LevelConverter))]
        public Map Map { get; set; }

        [JsonInput("leveltype")]
        public string MapType { get; set; }

        [JsonInput("playercount")]
        public short Agents { get; set; }

        [JsonInput("maxplayers")]
        public short MaxAgents { get; set; }

        [JsonInput("spectatorslots")]
        public short MaxSpectators { get; set; }

        [JsonInput("gamemode")]
        public string Gamemode { get; set; }

        [JsonInput("timemode")]
        public string Timemode { get; set; }

        [JsonInput("timetime")]
        public int Time { get; set; }

        [JsonInput("gameprogress")]
        [JsonConverter(typeof(BooleanConverter))]
        public bool InProgress { get; set; }

        [JsonInput("ranked")]
        [JsonConverter(typeof(BooleanConverter))]
        public bool Ranked { get; set; }

        [JsonInput("joinableby")]
        [JsonConverter(typeof(JoinableByConverter))]
        public JoinableBy JoinableBy { get; set; }

        [JsonInput("visibleby")]
        [JsonConverter(typeof(VisibleByConverter))]
        public VisibleBy VisibleBy { get; set; }

        [JsonInput("password")]
        [JsonConverter(typeof(PasswordConverter))]
        public bool Passworded { get; set; }

        [JsonInput("masterplayer")]
        public string MasterAgent { get; set; }

        [JsonInput("playernames")]
        public string AgentNames { get; set; }

        public List<AgentProfile> OnlineAgents { get; private set; }

        [JsonInput("masterip")]
        public string MasterIP { get; set; }

        [JsonInput("serverip")]
        public string ServerIP { get; set; }

        [JsonInput("updateip")]
        public string UpdateIP { get; set; }

        [JsonInput("timestamp")]
        public DateTime LastUpdate { get; set; }

        [JsonInput("rules")]
        public JRaw Rules { get; set; }

        [JsonInput("tuning")]
        public JRaw Tuning { get; set; }

        public bool ShouldSerializeAgentNames()
        {
            return false;
        }

        public bool ShouldSerializeMasterIP()
        {
            return false;
        }

        public bool ShouldSerializeServerIP()
        {
            return false;
        }

        public bool ShouldSerializeUpdateIP()
        {
            return false;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            string currentRules = JsonConvert.DeserializeObject<string>(this.Rules.ToString(CultureInfo.InvariantCulture));
            string rules = string.Empty;
            string tuning = string.Empty;

            int index = currentRules.LastIndexOf("}|", StringComparison.Ordinal);

            // Both rules and tuning objects are present
            if (index != -1)
            {
                rules = currentRules.Substring(0, index + 1);
                tuning = JToken.Parse(currentRules.Substring(index + 2)).ToString(Formatting.None);
            }
            else
            {
                rules = currentRules;
                tuning = "{}";
            }

            if (string.IsNullOrEmpty(rules.ToString(CultureInfo.InvariantCulture)))
            {
                rules = "{}";
            }
            else
            {
                JToken tokenRules = JsonConvert.DeserializeObject<JToken>(rules);
                JArray mapCycle = (JArray)tokenRules.SelectToken("MatchMode.currentMapCycle");

                if (mapCycle != null && mapCycle.HasValues)
                {
                    mapCycle.ReplaceAll(Util.GetUpdatedMapCycle(mapCycle.Children()));
                }

                rules = tokenRules.ToString(Formatting.None);
            }

            this.Rules = new JRaw(rules);
            this.Tuning = new JRaw(tuning);
        }
    }
}
