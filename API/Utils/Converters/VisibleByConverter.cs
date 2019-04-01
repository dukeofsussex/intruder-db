﻿// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Utils.Converters
{
    using System;
    using API.Models;
    using Newtonsoft.Json;

    public class VisibleByConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JoinableBy);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string value = reader.Value.ToString();
            VisibleBy visibleBy = VisibleBy.Everybody;

            if (!value.Contains("Agents"))
            {
                visibleBy = VisibleBy.AUG_Only;
            }

            if (!value.Contains("AUG"))
            {
                visibleBy = VisibleBy.Nobody;
            }

            return visibleBy;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().Replace("_", " ", StringComparison.Ordinal));
        }
    }
}
