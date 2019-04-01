// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System;

    public class AgentForumProfile
    {
        public int ID { get; set; }

        public AgentRole Role { get; set; }

        public DateTime Registered { get; set; }
    }
}
