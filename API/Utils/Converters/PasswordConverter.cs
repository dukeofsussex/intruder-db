// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Utils.Converters
{
    using System;
    using Newtonsoft.Json;

    // It's a joke that this is necessary,
    // but SBG's "API" is garbage and the "passworded" field is always "0"
    public class PasswordConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value.ToString().Length > 0;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
