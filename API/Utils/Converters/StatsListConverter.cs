// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Utils.Converters
{
    using System;
    using API.Models;
    using Newtonsoft.Json;

    public class StatsListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StatsList);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            StatsList statsList = value as StatsList;

            writer.WriteStartObject();

            foreach (IStats statsObject in statsList.Stats)
            {
                writer.WritePropertyName(statsObject.Identifier);
                serializer.Serialize(writer, statsObject.Item);
            }

            writer.WriteEndObject();
        }
    }
}
