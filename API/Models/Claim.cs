// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System;

    public class Claim
    {
        public AgentProfile Claimer { get; set; }

        public Guid UID { get; set; }

        public bool Sent { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
