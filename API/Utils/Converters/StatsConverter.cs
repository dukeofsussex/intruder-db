// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Utils.Converters
{
    using System;
    using API.Models;
    using Newtonsoft.Json;

    public class StatsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IStats);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IStats statsObject = value as IStats;
            serializer.Serialize(writer, statsObject.Item);
        }
    }
}
