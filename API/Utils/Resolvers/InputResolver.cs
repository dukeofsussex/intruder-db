// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Utils.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Newtonsoft.Json.Serialization;

    public class InputResolver : DefaultContractResolver
    {
        public InputResolver(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();

            this.PropertyMappings = new Dictionary<string, string>();

            foreach (PropertyInfo prop in properties)
            {
                JsonInputAttribute inputName = prop.GetCustomAttribute(typeof(JsonInputAttribute)) as JsonInputAttribute;
                this.PropertyMappings.Add(prop.Name, inputName == null ? prop.Name : inputName.Name);
            }
        }

        private Dictionary<string, string> PropertyMappings { get; set; }

        protected override string ResolvePropertyName(string propertyName)
        {
            bool resolved = this.PropertyMappings.TryGetValue(propertyName, out string resolvedName);
            return resolved ? resolvedName : base.ResolvePropertyName(propertyName);
        }
    }
}
