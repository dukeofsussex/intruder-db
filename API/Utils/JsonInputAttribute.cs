// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Utils
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class JsonInputAttribute : Attribute
    {
        public JsonInputAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
