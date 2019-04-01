// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Utils.Converters
{
    using System;
    using API.Models;
    using Newtonsoft.Json;

    public class JoinableByConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JoinableBy);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string value = reader.Value.ToString();
            JoinableBy joinableBy = JoinableBy.Everybody;

            if (!value.Contains("Demoted"))
            {
                joinableBy = JoinableBy.AUG_And_Agents;
            }

            if (!value.Contains("Agents"))
            {
                joinableBy = JoinableBy.AUG_Only;
            }

            return joinableBy;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().Replace("_", " ", StringComparison.Ordinal));
        }
    }
}
